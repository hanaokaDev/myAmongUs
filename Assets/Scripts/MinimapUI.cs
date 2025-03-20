using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{
    [SerializeField] private Transform left;
    [SerializeField] private Transform right;
    [SerializeField] private Transform top;
    [SerializeField] private Transform bottom;
    [SerializeField] private Image minimapImage;
    [SerializeField] private Image minimapPlayerImage;
    private CharacterMover targetPlayer;

    void Start()
    {
        var inst = Instantiate(minimapImage.material);
        minimapImage.material = inst; // 미니맵 이미지 복사
        targetPlayer = AmongUsRoomPlayer.MyRoomPlayer.myCharacter;
    }
    
    void Update()
    {
        if(targetPlayer != null){
            float mapWidth = Vector3.Distance(left.position, right.position);
            float mapHeight = Vector3.Distance(bottom.position, top.position);

            Vector3 playerPosX = new Vector3(targetPlayer.transform.position.x, 0f, 0f);
            Vector3 playerPosY = new Vector3(0f, targetPlayer.transform.position.y, 0f);
            float minimapPosX = Vector3.Distance(left.position, playerPosX);
            float minimapPosY = Vector3.Distance(bottom.position, playerPosY);

            Vector2 normalizedPlayerPos = new Vector2(minimapPosX / mapWidth, minimapPosY / mapHeight);

            float scaledMinimapPosX = normalizedPlayerPos.x * minimapImage.rectTransform.sizeDelta.x;
            float scaledMinimapPosY = normalizedPlayerPos.y * minimapImage.rectTransform.sizeDelta.y;

            minimapPlayerImage.rectTransform.anchoredPosition = new Vector2(scaledMinimapPosX, scaledMinimapPosY);
        }
    }
    

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}

