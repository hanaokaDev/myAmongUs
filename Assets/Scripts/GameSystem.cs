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

        AllocatePlayerToAroundTable(players.ToArray()); // 플레이어 스폰 시 원형으로 배치되도록 삼각함수 사용        

        yield return new WaitForSeconds(1f);
        RpcStartGame();

        foreach(var player in players)
        {
            player.SetKillCoolDown();
        }
    }
    private void AllocatePlayerToAroundTable(InGameCharacterMover[] players)
    {
        for(int i=0; i<players.Length; i++)
        {
            float radian = 2 * Mathf.PI * i / players.Length;
            float x = Mathf.Cos(radian) * spawnDistance;
            float y = Mathf.Sin(radian) * spawnDistance;
            Vector3 newPosition = spawnTransform.position + new Vector3(x, y, 0);
            players[i].RpcTeleport(newPosition);
        } // 캐릭터 위치 동기화권한은 각 클라이언트에게 있기 때문에, transform.position을 여기서 직접 수정하면 위치가 제대로 수정되지 않는다. 따라서 InGameCharacterMover 스크립트에서 RpcTeleport 함수를 만들고 서버에서 클라이언트에게 하여금 스스로 transform.position을 수정하도록 해야 한다.

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
        yield return new WaitForSeconds(3f); // 투표가 끝나고, 3초뒤에 결과를 보여줌.

        StartCoroutine(CalculateVoteResult_Coroutine(players)); // 투표결과를 계산함.
    }

    private class CharacterVoteComparer: IComparer // 배열을 빠르게 정렬하기 위해 IComparer를 상속구현함.
    {
        public int Compare(object x, object y)
        {
            InGameCharacterMover xPlayer = (InGameCharacterMover)x;
            InGameCharacterMover yPlayer = (InGameCharacterMover)y;
            return xPlayer.voteCount <= yPlayer.voteCount ? 1 : -1;
        }
    }

    private IEnumerator CalculateVoteResult_Coroutine(InGameCharacterMover[] players)
    {
        System.Array.Sort(players, new CharacterVoteComparer());
        int remainImposterCount=0;
        foreach(var player in players)
        {
            if((player.playerType & EPlayerType.Imposter_Alive) == EPlayerType.Imposter_Alive)
            {
                remainImposterCount++;
            }
        }

        if(skipVotePlayerCount >= players[0].voteCount)
        { // 기권한 플레이어가 최다득표자보다 더 많으면, 아무도 퇴출되지않음.
            RpcOpenEjectionUI(false, EPlayerColor.White, false, remainImposterCount);
        }
        else if(players[0].voteCount == 0)
        { // 투표를 한사람이 아무도 없으면, 아무도 퇴출되지않음.
            RpcOpenEjectionUI(false, EPlayerColor.White, false, remainImposterCount);
        }
        else if(players[0].voteCount == players[1].voteCount)
        { // 투표수가 같으면, 아무도 퇴출되지않음.
            RpcOpenEjectionUI(false, EPlayerColor.White, false, remainImposterCount);
        }
        else
        { // 투표수가 다르면, 최다득표자 퇴출.
            bool isImposter = (players[0].playerType & EPlayerType.Imposter) == EPlayerType.Imposter;
            RpcOpenEjectionUI(true, players[0].playerColor, isImposter, isImposter ? remainImposterCount - 1 : remainImposterCount);

            players[0].Dead(true, EPlayerColor.White); // 투표로 죽었을 경우, 임의로 죽인 플레이어 색깔을 흰색으로 설정하였음.
        }

        var deadbodies = FindObjectsOfType<DeadBody>();
        for(int i=0; i<deadbodies.Length; i++)
        {
            Destroy(deadbodies[i].gameObject); // 시체를 없애버림.
        }
        AllocatePlayerToAroundTable(players); // 투표가 끝나면, 다시 원형으로 배치함.

        yield return new WaitForSeconds(10f); // 10초동안 결과를 보여줌.
        RpcCloseEjectionUI(); // 결과를 보여준 후, UI를 닫음.
    }

    [ClientRpc]
    public void RpcCloseEjectionUI()
    {
        InGameUIManager.Instance.EjectionUI.Close();
        AmongUsRoomPlayer.MyRoomPlayer.myCharacter.IsMovable = true; // 투표가 끝나면, 다시 움직일 수 있도록 해줌.
    }

    [ClientRpc]
    public void RpcOpenEjectionUI(
        bool isEjection, 
        EPlayerColor ejectionPlayerColor, 
        bool isImposter,
        int remainImposterCount
    )
    {
        InGameUIManager.Instance.EjectionUI.Open(isEjection, ejectionPlayerColor, isImposter, remainImposterCount);
        InGameUIManager.Instance.MeetingUI.Close(); // 회의창을 닫음.
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
