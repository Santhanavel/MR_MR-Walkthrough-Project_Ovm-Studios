using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireHolder : MonoBehaviour
{
    public List<GameObject> Fire;
    public int FireCount = 0;
    public  UnityEvent taskCompleted;

    public void AddFireCount()
    {
        FireCount++;

        if (FireCount == Fire.Count) 
        {
            taskCompleted?.Invoke();
        }
    }
}
