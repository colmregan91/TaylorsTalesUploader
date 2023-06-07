
using System;
using UnityEngine;

public class AnimationBehavior : TouchBase
{
    [SerializeField] private Animator anim;
    [SerializeField] private float waitTime = 1;

    private int triggerHash = Animator.StringToHash("AnimTrigger");
    private float clickTime;

    protected override Action MouseDownbehavior { get => animBehavior; }


    private void animBehavior()
    {
        if (!IsPulsating) return;
        if (Time.time < clickTime + waitTime) return;
        if (MouseDownClip) playMouseDownClip();
        anim.SetTrigger(triggerHash);
        clickTime = Time.time;
    }
}
