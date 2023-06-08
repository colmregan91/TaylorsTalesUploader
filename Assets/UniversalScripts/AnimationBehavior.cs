
using System;
using UnityEngine;

public class AnimationBehavior : TouchBase
{
    [SerializeField] private Animator anim;

    private int triggerHash;
    [SerializeField] private string TriggerParam;
    protected override Action MouseDownbehavior { get => animBehavior; }

    public override void Awake()
    {
        base.Awake();
        triggerHash = Animator.StringToHash(TriggerParam);

    }
    private void animBehavior()
    {
        if (MouseDownClip) playMouseDownClip();
        anim.SetTrigger(triggerHash);
    }
}
