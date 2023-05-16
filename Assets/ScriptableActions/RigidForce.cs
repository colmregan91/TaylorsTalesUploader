using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RigidForce : MonoBehaviour, Iinteractable
{
    private Rigidbody2D rb;
    private Interact interactObj;

    public float GravityScale;
    public Vector2 AppliedForce;
    public ForceMode2D forceMode;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        interactObj = transform.parent.GetComponent<Interact>();
        interactObj.AddInteraction(InteractionBehavior);
    }
    public void InteractionBehavior(PointerEventData eventData)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = GravityScale;

        rb.AddForce(AppliedForce, forceMode);
    }

    private void OnDisable()
    {
        interactObj.RemoveInteraction(InteractionBehavior);
    }
}

