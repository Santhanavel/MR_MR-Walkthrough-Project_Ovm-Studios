using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvAutoAlign : MonoBehaviour
{
    public Transform positionAnchor;     // Anchor A
    public Transform rotationAnchor;     // Anchor B
    public float rotateSpeed = 30f;      // Degrees per second

    public bool stopRotation = true;
    public MRPlaceAndPersistEnv placeAndPersistEnv;
    private void Start()
    {
        if (placeAndPersistEnv == null)
            placeAndPersistEnv = FindObjectOfType<MRPlaceAndPersistEnv>();

        positionAnchor = placeAndPersistEnv.FindAnchorByGuid(placeAndPersistEnv.GUIDs[0]).gameObject.transform;
        rotationAnchor = placeAndPersistEnv.FindAnchorByGuid(placeAndPersistEnv.GUIDs[1]).gameObject.transform;
        
    }

    void Update()
    {
        if (stopRotation) return;
        if (rotationAnchor == null || positionAnchor == null) return;

        // Keep ENV at position anchor
        transform.position = positionAnchor.position;

        // Find direction from ENV to rotation anchor
        Vector3 direction = rotationAnchor.position - transform.position;
        direction.y = 0f; // keep only horizontal direction

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                rotateSpeed * Time.deltaTime
            );
        }
        else
        {
            if(!stopRotation)
            {
                stopRotation = true;
                Debug.Log("Rotation stopped — collider hit.");
            }
        }
    }

    // Stop when colliders collide
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RotationStop"))
        {
            stopRotation = true;
            Debug.Log("Rotation stopped — collider hit.");
        }
    }
    public void Rotate()
    {
        stopRotation = false;
    }
}

