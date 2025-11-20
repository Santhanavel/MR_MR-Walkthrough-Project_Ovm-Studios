using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerFinder : MonoBehaviour
{
    public GameObject triggerPrefab;
    public UnityEvent OntriggerObject;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == triggerPrefab)
        {

            OntriggerObject?.Invoke();
            other.transform.position = transform.position;
            other.transform.rotation = transform.rotation;
        }
    }

}
