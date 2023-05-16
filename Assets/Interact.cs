using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Interact : MonoBehaviour, IPointerClickHandler
{
    public bool shouldTurnOff;

    public Action<PointerEventData> InteractionBehavior;
    private BoxCollider2D boxCollider;
    private ParticleSystem.EmissionModule particleEmission;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        particleEmission = GetComponent<ParticleSystem>().emission;
       
    }

    public void AddInteraction(Action<PointerEventData> action)
    {
        InteractionBehavior += action;
    }
    public void RemoveInteraction(Action<PointerEventData> action)
    {
        InteractionBehavior -= action;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (shouldTurnOff)
        {
            boxCollider.enabled = false;
            particleEmission.enabled = false;
        }

        InteractionBehavior?.Invoke(eventData);
    }
}
