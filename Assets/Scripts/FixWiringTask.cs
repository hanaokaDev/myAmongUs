using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixWiringTask : MonoBehaviour
{   
    [SerializeField]
    private List<LeftWire> mLeftWires;
    [SerializeField]
    private List<RightWire> mRightWires;


    [SerializeField] // serialized for debugging
    private LeftWire mSelectedLeftWire;

    int LEFT_WIRE_COUNT = 4;

    private void OnEnable()
    {
        List<int> numberPool = new List<int>();
        for(int i=0; i<LEFT_WIRE_COUNT; i++)
        {
            numberPool.Add(i);
        }

        int index=0;

        while(numberPool.Count != 0)
        {
            var number = numberPool[Random.Range(0, numberPool.Count)];
            mLeftWires[index++].SetWireColor((EWireColor)number);
            numberPool.Remove(number);
        }

        for(int i=0; i<LEFT_WIRE_COUNT; i++)
        {
            numberPool.Add(i);
        }

        index=0;
        while(numberPool.Count != 0)
        {
            var number = numberPool[Random.Range(0, numberPool.Count)];
            mRightWires[index++].SetWireColor((EWireColor)number);
            numberPool.Remove(number);
        }
    }
    
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Down");
            RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.right, 1f);
            if(hit.collider != null)
            {
                var left = hit.collider.GetComponentInParent<LeftWire>();
                if(left != null)
                {
                    mSelectedLeftWire = left;
                }
            }
            else
            {
            }
        }

        // 마우스 떼면 원상복구
        if(Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse Up");
            if(mSelectedLeftWire != null)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(Input.mousePosition, Vector2.right, 1f);
                foreach(var hit in hits) // rightWire 찾기
                {
                    if(hit.collider != null)
                    {
                        var right = hit.collider.GetComponentInParent<RightWire>();
                        if(right != null)
                        { // left <-> right를 연결시키기
                            Debug.Log("001 LeftWire: " + mSelectedLeftWire.WireColor + " RightWire: " + right.WireColor);
                            mSelectedLeftWire.SetTarget(hit.transform.position, 40f);
                            Debug.Log("002 SetTarget: " + hit.transform.position);
                            mSelectedLeftWire.ConnectWire(right);
                            Debug.Log("003 ConnectWire: " + right.WireColor);
                            right.ConnectWire(mSelectedLeftWire);
                            Debug.Log("004 ConnectWire: " + mSelectedLeftWire.WireColor);
                            mSelectedLeftWire = null;
                            return;
                        }
                    }
                }
                // right 못찾으면 원상복구한다.
                mSelectedLeftWire.ResetTarget();
                mSelectedLeftWire.DisconnectWire(); // 연결된 전선이 없을때만 불빛을 끈다.
                mSelectedLeftWire = null; // 마우스 떼면 selectedWire 를 비워준다.
            }
        }

        if(mSelectedLeftWire != null) // 누르고있는동안은 계속 LeftWire가 마우스를 따라다니게 구현.
        {
            mSelectedLeftWire.SetTarget(Input.mousePosition, 15f);
        }
    }

}

public enum EWireColor
{
    None = -1,
    Red = 0,
    Blue,
    Green,
    Yellow,
    Magneta,
}