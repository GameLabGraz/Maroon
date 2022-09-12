using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationPanel : MonoBehaviour
{
    public TMPro.TextMeshProUGUI MainNotificationText;
    public TMPro.TextMeshProUGUI DetailNotificationText;

    public void SetNotification(string notification, string detail)
    {
        gameObject.SetActive(!string.IsNullOrWhiteSpace(notification) || !string.IsNullOrWhiteSpace(detail));

        if (MainNotificationText != null)
        {
            MainNotificationText?.gameObject.SetActive(!string.IsNullOrWhiteSpace(notification));
            MainNotificationText.text = notification;
        }

        if (DetailNotificationText != null)
        {
            DetailNotificationText?.gameObject.SetActive(!string.IsNullOrWhiteSpace(detail));
            DetailNotificationText.text = detail;
        }
    }
}
