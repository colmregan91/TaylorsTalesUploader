using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimationControl : MonoBehaviour
{
    public bool shouldTurnOff;

    public Action<PointerEventData> InteractionBehavior;
    private BoxCollider2D boxCollider;
    private ParticleSystem.EmissionModule particleEmission;

    public Animator anim;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        particleEmission = GetComponent<ParticleSystem>().emission;

    }
    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if (shouldTurnOff)
    //    {
    //        boxCollider.enabled = false;
    //        particleEmission.enabled = false;
    //    }

    //    anim.SetTrigger("AnimTrigger");
    //}

    private void OnMouseDown()
    {
        if (shouldTurnOff)
        {
            boxCollider.enabled = false;
            particleEmission.enabled = false;
        }
        anim.SetTrigger("AnimTrigger");
    }
}
