using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RigidbodyBehavior : TouchBase
{
    
    public enum RigidBehaviors
    {
        appleFalling,
        hatFlying,
        spiderDynamic
    }
    [SerializeField] private Rigidbody2D rb;
    public RigidBehaviors rigidBehavior;



    protected override Action MouseDownbehavior { get => HandleMouseClick; }

    private void HandleMouseClick()
    {
        if (MouseDownClip) playMouseDownClip();

        switch (rigidBehavior)
        {
            case RigidBehaviors.appleFalling:
                appleFallBehavior();
                break;
            case RigidBehaviors.hatFlying:
                flyHatBehavior();
                break;
            case RigidBehaviors.spiderDynamic:
                SeTdynamiclBehavior();
                break;

        }
    }

    private void appleFallBehavior() // APPLE FALLING
    {
        // rb.gameObject.GetComponent<Collider2D>().enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 5;
            rb.AddForce(new Vector2(-5f, 0f), ForceMode2D.Impulse);
    }

    private void SeTdynamiclBehavior() // APPLE FALLING
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
    private void flyHatBehavior()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 5f;
        rb.AddForce(new Vector2(9000f, 2500f));
    }

}
