using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationData : MonoBehaviour
{
   public List<Data> data;

    public void ShowPopUp(int index)
    {
        MetaNotificationManager.instance.Show(data[index]._message , data[index]._icon);
    }
}

[System.Serializable]
public class Data
{
    public string _message;
    public Sprite _icon;
}