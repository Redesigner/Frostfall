using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogBoxScroll : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private float _currentCharacterDisplayTime;
    private bool _playing = false;
    private DialogEntry _currentDialog;

    [SerializeField]
    private float characterStepTime = 0.5f;

    [SerializeField] private GameObject cursorObject;
    
    private void Awake()
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

        if (_text.maxVisibleCharacters < _currentDialog.text.Length)
        {
            return;
        }
        
        _playing = false;
        cursorObject.SetActive(true);
    }

    public void SetDialog(DialogEntry dialog)
    {
        _playing = true;
        _currentDialog = dialog;
        _text.text = dialog.text;
        _text.maxVisibleCharacters = 0;
        cursorObject.SetActive(false);
    }

    public void Advance(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (_playing)
        {
            return;
        }
        
        // NOOOOOOOOOOO
        gameObject.transform.parent.gameObject.SetActive(false);
        GameState.instance.Unpause();
    }
}
