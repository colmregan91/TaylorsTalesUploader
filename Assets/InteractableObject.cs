using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InteractableObject : MonoBehaviour
{
    [Header("Components")]
    public bool Rigidbody;
    public bool SpriteRenderer;
    public bool CircleCollider;



    private void OnValidate()
    {
        if (Rigidbody) gameObject.AddComponent<Rigidbody2D>();

        if (SpriteRenderer) gameObject.AddComponent<SpriteRenderer>();

        if (CircleCollider) gameObject.AddComponent<Rigidbody2D>();


    }


}
