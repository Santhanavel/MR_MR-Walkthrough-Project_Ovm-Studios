using Meta.XR;
using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaNotificationManager : MonoBehaviour
{
    public OVRCameraRig _cameraRig;
    public Transform _anchorPrefabTransform;

    public List<Image> images;
    public TMP_Text messageText;

    private List<Color> imageBaseColors = new List<Color>();
    private Color textBaseColor;

    public float showDuration = 2f;
    public float fadeSpeed = 3f;

    public static MetaNotificationManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        _anchorPrefabTransform.parent = _cameraRig.rightControllerAnchor;
        _anchorPrefabTransform.localPosition = Vector3.zero;
        _anchorPrefabTransform.localRotation = Quaternion.identity;

        foreach (var img in images)
            imageBaseColors.Add(img.color);

        textBaseColor = messageText.color;

        SetAlpha(0);
    }

    public void Show(string message, Sprite icon)
    {
        StopAllCoroutines();

        messageText.text = message;
        images[1].sprite = icon;

        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        // Fade in
        for (float a = 0; a <= 1f; a += Time.deltaTime * fadeSpeed)
        {
            SetAlpha(a);
            yield return null;
        }

        yield return new WaitForSeconds(showDuration);

        // Fade out
        for (float a = 1f; a >= 0; a -= Time.deltaTime * fadeSpeed)
        {
            SetAlpha(a);
            yield return null;
        }
        SetAlpha(0);

    }

    private void SetAlpha(float alpha)
    {
        for (int i = 0; i < images.Count; i++)
        {
            var baseColor = imageBaseColors[i];
            images[i].color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        }

        messageText.color = new Color(textBaseColor.r, textBaseColor.g, textBaseColor.b, alpha);
    }
}
