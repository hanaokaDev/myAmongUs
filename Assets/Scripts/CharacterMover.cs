using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterMover : NetworkBehaviour
{
    private Animator animator;

    private bool isMovable; // property로 바꿈. 의도는, isMovable이 false가 되면 animator도 꺼버리게 하기 위함.
    public bool IsMovable
    {
        get{
            return isMovable;
        }
        set{
            if(!value){ // 받아온 value(=할당된새로운값)가 false일때만 animator의 isMove를 덩달아 false로 변경.
                animator.SetBool("isMove", false);
            }
            isMovable = value;
        }
    }
    [SyncVar] // Network로 동기화되도록
    public float speed = 2f;

    private SpriteRenderer spriteRenderer;

    [SyncVar(hook = nameof(SetPlayerColor_Hook))]
    public EPlayerColor playerColor;
    public void SetPlayerColor_Hook(EPlayerColor oldColor, EPlayerColor newColor){
        if(spriteRenderer == null)
        {
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
