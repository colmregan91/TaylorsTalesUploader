using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingInsectBehavior : MonoBehaviour
{
    [SerializeField] private Transform pos1;
    [SerializeField] private Transform pos2;

    private Vector3 targetPos;
    private float lerpTime;
    private float speed;
    Vector3 withFlipRot = new Vector3(0f, 180f, 0f);
    private bool isFlipped;
    [SerializeField] private Transform DeadSpot;
    private void Awake()
    {
        swapPositions();
        setRandomSpeed();

    }
    public Vector3 GetRandomVector3Between(Vector3 min, Vector3 max)
    {
        float posX = min.x + Random.Range(0f, 8f) * (max.x - min.x);
        float posY = min.y + Random.Range(0f, 8f) * (max.y - min.y);

        Vector3 pos = new Vector3(posX, posY, 0);

        return pos;


    }
    private void swapPositions()
    {
        targetPos = GetRandomVector3Between(pos1.position, pos2.position);

        if ((targetPos.x - transform.localPosition.x) < 0)
        {
            if (!isFlipped)
            {
                transform.eulerAngles += withFlipRot;
                isFlipped = true;
            }

        }
        else
        {
            if (isFlipped)
            {
                transform.eulerAngles -= withFlipRot;
                isFlipped = false;
            }
        }

    }
    private void setRandomSpeed()
    {
        speed = Random.Range(1f, 4f);
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * speed);

        float dist = Vector3.Distance(transform.localPosition, targetPos);
        if (dist < 100)
        {
            setRandomSpeed();

            swapPositions();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            speed = 1;
            transform.position = DeadSpot.position;
        }
            
    }
}
