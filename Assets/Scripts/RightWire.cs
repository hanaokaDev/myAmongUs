using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightWire : MonoBehaviour
{   
    public EWireColor WireColor { get; private set; } // 왼쪽 와이어 색상

    public bool IsConnected{get; private set;}

    [SerializeField]
    private List<Image> mWireImages;

    [SerializeField]
    private Image mLightImage;

    [SerializeField] // serialized for debugging
    private List<LeftWire> mConnectedWires; // 연결된 왼쪽 와이어들
    
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

    
    // 우측전선은 여러개의 좌측전선이 연결될 수 있다.
    public void ConnectWire(LeftWire leftWire) // 색이 같은 두 전선이 연결되면 빛을 내게 한다.
    {
        if(mConnectedWires.Contains(leftWire))
        {
            Debug.Log("Already connected wire.");
            return;
        }
        Debug.Log("Connect wire: " + leftWire.name);
        mConnectedWires.Add(leftWire);
        Debug.Log("Connected wires count: " + mConnectedWires.Count);
        if(mConnectedWires.Count == 1 && leftWire.WireColor == WireColor) // 연결된 전선이 처음일때만 불빛을 켠다.
        {
            mLightImage.color = Color.yellow;
            IsConnected = true;
        }
        else{
            mLightImage.color = Color.gray; // 연결된 전선이 없을때만 불빛을 끈다.
            IsConnected = false;
        }
        Debug.Log("Done");
    }
    public void DisconnectWire(LeftWire leftWire) // 색이 다른 전선이 연결되면 빛을 끈다.
    {
        mConnectedWires.Remove(leftWire);
        if(mConnectedWires.Count == 1 && (mConnectedWires[0].WireColor == WireColor)) // 연결된 전선이 없을때만 불빛을 끈다.
        {
            mLightImage.color = Color.yellow;
            IsConnected = true;
        }
        else
        {
            mLightImage.color = Color.gray; // 연결된 전선이 없을때만 불빛을 끈다.
            IsConnected = false;
        }
    }
}
