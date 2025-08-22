using UnityEngine;

public class HudController : MonoBehaviour
{
    [SerializeField] private DialogBoxScroll dialogBox;

    public void DisplayDialog(DialogEntry dialog)
    {
        dialogBox.SetDialog(dialog);
    }
}
