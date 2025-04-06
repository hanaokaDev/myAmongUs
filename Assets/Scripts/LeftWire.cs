using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftWire : MonoBehaviour
{   
    [SerializeField]
    private RectTransform mWireBody;

    private LeftWire mSelectedWire;

    [SerializeField]
    private float offset = 15f;

    private Canvas mGameCanvas; // 해상도가 달라질때마다 달라지는 Canvas 크기로 인한 distance 변동을 보정해주기 위함.

    void Start()
    {
        mGameCanvas = FindObjectOfType<Canvas>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            
            RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.right, 1f);
            if(hit.collider != null)
            {
                var left = hit.collider.GetComponentInParent<LeftWire>();
                if(left != null)
                {
                    mSelectedWire = left;
                }
            }
            else
            {
            }
        }

        // 마우스 떼면 원상복구
        if(Input.GetMouseButtonUp(0))
        {
            if(mSelectedWire != null)
            {
                mWireBody.localRotation = Quaternion.Euler(Vector3.zero);
                mWireBody.sizeDelta = new Vector2(0f, mWireBody.sizeDelta.y);
                mSelectedWire = null; // 마우스 떼면 selectedWire 를 비워준다.
            }
        }

        if(mSelectedWire != null)
        {
            float angle = Vector2.SignedAngle(transform.position + Vector3.right - transform.position, Input.mousePosition - transform.position); // 왜 transform.position 더했다 뺌?
            float distance = Vector2.Distance(mWireBody.transform.position, Input.mousePosition) - offset;
            mWireBody.localRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            mWireBody.sizeDelta = new Vector2(distance * (1/mGameCanvas.transform.localScale.x), mWireBody.sizeDelta.y);
        }
    }
}
