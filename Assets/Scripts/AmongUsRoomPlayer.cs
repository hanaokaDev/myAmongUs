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
        Vector3 spawnPos = FindFirstObjectByType<SpawnPositions>().GetSpawnPosition();

        var player = Instantiate(AmongUsRoomManager.singleton.spawnPrefabs[0], spawnPos, Quaternion.identity);
        NetworkServer.Spawn(player, connectionToClient); 
    }
}
