using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class AmongUsRoomManager : NetworkRoomManager
{
    // 서버에서 새로 접속한 클라이언트를 감지했을 때 동작하는 함수.
    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);

        Vector3 spawnPos = FindFirstObjectByType<SpawnPositions>().GetSpawnPosition(); // SpawnPositions 스크립트를 찾아서, GetSpawnPosition 함수를 호출하여 스폰 위치를 가져온다.

        var player = Instantiate(spawnPrefabs[0], spawnPos, Quaternion.identity); // AmongUsPlayer 프리팹을 인스턴스화. 1번째와 2번째 매개변수는 optional함.
        // 클라이언트들에게 이 게임 오브젝트가 소환되었음을 알려주고, 
        // 두 번째 매개변수를 통해 방금 서버에 접속한 플레이어의 정보를 담고 있는 NetworkConnection을 전달하여, 
        // 이 오브젝트가 새로 접속한 플레이어의 소유임을 알려준다.
        NetworkServer.Spawn(player, conn); 
    }
}
