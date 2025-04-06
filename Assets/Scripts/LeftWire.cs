using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftWire : MonoBehaviour
{   
    public EWireColor WireColor { get; private set; } // 왼쪽 와이어 색상
    [SerializeField]
    private List<Image> mWireImages;

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

    public void SetWireColor(EWireColor wireColor)
    {
        WireColor = wireColor;
        Color color = Color.black;
        switch(WireColor)
        {
            case EWireColor.Red:
                color = Color.red;
                break;
            case EWireColor.Blue:
                color = Color.blue;
                break;
            case EWireColor.Green:
                color = Color.green;
                break;
            case EWireColor.Yellow:
                color = Color.yellow;
                break;
            case EWireColor.Magneta:
                color = Color.magenta;
                break;
        }
        foreach(var image in mWireImages)
        {
            image.color = color;
        }
    }
}
