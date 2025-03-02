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
    }
}
