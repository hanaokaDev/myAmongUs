using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCrew : MonoBehaviour
{    
    private SpriteRenderer spriteRenderer; // crew 의 스프라이트와 색깔 저장
    private Vector3 direction;
    private float floatingSpeed;
    private float rotateSpeed;

    public EPlayerColor playerColor;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();   
    }

    public void SetFloatingCrew(Sprite sprite, EPlayerColor playerColor, Vector3 direction, float floatingSpeed, float rotateSpeed, float size)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.material.SetColor("_PlayerColor", PlayerColor.GetColor(playerColor));
        spriteRenderer.sortingOrder = (int)Mathf.Lerp(1, 32767, size);
        this.playerColor = playerColor;
        this.direction = direction;
        this.floatingSpeed = floatingSpeed;
        this.rotateSpeed = rotateSpeed;
        transform.localScale = Vector3.one * size;
    }

    private void Update()
    {
        transform.position += direction * floatingSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, 0f, rotateSpeed));
    }

}
