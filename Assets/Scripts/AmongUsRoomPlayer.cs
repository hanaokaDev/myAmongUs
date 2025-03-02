using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AmongUsRoomPlayer : NetworkRoomPlayer
{
    [SyncVar]
    public EPlayerColor playerColor;

    public void Start()
    {
        base.Start();
        if(isServer) SpawnLobbyPlayerCharacter();
    }

    private void SpawnLobbyPlayerCharacter(){
        Debug.Log("SpawnLobbyPlayerCharacter Called!");
        var roomSlots = (NetworkManager.singleton as AmongUsRoomManager).roomSlots;
        EPlayerColor color = EPlayerColor.Red;
        // 겹치지 않는 플레이어의 색상을 정한다.
        for(int i=0;i<(int)EPlayerColor.Lime + 1; i++){
            bool isFindSameColor = false;
            int debug_index = 0;
            foreach(var roomPlayer in roomSlots){
                var amongUsRoomPlayer = roomPlayer as AmongUsRoomPlayer;
                Debug.Log(debug_index + "th" + "Player Color: " + amongUsRoomPlayer.playerColor);
                if(amongUsRoomPlayer.playerColor == (EPlayerColor)i &&
                    roomPlayer.netId != netId)
                {
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
        Debug.Log("current Player Color: " + playerColor);

        Vector3 spawnPos = FindFirstObjectByType<SpawnPositions>().GetSpawnPosition(); // SpawnPositions 스크립트를 찾아서, GetSpawnPosition 함수를 호출하여 스폰 위치를 가져온다.

        var playerCharacter = Instantiate(AmongUsRoomManager.singleton.spawnPrefabs[0], spawnPos, Quaternion.identity).GetComponent<LobbyCharacterMover>(); // AmongUsPlayer 프리팹을 인스턴스화. 1번째와 2번째 매개변수는 optional함.
        // 클라이언트들에게 이 게임 오브젝트가 소환되었음을 알려주고, 
        // 두 번째 매개변수를 통해 방금 서버에 접속한 플레이어의 정보를 담고 있는 NetworkConnection을 전달하여, 
        // 이 오브젝트가 새로 접속한 플레이어의 소유임을 알려준다.
        NetworkServer.Spawn(playerCharacter.gameObject, connectionToClient); 
        playerCharacter.playerColor = color;


        // 컴포넌트에서 게임오브젝트 접근은 .gameObject 사용.
        // 게임오브젝트에서 컴포넌트 접근은 .getComponent 사용.
    }

}
