using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TMP_Text TimerVisualText;

    public bool isTimerStart = false;
    private float timer = 0;
    // Start is called before the first frame update
   public void StartTimer()
    {
        isTimerStart = true;    
    }
    public void StopTimer()
    {
        isTimerStart = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (isTimerStart)
        {
            timer += Time.deltaTime;

            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);

            TimerVisualText.text = $"{minutes:00}:{seconds:00}";
        }
     
    }
}
