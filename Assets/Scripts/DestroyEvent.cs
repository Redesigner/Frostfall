using System;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public sealed class DestroyEvent : MonoBehaviour
{
    [SerializeField] public UnityEvent destroyed;

    private void OnDestroy()
    {
        destroyed.Invoke();
    }
}
