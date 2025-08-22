using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(KinematicCharacterController))]
public class InteractComponent : MonoBehaviour
{
    // We need the kinematic character controller to tell our look direction!
    private KinematicCharacterController _controller;

    [SerializeField] private LayerMask interactionMask;

    [SerializeField] [Min(0.0f)] private float debounceTime = 0.2f;

    private bool _canInteract = true;

    private void Awake()
    {
        _controller = GetComponent<KinematicCharacterController>();
    }

    public void TryInteract(InputAction.CallbackContext context)
    {
        if (!context.performed || Time.timeScale == 0.0f || !_canInteract)
        {
            return;
        }
        
        // ReSharper disable once Unity.PreferNonAllocApi
        // This overlap check is not very frequent
        var result = Physics2D.OverlapBoxAll(
            new Vector2(transform.position.x, transform.position.y) + (_controller.lookDirection * 4.0f),
            new Vector2(8.0f, 8.0f), 0.0f, interactionMask);

        foreach (var collision in result)
        {
            var interactable = collision.GetComponent<InteractableComponent>();
            if (!interactable)
            {
                continue;
            }

            interactable.InteractWith(this);
            _canInteract = false;
            TimerManager.instance.CreateTimer(this, debounceTime, () => { _canInteract = true; });
            break;
        }
    }

    public void PlayDialog(DialogEntry dialog)
    {
        GameState.instance.Pause();
        FindFirstObjectByType<HudController>().DisplayDialog(dialog);
    }
}
