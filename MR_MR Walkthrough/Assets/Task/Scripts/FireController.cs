using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class FireController : MonoBehaviour
{

    public Slider HealthSlider;
    public GameObject water;
    public float health = 1;

    public bool isHiting = false;
    private bool isSwipe = true;
    public FireHolder fire;

    private void Update()
    {
        if(isHiting && isSwipe)
        {
            if (health >= 0)
            {
                health -= Time.deltaTime;
                HealthSlider.value = health;
            }

            if (health == 0.5f)
            {
                transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            }
            if (health < 0.25f)
            {
                float size = health ;
                transform.localScale = new Vector3(size, size, size);
            }

            if(health <= 0.05)
            {
                transform.localScale = Vector3.zero;
                fire.AddFireCount();
                gameObject.SetActive(false);
                HealthSlider.gameObject.SetActive(false);
            }
        }

       
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == water)
        {
            isHiting = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == water)
        {
            isHiting = false;
        }

    }
}
