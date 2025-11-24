using System.Collections.Generic;
using UnityEngine;

public enum NotificationType
{
    CreateAnchor,
    SaveSuccess,
    LoadSuccess,
    ClearSuccess,
    Error
}

public class NotificationData : MonoBehaviour
{
    [Header("Notification Entries")]
    public List<NotificationEntry> entries;

    [Header("Error Icon (Used for all errors)")]
    public Sprite errorIcon;

    public void Show(NotificationType type, string overrideMessage = null)
    {
        // ERROR notification (always use error icon)
        if (type == NotificationType.Error)
        {
            string msg = string.IsNullOrEmpty(overrideMessage) ? "Something went wrong!" : overrideMessage;
            MetaNotificationManager.instance.Show(msg, errorIcon);
            return;
        }

        // Find matching entry
        var entry = entries.Find(e => e.type == type);

        if (entry != null)
        {
            string msg = string.IsNullOrEmpty(overrideMessage) ? entry.message : overrideMessage;
            MetaNotificationManager.instance.Show(msg, entry.icon);
        }
        else
        {
            MetaNotificationManager.instance.Show("Missing notification entry!", errorIcon);
        }
    }
}

[System.Serializable]
public class NotificationEntry
{
    public NotificationType type;
    public string message;
    public Sprite icon;
}