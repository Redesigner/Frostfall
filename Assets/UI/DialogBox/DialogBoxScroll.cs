using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogBoxScroll : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private float _currentCharacterDisplayTime;
    private bool _playing = false;
    private DialogEntry _currentDialog;

    [SerializeField]
    private float characterStepTime = 0.5f;
    
    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _text.maxVisibleCharacters = 0;
    }

    private void Update()
    {
        if (!_playing)
        {
            return;
        }
        _currentCharacterDisplayTime += Mathf.Min(Time.unscaledDeltaTime, characterStepTime);

        if (!(_currentCharacterDisplayTime >= characterStepTime))
        {
            return;
        };
        
        _currentCharacterDisplayTime -= characterStepTime;
        ++_text.maxVisibleCharacters;
    }

    public void SetDialog(DialogEntry dialog)
    {
        _playing = true;
        _currentDialog = dialog;
        _text.maxVisibleCharacters = 0;
    }
}
