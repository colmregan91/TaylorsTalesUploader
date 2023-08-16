using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public abstract class TouchBase : MonoBehaviour
{

    protected abstract Action MouseDownbehavior { get; }
    [SerializeField] protected ParticleSystem PulsateParticles;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected bool ShouldPulsate;
    [SerializeField] protected bool TurnOffOnClick;
    [SerializeField] protected bool ShouldClickAudioLoop;
    [SerializeField] public bool IsPulsating = true;
    [SerializeField] protected float waitTime;
    [SerializeField] protected bool detectRelease;
    protected Action MouseUpAsButBehavior;
    protected Action MouseUpBehavior;

    [SerializeField] protected AudioClip MouseDownClip;
    [SerializeField] protected AudioClip MouseUpClip;


    public virtual void Awake()
    {
        SetMixerGroup();

        if (!ShouldPulsate)
        {
            if (PulsateParticles == null) return;
            SetParticleEmission(false);
        }

        if (audioSource == null) return;

        if (ShouldClickAudioLoop)
        {
            setAudioClickLoop(true);
        }
        else
        {
            setAudioClickLoop(false);
        }
    }

    public void SetMixerGroup()
    {
        if (audioSource != null)
        {
            AudioMixer mixerGroup = Resources.Load<AudioMixer>("main");
            var group = mixerGroup.FindMatchingGroups("TouchNoises")[0];
            if (mixerGroup != null)
            {
                audioSource.outputAudioMixerGroup = group;
            }

        }

    }

    public void SetParticleEmission(bool value)
    {
        IsPulsating = value;
        if (PulsateParticles == null) return;

        var em = PulsateParticles.emission;
        em.enabled = value;

    }



    private void OnMouseUp()
    {
        MouseUpBehavior?.Invoke();
        if (detectRelease) setEmissionOnInvoke();

    }

    private void OnMouseUpAsButton()
    {
        MouseUpAsButBehavior?.Invoke();
    }

    private void OnMouseDown()
    {
        if (!IsPulsating) return;


        MouseDownbehavior?.Invoke();
        if (TurnOffOnClick)
        {
            SetParticleEmission(false);
            enabled = false;
            return;
        }
        if (!ShouldPulsate) return; // if should pulsta is false and waittime not zero, do waitime check in inheritor 
        if (waitTime != 0)
        {
            if (!detectRelease)
            {
                SetParticleEmission(false);
                Invoke("setEmissionOnInvoke", waitTime);
            }

        }
        if (detectRelease) SetParticleEmission(false);


    }
    private void setEmissionOnInvoke()
    {
        SetParticleEmission(true);
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
