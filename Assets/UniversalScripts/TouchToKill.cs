using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class TouchToKill : TouchBase
{
    [SerializeField] private Animator anim;
    [SerializeField] private FlyingInsectBehavior flyingBehavior;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Rigidbody2D rb;
    private bool isFalling;

    protected override Action MouseDownbehavior => kill;

    private void kill()
    {
        if (MouseDownClip) playMouseDownClip();
        anim.speed = 0;
        flyingBehavior.SetisFlying(false);
        particles.Play(true);
        if (!isFalling)
            fallAsync();
    }

    private async void fallAsync()
    {
        if (rb == null) return;
        rb.constraints = RigidbodyConstraints2D.None;
        isFalling = true;
        await Task.Delay(700);
        if (rb == null) return;
        //      rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 6;
        await Task.Delay(3000);
        ResetMosquito();

    }

    private void ResetMosquito()
    {
        if (rb == null || this==null) return; // this check as async dont cancel on quit
        rb.gravityScale = 0;
        isFalling = false;
        transform.position = flyingBehavior.DeadSpot.position;
        anim.speed = 1;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        flyingBehavior.SetisFlying(true);
    }


}


