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

    [SerializeField]
    private Transform spawnTransform;

    [SerializeField]
    private float spawnDistance;
    public void AddPlayer(InGameCharacterMover player)
    {
        if(!players.Contains(player)){
            players.Add(player);
        }
    }

    [SyncVar]
    public float killCoolDown;

    [SyncVar]
    public EKillRange killRange; // killRange는 SyncVar로 선언하여, 서버에서 클라이언트에게 동기화할 수 있도록 한다.

    [SyncVar]
    public int skipVotePlayerCount;

    [SyncVar]
    public float remainTime;

    private IEnumerator GameReady() // server에서만 호출해야 함.
    {
        var manager = NetworkManager.singleton as AmongUsRoomManager;
        killCoolDown = manager.gameRuleData.killCoolDown;
        killRange = manager.gameRuleData.killRange;
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

        // 플레이어 스폰 시 원형으로 배치되도록 삼각함수 사용
        for(int i=0; i<players.Count; i++)
        {
            float radian = 2 * Mathf.PI * i / players.Count;
            float x = Mathf.Cos(radian) * spawnDistance;
            float y = Mathf.Sin(radian) * spawnDistance;
            Vector3 newPosition = spawnTransform.position + new Vector3(x, y, 0);
            players[i].RpcTeleport(newPosition);
        } // 캐릭터 위치 동기화권한은 각 클라이언트에게 있기 때문에, transform.position을 여기서 직접 수정하면 위치가 제대로 수정되지 않는다. 따라서 InGameCharacterMover 스크립트에서 RpcTeleport 함수를 만들고 서버에서 클라이언트에게 하여금 스스로 transform.position을 수정하도록 해야 한다.

        yield return new WaitForSeconds(1f);
        RpcStartGame();

        foreach(var player in players)
        {
            player.SetKillCoolDown();
        }
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
        
        // 게임시작할 때, 본인 직업에 따라서 다른 플레이어 닉 색깔을 정하는 코드
        InGameCharacterMover myCharacter = null;
        foreach(var player in players)
        {
          if(player.isOwned)
            {
                myCharacter = player;
                break;
            }
        }
        foreach(var player in players)
        {
            player.SetNicknameColor(myCharacter.playerType);
        } 

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

    public void StartReportMeeting(EPlayerColor deadbodyColor)
    {
        RpcStartReportMeeting(deadbodyColor);
        StartCoroutine(MeetingProcess_Coroutine()); // 회의시간을 정해주고, 투표시간을 정해준다.
    }
    [ClientRpc]
    public void RpcStartReportMeeting(EPlayerColor deadbodyColor)
    {
        InGameUIManager.Instance.ReportUI.Open(deadbodyColor);   
        StartCoroutine(StartMeeting_Coroutine()); // 3초동안 리포트UI 띄워준 후 닫고, 회의창으로 이동
    }

    private IEnumerator StartMeeting_Coroutine()
    {
        yield return new WaitForSeconds(3f);
        InGameUIManager.Instance.ReportUI.Close();
        InGameUIManager.Instance.MeetingUI.Open();
        InGameUIManager.Instance.MeetingUI.ChangeMeetingState(EMeetingState.Meeting);
    }

    private IEnumerator MeetingProcess_Coroutine()
    {
        var players = FindObjectsOfType<InGameCharacterMover>();
        foreach(var player in players)
        {
            player.isVote = true; // 초기화
        }

        var manager = NetworkManager.singleton as AmongUsRoomManager;
        remainTime = manager.gameRuleData.meetingsTime;
        while(true)
        {
            remainTime -= Time.deltaTime;
            yield return null; // 매 프레임마다 remainTime을 줄여줌.
            if(remainTime <= 0f){
                break;
            }
        }

        skipVotePlayerCount = 0; // 투표가 끝나면, skipVotePlayerCount를 초기화 해줘야함.
        foreach(var player in players)
        {
            if((player.playerType & EPlayerType.Ghost) != EPlayerType.Ghost)
            {
                player.isVote = false; // 투표가 끝나면, 투표를 할 수 있도록 해줘야함.
            }
            player.voteCount = 0;
        }

        RpcStartVoteTime();
        remainTime = manager.gameRuleData.voteTime;
        while(true)
        {
            remainTime -= Time.deltaTime;
            yield return null; // 매 프레임마다 remainTime을 줄여줌.
            if(remainTime <= 0f){
                break;
            }
        }

        foreach(var player in players)
        {
            // 투표가 종료되는 시점임에도 아직 투표하지 않은 플레이어가 있다면, 기권처리한다.
            if(!player.isVote && (player.playerType & EPlayerType.Ghost) != EPlayerType.Ghost)
            {
                player.isVote = true; // 기권처리
                skipVotePlayerCount += 1;
                RpcSignSkipVote(player.playerColor); // 기권처리한 플레이어를 알림.
            }
        }

        RpcEndVoteTime();

    }

    [ClientRpc]
    public void RpcStartVoteTime() // Client들에게 투표가 시작되었음을 알림.
    {
        InGameUIManager.Instance.MeetingUI.ChangeMeetingState(EMeetingState.Vote);
    }

    [ClientRpc]
    public void RpcEndVoteTime()  // Client들에게 투표가 끝났음을 알림.
    {
        InGameUIManager.Instance.MeetingUI.CompleteVote();
    }
    


    [ClientRpc]
    public void RpcSignVoteEject(EPlayerColor voterColor, EPlayerColor ejectColor)
    {
        InGameUIManager.Instance.MeetingUI.UpdateVote(voterColor, ejectColor);
    } 

    [ClientRpc]
    public void RpcSignSkipVote(EPlayerColor skipVotePlayerColor)
    {
        InGameUIManager.Instance.MeetingUI.UpdateSkipVotePlayer(skipVotePlayerColor);
    }
}
