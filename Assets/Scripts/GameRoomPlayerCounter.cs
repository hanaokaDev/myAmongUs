using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class GameRoomPlayerCounter : NetworkBehaviour
{
    [SyncVar]
    public int minPlayer;
    [SyncVar]
    public int maxPlayer;

    [SerializeField]
    private Text playerCountText;
    public void UpdatePlayerCount()
    {
        var manager = NetworkManager.singleton as AmongUsRoomManager;
        var players = FindObjectsOfType<AmongUsRoomPlayer>();

        bool isStartable = players.Length >= minPlayer;
        playerCountText.color = isStartable ? Color.white : Color.red;

        playerCountText.text = string.Format("{0}/{1}", players.Length, maxPlayer); // 기존에는 manager.maxConnections가 Client들에게 제대로 공유되지않는 문제가 있었음. 그것은 원래 소속이었던 LobbyUIManager.cs에서 NetworkBehaviour를 상속받지 않고, MonoBehaviour만 상속받아서 발생한 문제임.
    }

    void Start()
    {
        if(isServer)
        {
            var manager = NetworkManager.singleton as AmongUsRoomManager;
            minPlayer = manager.minPlayerCount;
            maxPlayer = manager.maxConnections;
        }
    }


}
