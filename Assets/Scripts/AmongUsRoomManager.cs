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
        var player = Instantiate(spawnPrefabs[0]); // AmongUsPlayer 프리팹을 인스턴스화.


        // 클라이언트들에게 이 게임 오브젝트가 소환되었음을 알려주고, 
        // 두 번째 매개변수를 통해 방금 서버에 접속한 플레이어의 정보를 담고 있는 NetworkConnection을 전달하여, 
        // 이 오브젝트가 새로 접속한 플레이어의 소유임을 알려준다.
        NetworkServer.Spawn(player, conn); 
    }
}
