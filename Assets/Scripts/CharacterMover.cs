using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterMover : NetworkBehaviour
{
    private Animator animator;

    public bool isMovable;
    [SyncVar] // Network로 동기화되도록
    public float speed = 2f;

    void Start()
    {
        animator = GetComponent<Animator>();
        if(isOwned){
            Camera cam = Camera.main;
            cam.transform.SetParent(transform);
            cam.transform.localPosition = new Vector3(0f, 0f, -10f);
            cam.orthographicSize = 2.5f; // 카메라 줌인
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    public void Move(){
        if(isOwned && isMovable){
            bool isMove = false;
            if(PlayerSettings.controlType == EControlType.KeyboardMouse){
                Vector3 dir = Vector3.ClampMagnitude(
                    new Vector3(
                        Input.GetAxis("Horizontal"), 
                        Input.GetAxis("Vertical"),
                        0f
                    ), 
                    1f
                ); 

                if(dir.x < 0f) transform.localScale = new Vector3(-0.5f, 0.5f, 1f); // 좌우반전
                else if(dir.x > 0f) transform.localScale = new Vector3(0.5f, 0.5f, 1f); 
                transform.position += dir * speed * Time.deltaTime;
                isMove = dir.magnitude > 0f;
            }
            else{
                if(Input.GetMouseButton(0)){
                    Vector3 dir = (Input.mousePosition - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f)).normalized;
                    if(dir.x < 0f) transform.localScale = new Vector3(-0.5f, 0.5f, 1f); // 좌우반전
                    else if(dir.x > 0f) transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                    transform.position += dir * speed * Time.deltaTime;
                    isMove = dir.magnitude > 0f;
                }
            }
            animator.SetBool("isMove", isMove);
        }
    }
}
