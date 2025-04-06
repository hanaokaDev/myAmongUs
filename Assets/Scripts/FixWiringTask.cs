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

    private LeftWire mSelectedLeftWire;


    
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
                            mSelectedLeftWire.SetTarget(hit.transform.position, 40f);
                            mSelectedLeftWire = null;
                            return;
                        }
                    }
                }
                // right 못찾으면 원상복구한다.
                mSelectedLeftWire.ResetTarget();
                mSelectedLeftWire = null; // 마우스 떼면 selectedWire 를 비워준다.
            }
        }

        if(mSelectedLeftWire != null) // 누르고있는동안은 계속 LeftWire가 마우스를 따라다니게 구현.
        {
            mSelectedLeftWire.SetTarget(Input.mousePosition, 15f);
        }
    }

}
