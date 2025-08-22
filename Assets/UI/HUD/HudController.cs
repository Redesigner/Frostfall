using UnityEngine;

public class HudController : MonoBehaviour
{
    [SerializeField] private DialogBoxScroll dialogBox;
    [SerializeField] private GameObject dialogChild;

    public void DisplayDialog(DialogEntry dialog)
    {
        dialogChild.SetActive(true);
        dialogBox.SetDialog(dialog);
    }
}
