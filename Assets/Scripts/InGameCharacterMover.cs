using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public enum EPlayerType
{
    Crew,
    Imposter
}

public class InGameCharacterMover : CharacterMover
{
    [SyncVar(hook = nameof(SetPlayerType_Hook))]
    public EPlayerType playerType;
    private void SetPlayerType_Hook(EPlayerType _, EPlayerType type)
    {
        if(isOwned && type == EPlayerType.Imposter){
            InGameUIManager.Instance.KillButtonUI.Show(this);
        }
    }

    [SyncVar]
    private float killCoolDown;
    public float KillCoolDown
    {
        get { return killCoolDown; }
    }
    public bool isKillable{
        get { return killCoolDown < 0f; }
    }


    [ClientRpc]
    public void RpcTeleport(Vector3 position)
    {
        transform.position = position;
    }
    
    public void SetNicknameColor(EPlayerType type)
    {
        if(playerType == EPlayerType.Imposter && type == EPlayerType.Imposter){
            nicknameText.color = Color.red;
        }
    }

    public void SetKillCoolDown()
    {
        if(isServer)
        {
            killCoolDown = GameSystem.Instance.killCoolDown;
        }
    }

    public override void Start()
    {
        base.Start();
        if(isOwned)
        {
            IsMovable = true;
            var myRoomPlayer = AmongUsRoomPlayer.MyRoomPlayer;
            myRoomPlayer.myCharacter = this;
            CmdSetPlayerCharacter(myRoomPlayer.nickname, myRoomPlayer.playerColor);
        }
        GameSystem.Instance.AddPlayer(this);
    }

    public void Update()
    {
        if(isServer && playerType == EPlayerType.Imposter){
            killCoolDown -= Time.deltaTime;
        }
    }


    [Command]
    void CmdSetPlayerCharacter(string nickname, EPlayerColor playerColor)
    {
        this.nickname = nickname;
        this.playerColor = playerColor;
    }
}
