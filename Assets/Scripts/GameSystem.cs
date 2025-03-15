using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameSystem : NetworkBehaviour
{
    public static GameSystem Instance;

    // 이 클래스의 Start에서 players에 할당하려고 하면, Start 시점에 Players의 InGameCharacterMover 객체가 없을 가능성이 있음.
    // 따라서, Player가 생성될때마다 스스로 직접 이 players 객체에 값을 추가해주는식으로 구현하여야 한다.
    private List<InGameCharacterMover> players = new List<InGameCharacterMover>(); 

    public void AddPlayer(InGameCharacterMover player)
    {
        if(!players.Contains(player)){
            players.Add(player);
        }
    }

    private IEnumerator GameReady()
    {
        var manager = NetworkManager.singleton as AmongUsRoomManager;
        while(manager.roomSlots.Count != players.Count)
        {
            yield return null;
        }

        // 임포스터 할당
        for(int i=0; i<manager.imposterCount; i++)
        {
            var player = players[Random.Range(0, players.Count)];
            if(player.playerType != EPlayerType.Imposter)
            {
                player.playerType = EPlayerType.Imposter;
            }
            else{
                i--;
            }
        }
    }

    public List<InGameCharacterMover> GetPlayerList()
    {
        return players;
    }

    void Start()
    {
        StartCoroutine(GameReady());
    }


    private void Awake()
    {
        Instance = this;
    }
}
