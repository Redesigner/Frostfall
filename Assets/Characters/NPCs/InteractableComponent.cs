using UnityEngine;

public class InteractableComponent : MonoBehaviour
{
    [SerializeField]
    private DialogEntry dialogToDisplay;
    public void InteractWith(InteractComponent source)
    {
        source.PlayDialog(dialogToDisplay);
    }
}
