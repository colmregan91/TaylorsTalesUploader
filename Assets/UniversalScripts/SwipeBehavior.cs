using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SwipeBehavior : TouchBase
{
    private bool isFlying;
    private Camera cam;
    private Vector3 startPos;
    [SerializeField] private SpriteRenderer flyingObjRenderer;
    private Vector3 endPos;
    private Transform objTransform;
    private bool hasStartPos;
    [SerializeField] private float speed;
    protected override Action MouseDownbehavior => handleMouseDown;

    public override void Awake()
    {
        cam = Camera.main;
        flyingObjRenderer.Init(cam);
        objTransform = flyingObjRenderer.transform.parent;

        MouseUpAsButBehavior += handleMouseUp;
        base.Awake();


    }

    private void OnDisable()
    {
        MouseUpAsButBehavior -= handleMouseUp;
    }


    private void handleMouseDown()
    {
        if (isFlying) return;
 
        var mouse = Input.mousePosition;
        mouse.z = 100; //distance of the plane from the camera
        var screenPoint = cam.ScreenToWorldPoint(mouse);
        startPos = screenPoint;

        hasStartPos = true;
    }


    private void handleMouseUp()
    {
        if (!hasStartPos || isFlying) return;
        flyingObjRenderer.transform.localPosition = Vector3.zero;
        objTransform.position = startPos;

        var mouse = Input.mousePosition;
        mouse.z = 100; //distance of the plane from the camera
        var screenPoint = cam.ScreenToWorldPoint(mouse);
        endPos = screenPoint;
        objTransform.LookAt(endPos);

        while (flyingObjRenderer.IsVisibleFrom(cam))
        {

            flyingObjRenderer.transform.position -= objTransform.forward;
        }


        fly();
    }


    private async void fly()
    {
        if (!IsPulsating) return;
        if (MouseUpClip) playMouseUpClip();
        isFlying = true;
        flyingObjRenderer.transform.position += objTransform.forward;
        while (flyingObjRenderer.IsVisibleFrom(cam))
        {
            flyingObjRenderer.transform.position += objTransform.forward * speed * Time.deltaTime;
            await Task.Yield();
        }
        isFlying = false;
        hasStartPos = false;
    }
}
