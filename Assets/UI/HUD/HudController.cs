using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [SerializeField] private DialogBoxScroll dialogBox;
    [SerializeField] private GameObject dialogChild;

    [SerializeField] private PixelPerfectCamera pixelCamera;

    private CanvasScaler _canvasScaler;

    public void Awake()
    {
        _canvasScaler = GetComponent<CanvasScaler>();
    }

    public void DisplayDialog(DialogEntry dialog)
    {
        dialogChild.SetActive(true);
        dialogBox.SetDialog(dialog);
    }

    private void OnRectTransformDimensionsChange()
    {
        if (!pixelCamera || !isActiveAndEnabled)
        {
            return;
        }
        
        _canvasScaler.scaleFactor = pixelCamera.pixelRatio;
    }
}
