using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameRuleItem : MonoBehaviour // Client 가 열었다면 규칙수정을 불가하게 해줌.
{
    [SerializeField]
    private GameObject inactiveObject;

    void Start()
    {
        if(!AmongUsRoomPlayer.MyRoomPlayer.isServer){
            inactiveObject.SetActive(false);
        }
    }
}
