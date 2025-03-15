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
    public List<InGameCharacterMover> players = new List<InGameCharacterMover>(); 
    // 디버깅위해서 public으로 변경함.

    public void AddPlayer(InGameCharacterMover player)
    {
        if(!players.Contains(player)){
            players.Add(player);
            Debug.Log("AddPlayer: " + player.nickname);
            Debug.Log("AddPlayer: players.Count: " + players.Count);
        }
    }

    private IEnumerator GameReady() // server에서만 호출해야 함.
    {
        Debug.Log("GameReady Called");
        var manager = NetworkManager.singleton as AmongUsRoomManager;
        while(manager.roomSlots.Count != players.Count)
        {
            Debug.Log("GameReady: Error - Waiting for all players to be ready");
            Debug.Log("GameReady: roomSlots.Count: " + manager.roomSlots.Count);
            Debug.Log("GameReady: players.Count: " + players.Count);
            yield return null;
        }
        Debug.Log("GameReady: Success - All players are ready");
        Debug.Log("GameReady: roomSlots.Count: " + manager.roomSlots.Count);
        Debug.Log("GameReady: players.Count: " + players.Count);

        Debug.Log("GameReady: Allocating Imposters");
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
        Debug.Log("GameReady: Done Allocating Imposters");
        yield return new WaitForSeconds(1f);
        Debug.Log("GameReady: Done WaitForSeconds(1f)");
        RpcStartGame();
    }
        
    // GameReady에서 Client도 실행해야하는부분을 여기로 뺌.
    [ClientRpc] // Client에게 실행하라고 명령함.
    private void RpcStartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }
    private IEnumerator StartGameCoroutine()
    {
        yield return StartCoroutine(InGameUIManager.Instance.InGameIntroUI.ShowIntroSequence());
        yield return new WaitForSeconds(3f);
        InGameUIManager.Instance.InGameIntroUI.Close();
    }


    public List<InGameCharacterMover> GetPlayerList()
    {
        return players;
    }

    void Start()
    {
        if(isServer){
            StartCoroutine(GameReady());
        }
    }


    private void Awake()
    {
        Instance = this;
    }
}
