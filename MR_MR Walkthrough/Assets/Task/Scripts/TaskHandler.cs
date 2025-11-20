using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TaskHandler : MonoBehaviour
{
    public string currentTaskName;
    public List<Task> interactions = new List<Task>();

    public int currentInteraction = 0;
    public ScoreManager scoreManager;
    public void InvokeInteraction()
    {
        interactions[currentInteraction].TaskEvents?.Invoke();
        currentTaskName = interactions[currentInteraction].taskName;
        scoreManager.AddScore(interactions[currentInteraction].score);
    }

    public void NextInteraction()
    {
        currentInteraction++;
        if (currentInteraction < interactions.Count)
        {
            InvokeInteraction();
        }
    }

}
[System.Serializable]
public class Task
{
    public string taskName;
    public string taskDescription;

    public UnityEvent TaskEvents;
    public int score = 0;
}