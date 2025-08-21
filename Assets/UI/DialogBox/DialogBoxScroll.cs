using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogBoxScroll : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private float _currentCharacterDisplayTime;

    [SerializeField]
    private float characterStepTime = 0.5f;
    
    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _text.maxVisibleCharacters = 0;
    }

    private void Update()
    {
        _currentCharacterDisplayTime += Time.unscaledTime;

        if (!(_currentCharacterDisplayTime >= characterStepTime))
        {
            return;
        };
        
        _currentCharacterDisplayTime -= characterStepTime;
        ++_text.maxVisibleCharacters;
    }
}
