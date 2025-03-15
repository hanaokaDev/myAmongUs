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
    [SyncVar]
    public EPlayerType playerType;
    
    public override void Start()
    {
        base.Start();
        if(isOwned)
        {
            IsMovable = true;
            var myRoomPlayer = AmongUsRoomPlayer.MyRoomPlayer;
            CmdSetPlayerCharacter(myRoomPlayer.nickname, myRoomPlayer.playerColor);
        }
        GameSystem.Instance.AddPlayer(this);
    }



    [Command]
    void CmdSetPlayerCharacter(string nickname, EPlayerColor playerColor)
    {
        this.nickname = nickname;
        this.playerColor = playerColor;
    }
}
