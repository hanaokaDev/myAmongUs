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
    public void SetPlayerColor_Hook(EPlayerColor oldColor, EPlayerColor newColor) // Client에서 호출되는 함수
    {
        LobbyUIManager.Instance.CustomizeUI.UpdateColorButtonValid(oldColor); // 클라이언트들에게 색변경 통보하는 부분
        LobbyUIManager.Instance.CustomizeUI.UpdateColorButtonInvalid(newColor); // 클라이언트들에게 색변경 통보하는 부분
    }

    public CharacterMover lobbyPlayerCharacter;

    public void Start()
    {
        base.Start();
        if(isServer){ 
            SpawnLobbyPlayerCharacter();
        }

    }

    private void OnDestroy()
    {
        if(LobbyUIManager.Instance != null){
            LobbyUIManager.Instance.CustomizeUI.UpdateColorButtonValid(playerColor);
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


    // 새로운 플레이어가 접속했을 때, 서버역할 호스트에서 LobbyPlayerCharacter를 생성하는 단계.
    // 그러면서 플레이어의 기본색상을 정하고, 
    // 다른 클라이언트에서는 SetPlayerColor_Hook 함수를 통해 통보받아서 UI를 업데이트한다.
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
        playerColor = color; // playerColor가 바뀌었으니 hook함수가 호출된다.

        Vector3 spawnPos = FindFirstObjectByType<SpawnPositions>().GetSpawnPosition();


        var playerCharacter = Instantiate(AmongUsRoomManager.singleton.spawnPrefabs[0], spawnPos, Quaternion.identity).GetComponent<LobbyCharacterMover>();
        playerCharacter.transform.localScale = index < 5 ? new Vector3(0.5f, 0.5f, 1f) : new Vector3(-0.5f, 0.5f, 1f); // 5번째부터의 의자에서 스폰되었다면 좌우반전해줌.
        NetworkServer.Spawn(playerCharacter.gameObject, connectionToClient); 
        playerCharacter.ownerNetId = netId;
        playerCharacter.playerColor = playerColor;
    }
}
