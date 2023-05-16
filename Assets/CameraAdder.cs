using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdder : MonoBehaviour
{
    private Canvas canvas;
    private void OnValidate()
    {
        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }
        canvas.worldCamera = Camera.main;
    }
}
