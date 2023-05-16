using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GrowFlower : MonoBehaviour, Iinteractable
{
    private Interact interactObj;
    public Animator[] flowerArr;

    private void Awake()
    {
        flowerArr = GetComponentsInChildren<Animator>(true);
    }

    private void OnEnable()
    {
        interactObj = transform.parent.GetComponent<Interact>();
        interactObj.AddInteraction(InteractionBehavior);
    }

    public void InteractionBehavior(PointerEventData eventData)
    {
        //GameObject flower = flowerArr[0].gameObject;
        //flower.transform.position = eventData.position;
        //flower.transform.localScale = Vector3.one / (eventData.position.y / 200);
        var mouse = eventData.position;
      var   pos = Camera.main.ScreenToWorldPoint(mouse);
        flowerArr[0].transform.position = pos;
        flowerArr[0].transform.localScale = Vector3.one / (mouse.y / 200);
        flowerArr[0].gameObject.SetActive(true);

 
    }

    private void OnDisable()
    {
        interactObj.RemoveInteraction(InteractionBehavior);
    }
}
