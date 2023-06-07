using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public abstract class TouchBase : MonoBehaviour
{

    protected abstract Action MouseDownbehavior { get; }
    [SerializeField] protected ParticleSystem ps;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected bool ShouldPulsate;
    [SerializeField] protected bool TurnOffOnClick;
    [SerializeField] protected bool ShouldClickAudioLoop;
    [SerializeField] protected bool IsPulsating = true;
    protected Action MouseUpAsButBehavior;
    protected Action MouseUpBehavior;
    
    [SerializeField] protected AudioClip MouseDownClip;
    [SerializeField] protected AudioClip MouseUpClip;
    public virtual void Awake()
    {
        if (!ShouldPulsate)
        {
            if (ps == null) return;
            SetParticleEmission(false);
        }

        if (ShouldClickAudioLoop)
        {
            setAudioClickLoop(true);
        }
        else
        {
            setAudioClickLoop(false);
        }

    }

    private void SetParticleEmission(bool value)
    {
        var em = ps.emission;
        em.enabled = value;
        IsPulsating = value;
    }



    private void OnMouseUp()
    {
        MouseUpBehavior?.Invoke();

    }

    private void OnMouseUpAsButton()
    {
        MouseUpAsButBehavior?.Invoke();
    }

    private void OnMouseDown()
    {
        MouseDownbehavior?.Invoke();


        if (TurnOffOnClick) SetParticleEmission(false); 
    }

    protected void playMouseDownClip()
    {
        audioSource.clip = MouseDownClip;
        audioSource.Play();
    }
    protected void playMouseUpClip()
    {
        audioSource.clip = MouseUpClip;
        audioSource.Play();
    }
    protected void StopAudio()
    {
        audioSource.Stop();
    }
    private void setAudioClickLoop(bool value)
    {
        audioSource.loop = value;
    }
}
