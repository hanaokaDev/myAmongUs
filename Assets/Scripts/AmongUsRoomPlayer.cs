using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AmongUsRoomPlayer : NetworkRoomPlayer
{
    [SyncVar]
    public EPlayerColor playerColor;

    public void SpawnLobbyPlayerCharacter()
    {
        Vector3 spawnPos = FindFirstObjectByType<SpawnPositions>().GetSpawnPosition(); // SpawnPositions 스크립트를 찾아서, GetSpawnPosition 함수를 호출하여 스폰 위치를 가져온다.

        var player = Instantiate(AmongUsRoomManager.singleton.spawnPrefabs[0], spawnPos, Quaternion.identity); // AmongUsPlayer 프리팹을 인스턴스화. 1번째와 2번째 매개변수는 optional함.
        NetworkServer.Spawn(player, connectionToClient); 
    }
}
