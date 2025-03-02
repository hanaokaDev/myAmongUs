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

    private SpriteRenderer spriteRenderer;



    // hook을 통해, syncVar로 동기화된 변수가 서버측에서 변경되었을 때
    // hook으로 등록한 함수가 클라이언트측에서 자동으로 호출되도록 만들어준다.
    [SyncVar(hook = nameof(SetPlayerColor_Hook))] 
    public EPlayerColor playerColor;
    public void SetPlayerColor_Hook(EPlayerColor oldColor, EPlayerColor newColor){
        // 클라이언트에서 동기화된 playerColor 변수가 변경되었을 때 호출되는 함수
        // 서버에서 CharacterMover의 playerColor를 변경하면, 클라이언트는 그 변경을 감지하고 오브젝트 색을 변경해야 한다.
        if(spriteRenderer == null){
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        spriteRenderer.material.SetColor("_PlayerColor", PlayerColor.GetColor(newColor));
    }
     

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material.SetColor("_PlayerColor", PlayerColor.GetColor(playerColor));
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
