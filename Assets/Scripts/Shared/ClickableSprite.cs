using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class ClickableSprite : MonoBehaviour
{
    [System.Serializable]
    public class InteractionEvent : UnityEvent { }

    public InteractionEvent onInteraction = new InteractionEvent();

    private void OnMouseDown()
    {
        onInteraction.Invoke();
    }
}
