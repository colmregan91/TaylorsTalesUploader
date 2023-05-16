using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public Action ActionBehavior;

    public void SubscribeBehavior(Action behavior)
    {
        ActionBehavior += behavior;
    }

    public void UnsubscribeBehavior(Action behavior)
    {
        ActionBehavior -= behavior;
    }


    private void OnMouseDown()
    {
        ActionBehavior?.Invoke();
    }
}
