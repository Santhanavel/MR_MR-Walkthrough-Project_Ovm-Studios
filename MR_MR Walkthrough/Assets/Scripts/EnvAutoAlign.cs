using System.Collections;
using UnityEngine;

public class EnvAutoAlign : MonoBehaviour
{
    public Transform positionAnchor;     // Anchor A (position)
    public Transform rotationAnchor;     // Anchor B (rotation)
    public float rotateSpeed = 30f;

    public bool stopRotation = true;

    public MRPlaceAndPersistEnv placeAndPersistEnv;

    private bool anchorsAssigned = false;


    private const string RotX = "EnvRotX";
    private const string RotY = "EnvRotY";
    private const string RotZ = "EnvRotZ";
    private const string RotW = "EnvRotW";

  
    public void Rotate()
    {
        if(!HasSavedRotation())
        {
            stopRotation = false;
            StartCoroutine(AssignAnchorsAndRotate());
        }
        else
        {
            LoadRotation(transform);
        }
      
    }

    /// <summary>
    /// Waits until both anchors are found in the scene
    /// </summary>
    IEnumerator AssignAnchorsAndRotate()
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
                GameObject posObj = placeAndPersistEnv.FindAnchor(placeAndPersistEnv.GUIDs[0]).gameObject;
                GameObject rotObj = placeAndPersistEnv.FindAnchor(placeAndPersistEnv.GUIDs[1]).gameObject;

                if (posObj != null && rotObj != null)
                {
                    positionAnchor = posObj.transform;
                    rotationAnchor = rotObj.transform;
                    anchorsAssigned = true;
                    break;
                }
            }

            t += Time.deltaTime;
            yield return null;
        }

        if (!anchorsAssigned)
        {
            Debug.LogError("EnvAutoAlign: Could not find anchors within timeout.");
            stopRotation = true;
            yield break;
        }
    }

    private void Update()
    {
        if (stopRotation || !anchorsAssigned) return;
        if (positionAnchor == null || rotationAnchor == null) return;

        // Keep ENV at correct real-world position
        transform.position = positionAnchor.position;

        // Direction from ENV to rotation anchor
        Vector3 direction = rotationAnchor.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                rotateSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!stopRotation)
        {
            if (other.CompareTag("RotationStop"))
            {
                stopRotation = true;
                if (!HasSavedRotation())
                    SaveRotation(transform.rotation);
            }
        }
       
    }
    // Save rotation once the alignment is done
    public static void SaveRotation(Quaternion rotation)
    {
        PlayerPrefs.SetFloat(RotX, rotation.x);
        PlayerPrefs.SetFloat(RotY, rotation.y);
        PlayerPrefs.SetFloat(RotZ, rotation.z);
        PlayerPrefs.SetFloat(RotW, rotation.w);

        PlayerPrefs.Save();
        Debug.Log("Environment rotation saved.");
    }

    // Check if saved rotation exists
    public bool HasSavedRotation()
    {
        return PlayerPrefs.HasKey(RotW);
    }

    // Load rotation and apply to object
    public void LoadRotation(Transform env)
    {
        Quaternion rot = new Quaternion(
            PlayerPrefs.GetFloat(RotX),
            PlayerPrefs.GetFloat(RotY),
            PlayerPrefs.GetFloat(RotZ),
            PlayerPrefs.GetFloat(RotW)
        );

        env.rotation = rot;
        Debug.Log("Applied saved environment rotation.");
    }

    // Remove rotation data on delete
    public void ClearRotation()
    {
        PlayerPrefs.DeleteKey(RotX);
        PlayerPrefs.DeleteKey(RotY);
        PlayerPrefs.DeleteKey(RotZ);
        PlayerPrefs.DeleteKey(RotW);

        Debug.Log("Environment rotation cleared.");
    }
}
