using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftWire : MonoBehaviour
{   
    [SerializeField]
    private RectTransform mWireBody;

    [SerializeField]
    private float offset = 15f;

    private Canvas mGameCanvas; // 해상도가 달라질때마다 달라지는 Canvas 크기로 인한 distance 변동을 보정해주기 위함.

    void Start()
    {
        mGameCanvas = FindObjectOfType<Canvas>();
    }

    public void SetTarget(Vector3 targetPosition, float offset)
    {
        float angle = Vector2.SignedAngle(transform.position + Vector3.right - transform.position, targetPosition - transform.position); // 왜 transform.position 더했다 뺌?
        float distance = Vector2.Distance(mWireBody.transform.position, targetPosition) - offset;
        mWireBody.localRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        mWireBody.sizeDelta = new Vector2(distance * (1/mGameCanvas.transform.localScale.x), mWireBody.sizeDelta.y);
    }

    public void ResetTarget()
    {
        mWireBody.localRotation = Quaternion.Euler(Vector3.zero);
        mWireBody.sizeDelta = new Vector2(0f, mWireBody.sizeDelta.y);
    }

}
