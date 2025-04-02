using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CharacterMover : NetworkBehaviour
{
    protected Animator animator;

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

    protected SpriteRenderer spriteRenderer; // Ghost를 위해서, 상속클래스에서 해당 알파값 변경하기 위해 protected로 선언함.

    [SyncVar(hook = nameof(SetPlayerColor_Hook))]
    public EPlayerColor playerColor;
    public void SetPlayerColor_Hook(EPlayerColor oldColor, EPlayerColor newColor){
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        spriteRenderer.material.SetColor("_PlayerColor", PlayerColor.GetColor(newColor)); 
    }

    [SyncVar(hook = nameof(SetNickname_Hook))]
    public string nickname;
    [SerializeField]
    protected Text nicknameText; // InGameCharacterMover에서 Imposter끼리는 적색으로 표시되는 기능구현을 위해 protected로 자식클래스에 한해서만 노출하였음.
    public void SetNickname_Hook(string _, string value)
    {
        nicknameText.text = value;
    }

    [SerializeField]
    private float characterSize = 0.5f; 
    [SerializeField]
    private float cameraSize = 2.5f;

    public virtual void Start() // 재정의할수있게 virtual로 선언
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material.SetColor("_PlayerColor", PlayerColor.GetColor(playerColor));

        animator = GetComponent<Animator>();
        if(isOwned){
            Camera cam = Camera.main;
            cam.transform.SetParent(transform);
            cam.transform.localPosition = new Vector3(0f, 0f, -10f);
            cam.orthographicSize = cameraSize; // 카메라 줌인
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

                if(dir.x < 0f) transform.localScale = new Vector3(-characterSize, characterSize, 1f); // 좌우반전
                else if(dir.x > 0f) transform.localScale = new Vector3(characterSize, characterSize, 1f); 
                transform.position += dir * speed * Time.deltaTime;
                isMove = dir.magnitude > 0f;
            }
            else{
                if(Input.GetMouseButton(0)){
                    Vector3 dir = (Input.mousePosition - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f)).normalized;
                    if(dir.x < 0f) transform.localScale = new Vector3(-characterSize, characterSize, 1f); // 좌우반전
                    else if(dir.x > 0f) transform.localScale = new Vector3(characterSize, characterSize, 1f);
                    transform.position += dir * speed * Time.deltaTime;
                    isMove = dir.magnitude > 0f;
                }
            }
            animator.SetBool("isMove", isMove);
        }
        if(transform.localScale.x < 0)
        {
            nicknameText.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if(transform.localScale.x > 0){
            nicknameText.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
