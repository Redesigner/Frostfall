using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class DialogEntry : ScriptableObject
{
    [SerializeField] public string text;

    [SerializeField] public DialogEntry nextEntry;
}
