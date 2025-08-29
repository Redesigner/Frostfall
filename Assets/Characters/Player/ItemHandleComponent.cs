using UnityEngine;
using UnityEngine.InputSystem;

public class ItemHandleComponent : MonoBehaviour
{
    private CarryableObject _heldItem;
    
    public bool holdingItem => _heldItem != null;

    public void PickupItem(CarryableObject item)
    {
        _heldItem = item;
        item.Pickup();
        item.transform.SetParent(transform, false);
        item.transform.localPosition = Vector3.zero;
    }

    public void ThrowItem(Vector2 velocity, Vector2 parentVelocity)
    {
        if (!_heldItem)
        {
            return;
        }

        _heldItem.transform.SetParent(null);
        _heldItem.transform.position = transform.position;
        _heldItem.Release(velocity * 4.0f + parentVelocity);
        _heldItem = null;
    }
}
