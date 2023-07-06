using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowersManager : TouchBase
{

    [SerializeField] private Animator[] flowers;

  
    private Vector3 pos;
    private Camera cam;
    private Queue<Animator> flowerQueue = new Queue<Animator>();
    private float sizeMultiplier = 110;
    private Queue<Animator> offQueue = new Queue<Animator>();
    private float clickTime;

    protected override Action MouseDownbehavior => flowerBehavior;

    private void Start()
    {
        cam = Camera.main;

        foreach (Animator gam in flowers)
        {
            flowerQueue.Enqueue(gam);
        }
    }
    private void flowerBehavior()
    {
        if (Time.time < clickTime + waitTime) return;
        if (MouseDownClip) playMouseDownClip();
        var mouse = Input.mousePosition;
        mouse.z = 100; //distance of the plane from the camera
        pos = cam.ScreenToWorldPoint(mouse);
        Animator flo = flowerQueue.Dequeue();
        flo.transform.position = pos;
        flo.transform.localScale = Vector3.one / (mouse.y / sizeMultiplier);
        flo.gameObject.SetActive(true);
        offQueue.Enqueue(flo);

        if (flowerQueue.Count <= 0)
        {
            Animator offanim = offQueue.Dequeue();
            offanim.SetTrigger("fall");
            flowerQueue.Enqueue(offanim);
        }
        clickTime = Time.time;
    }
}

