using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesBehavior : TouchBase
{
    public ParticleBehaviors particleBehavior;
    [SerializeField] private ParticleSystem[] particleSystems;
 
    public enum ParticleBehaviors
    {
        handleEmission,
        play
    }



    protected override Action MouseDownbehavior => HandleMouseClick;

    private void HandleMouseClick()
    {
        switch (particleBehavior)
        {
            case ParticleBehaviors.handleEmission:
                HandleParticleEmission();
                break;
            case ParticleBehaviors.play:
                Playparticle();
                break;

        }
    }

    public override void Awake()
    {
        if (detectRelease)
            MouseUpBehavior += HandleMouseUp;

        base.Awake();
    }

    private void HandleParticleEmission()
    {
        if (MouseDownClip) playMouseDownClip();
        foreach (ParticleSystem obj in particleSystems)
        {
            var em = obj.emission;
            em.enabled = true;
        }
    }
    private void Playparticle()
    {
        if (MouseDownClip) playMouseDownClip();
        foreach (ParticleSystem obj in particleSystems)
        {
            obj.Play();
        }
    }


    private void HandleMouseUp()
    {
        if (!detectRelease) return;

        foreach (ParticleSystem obj in particleSystems)
        {
            var em = obj.emission;
            em.enabled = false;
        }
        StopAudio();
    }
    private void OnDisable()
    {
        if (detectRelease)
            MouseUpBehavior -= HandleMouseUp;
    }

}


//foreach (ParticleSystem obj in particleSystems)
//{
//    var em = obj.emission;
//    em.enabled = true;
//    if (clip != null)
//        Sentnce_and_wordAudio.instance.playTouchNoiseClip(clip, true);

//}
//    }

//    public void OnMouseUp()
//{
//    //  if (sceneManager.instance.CurrentSceneNumber != 4) return;

//    foreach (ParticleSystem obj in particleSystems)
//    {
//        var em = obj.emission;
//        em.enabled = false;
//        if (clip != null)
//            Sentnce_and_wordAudio.instance.StopBackgroundNoiseClip();

//    }
