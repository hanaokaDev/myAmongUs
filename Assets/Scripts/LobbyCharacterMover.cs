using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// 클라이언트측에서 사용하는 클래스.
public class LobbyCharacterMover : CharacterMover
{
    // AmongUsRoomPlayer.cs의 lobbyPlayerCharacter 를 채우기 위함.
    // SpawnLobbyPlayerCharacter 함수에서 LobbyPlayer를 생성할 때 해당 멤버변수를 채울 의도.
    [SyncVar(hook = nameof(SetOwnerNetId_Hook))]
    public uint ownerNetId; 
    // ownerNetId가 변경되면 클라이언트에서 SetOwnerNetId_Hook 함수가 호출된다.
    // 이 함수에서는 해당 네트워크 아이디를 가진 플레이어를 찾아서 lobbyPlayerCharacter에 할당한다.
    public void SetOwnerNetId_Hook(uint _, uint newOwnerId){
        var players = FindObjectsOfType<AmongUsRoomPlayer>();
        foreach(var player in players){
            if(player.netId == newOwnerId){
                player.lobbyPlayerCharacter = this;
                break;
            }
        }
    }

    public void CompleteSpawn(){
        Debug.Log("CompleteSpawn Called!");
        if(isOwned){
            IsMovable = true;
        }
    }
}
