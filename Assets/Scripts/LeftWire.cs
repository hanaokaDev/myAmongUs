using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftWire : MonoBehaviour
{   
    public EWireColor WireColor { get; private set; } // 왼쪽 와이어 색상

    public bool IsConnected{get; private set;}

    [SerializeField]
    private Image mLightImage;
    [SerializeField]
    private RightWire mConnectedWire; // 연결된 오른쪽 와이어

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

    public void ConnectWire(RightWire rightWire) // 색이 같은 두 전선이 연결되면 빛을 내게 한다.
    {
        if(mConnectedWire != null && mConnectedWire != rightWire)
        {
            mConnectedWire.DisconnectWire(this);
            mConnectedWire = null; // 연결된 전선이 없을때만 불빛을 끈다.
        }
        mConnectedWire = rightWire;
        if(mConnectedWire.WireColor == WireColor)
        {
            mLightImage.color = Color.yellow; // 연결되면 불빛이 켜진다.
            IsConnected = true;
        }
    }
    public void DisconnectWire() // 색이 다른 전선이 연결되면 빛을 끈다.
    {
        if(mConnectedWire != null)
        {
            mConnectedWire.DisconnectWire(this); // 연결된 오른쪽 전선과의 연결을 끊는다.
            mConnectedWire = null;
        }
        mLightImage.color = Color.gray; // 연결이 끊어지면 불빛이 꺼진다.
        IsConnected = false;
    }
}
