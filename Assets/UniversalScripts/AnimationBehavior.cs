
using System;
using UnityEngine;

public class AnimationBehavior : TouchBase
{
    [Tooltip("leave blank if multiple anims")]
    [SerializeField] private Animator anim;


    [Tooltip("leave blank if single anims")]
    [SerializeField] private Animator[] anims;

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

        if (anim != null)
        {
            anim.SetTrigger(triggerHash);
        }
        else if (anims != null)
        {
            foreach (Animator an in anims)
            {
                an.SetTrigger(triggerHash);
            }
        }

    }
}
