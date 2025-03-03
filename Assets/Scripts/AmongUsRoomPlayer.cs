using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AmongUsRoomPlayer : NetworkRoomPlayer
{
    private static AmongUsRoomPlayer myRoomPlayer;
    public static AmongUsRoomPlayer MyRoomPlayer
    {
        get
        {
            if(myRoomPlayer == null){
                var players = FindObjectsOfType<AmongUsRoomPlayer>();
                foreach(var player in players){
                    if(player.isOwned){
                        myRoomPlayer = player;
                    }
                }
            }
            return myRoomPlayer;
        }
    }

    [SyncVar(hook = nameof(SetPlayerColor_Hook))]
    public EPlayerColor playerColor;
    public void SetPlayerColor_Hook(EPlayerColor oldColor, EPlayerColor newColor)
    {
        LobbyUIManager.Instance.CustomizeUI.UpdateColorButton();
    }

    public CharacterMover lobbyPlayerCharacter;

    public void Start()
    {
        base.Start();
        if(isServer){
            SpawnLobbyPlayerCharacter();
        }

    }

    // public override void OnStartClient()
    // {
    //     base.OnStartClient();
    //     Debug.Log("OnStartClient Called!");
    //     if(isLocalPlayer){
    //         Debug.Log("isLocalPlayer");
    //         PlayerSettings.controlType = EControlType.KeyboardMouse;
    //         PlayerSettings.nickname = "Player" + Random.Range(0, 1000);
    //     }
    // }


    // Command: 미러 API에서 제공. 클라이언트에서 서버로 명령을 보낼 때 사용
    // 클라이언트에서 함수를 호출하면, 함수 내부의 동작이 서버에서 실행되게 만들어준다.
    // 함수의 이름은 Cmd로 시작해야 한다.
    [Command] 
    public void CmdSetPlayerColor(EPlayerColor color)
    {
        playerColor = color;
        lobbyPlayerCharacter.playerColor = color; // 자기자신을 굳이 찾지 말고, netId를 통해 기존에 저장해놓은 스스로를 바꾼다.
    }


    public void SpawnLobbyPlayerCharacter()
    {
        var roomSlots = (NetworkManager.singleton as AmongUsRoomManager).roomSlots;
        EPlayerColor color = EPlayerColor.Red;
        Debug.Log("roomSlots.Count: " + roomSlots.Count);
        for(int i=0; i<(int)EPlayerColor.Lime + 1; i++){
            bool isFindSameColor = false;
            foreach(var roomPlayer in roomSlots){
                var amongUsRoomPlayer = roomPlayer as AmongUsRoomPlayer;
                if(amongUsRoomPlayer.playerColor == (EPlayerColor)i && roomPlayer.netId != netId){
                    isFindSameColor = true;
                    break;
                }
            }

            if(!isFindSameColor){
                color = (EPlayerColor)i;
                break;
            }
        }
        playerColor = color;

        Vector3 spawnPos = FindFirstObjectByType<SpawnPositions>().GetSpawnPosition();

        var playerCharacter = Instantiate(AmongUsRoomManager.singleton.spawnPrefabs[0], spawnPos, Quaternion.identity).GetComponent<LobbyCharacterMover>();
        NetworkServer.Spawn(playerCharacter.gameObject, connectionToClient); 
        playerCharacter.ownerNetId = netId;
        playerCharacter.playerColor = playerColor;
    }
}
