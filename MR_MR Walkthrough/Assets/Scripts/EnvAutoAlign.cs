using System.Collections;
using UnityEngine;

public class EnvAutoAlign : MonoBehaviour
{
    public Transform positionAnchor;
    public Transform rotationAnchor;
    public float rotateSpeed = 30f;

    public bool stopRotation = true;
    public MRPlaceAndPersistEnv placeAndPersistEnv;

    private bool anchorsAssigned = false;

    // Relative rotation keys
    private const string RelRotX = "EnvRelRotX";
    private const string RelRotY = "EnvRelRotY";
    private const string RelRotZ = "EnvRelRotZ";
    private const string RelRotW = "EnvRelRotW";

    private Quaternion savedRelativeRotation;

    public void Rotate()
    {
        if (!HasSavedRotation())
        {
            stopRotation = false;
            StartCoroutine(AssignAnchorsAndRotate());
        }
        else
        {
            StartCoroutine(AssignAnchorsAndLoad());
        }
    }

    // ---------------------------
    // FIND ANCHORS
    // ---------------------------
    IEnumerator AssignAnchorsAndRotate()
    {
        yield return StartCoroutine(FindAnchors());
        if (!anchorsAssigned) stopRotation = true;
    }

    IEnumerator AssignAnchorsAndLoad()
    {
        yield return StartCoroutine(FindAnchors());
        if (anchorsAssigned) LoadRotation();
    }

    IEnumerator FindAnchors()
    {
        anchorsAssigned = false;
        float timeout = 10f;
        float t = 0f;

        if (placeAndPersistEnv == null)
            placeAndPersistEnv = FindObjectOfType<MRPlaceAndPersistEnv>();

        while (!anchorsAssigned && t < timeout)
        {
            if (placeAndPersistEnv.GUIDs.Count >= 2)
            {
                var posObj = placeAndPersistEnv.FindAnchor(placeAndPersistEnv.GUIDs[0])?.gameObject;
                var rotObj = placeAndPersistEnv.FindAnchor(placeAndPersistEnv.GUIDs[1])?.gameObject;

                if (posObj != null && rotObj != null)
                {
                    positionAnchor = posObj.transform;
                    rotationAnchor = rotObj.transform;
                    anchorsAssigned = true;
                }
            }

            t += Time.deltaTime;
            yield return null;
        }
    }

    // ---------------------------
    // UPDATE LOOP
    // ---------------------------
    private void Update()
    {
        if (!anchorsAssigned) return;

        // Always follow position anchor
        if (positionAnchor != null)
            transform.position = positionAnchor.position;

        if (!stopRotation)
            RotateTowardAnchor();
    }

    // ---------------------------
    // ROTATION WITH THRESHOLD
    // ---------------------------
    private void RotateTowardAnchor()
    {
        if (rotationAnchor == null) return;

        Vector3 direction = rotationAnchor.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(direction);
        float angle = Quaternion.Angle(transform.rotation, targetRot);

        // ⭐ CRITICAL FIX: Stop rotating before perfect alignment
        if (angle < 2f)
        {
            stopRotation = true;

            if (!HasSavedRotation())
                SaveRotation();

            return; // Stop micro-rotation that breaks triggers
        }

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );
    }

    // ---------------------------
    // TRIGGER STOP
    // ---------------------------
    private void OnTriggerEnter(Collider other)
    {
        // Only listen if rotation is happening
        if (stopRotation) return;

        if (other.CompareTag("RotationStop"))
        {
            stopRotation = true;

            if (!HasSavedRotation())
                SaveRotation();
        }
    }

    // ---------------------------
    // SAVE / LOAD RELATIVE ROTATION
    // ---------------------------
    public void SaveRotation()
    {
        if (rotationAnchor == null) return;

        savedRelativeRotation = Quaternion.Inverse(rotationAnchor.rotation) * transform.rotation;

        PlayerPrefs.SetFloat(RelRotX, savedRelativeRotation.x);
        PlayerPrefs.SetFloat(RelRotY, savedRelativeRotation.y);
        PlayerPrefs.SetFloat(RelRotZ, savedRelativeRotation.z);
        PlayerPrefs.SetFloat(RelRotW, savedRelativeRotation.w);

        PlayerPrefs.Save();
    }

    public void LoadRotation()
    {
        if (!HasSavedRotation() || rotationAnchor == null)
            return;

        savedRelativeRotation = new Quaternion(
            PlayerPrefs.GetFloat(RelRotX),
            PlayerPrefs.GetFloat(RelRotY),
            PlayerPrefs.GetFloat(RelRotZ),
            PlayerPrefs.GetFloat(RelRotW)
        );

        transform.rotation = rotationAnchor.rotation * savedRelativeRotation;
    }

    public bool HasSavedRotation()
    {
        return PlayerPrefs.HasKey(RelRotW);
    }

    public void ClearRotation()
    {
        PlayerPrefs.DeleteKey(RelRotX);
        PlayerPrefs.DeleteKey(RelRotY);
        PlayerPrefs.DeleteKey(RelRotZ);
        PlayerPrefs.DeleteKey(RelRotW);
    }
}
