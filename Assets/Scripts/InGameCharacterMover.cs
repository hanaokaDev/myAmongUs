using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

// 0x00: 크루원
// 0x01: 임포스터
// 0x02: 죽은 크루원
// 0x03: 죽은 임포스터
public enum EPlayerType
{
    Crew=0,
    Imposter=1,
    Ghost=2,

    Crew_Alive = 0,
    Imposter_Alive = 1,
    Crew_Ghost = 2,
    Imposter_Ghost = 3,

}

public class InGameCharacterMover : CharacterMover
{
    [SyncVar(hook = nameof(SetPlayerType_Hook))]
    public EPlayerType playerType;
    private void SetPlayerType_Hook(EPlayerType _, EPlayerType type)
    {
        if(isOwned && type == EPlayerType.Imposter){
            InGameUIManager.Instance.KillButtonUI.Show(this);
            playerFinder.SetKillRange((float)GameSystem.Instance.killRange + 1f);
        }
    }
    [SerializeField]
    private PlayerFinder playerFinder;

    [SyncVar]
    private float killCoolDown;
    public float KillCoolDown
    {
        get { return killCoolDown; }
    }
    public bool isKillable{
        get { return killCoolDown < 0f && playerFinder.targets.Count != 0; }
    }

    public EPlayerColor foundDeadbodyColor;


    [ClientRpc]
    public void RpcTeleport(Vector3 position)
    {
        transform.position = position;
    }
    
    public void SetNicknameColor(EPlayerType type)
    {
        if(playerType == EPlayerType.Imposter && type == EPlayerType.Imposter){
            nicknameText.color = Color.red;
        }
    }

    public void SetKillCoolDown()
    {
        if(isServer)
        {
            killCoolDown = GameSystem.Instance.killCoolDown;
        }
    }

    public override void Start()
    {
        base.Start();
        if(isOwned)
        {
            IsMovable = true;
            var myRoomPlayer = AmongUsRoomPlayer.MyRoomPlayer;
            myRoomPlayer.myCharacter = this;
            CmdSetPlayerCharacter(myRoomPlayer.nickname, myRoomPlayer.playerColor);
        }
        GameSystem.Instance.AddPlayer(this);
    }

    public void Update()
    {
        if(isServer && playerType == EPlayerType.Imposter){
            killCoolDown -= Time.deltaTime;
        }
    }

    [Command]
    void CmdSetPlayerCharacter(string nickname, EPlayerColor playerColor)
    {
        this.nickname = nickname;
        this.playerColor = playerColor;
    }

    public void Kill()
    {
        CmdKill(playerFinder.GetFirstTarget().netId);
    }

    [Command]
    public void CmdKill(uint targetNetId)
    {
        InGameCharacterMover target = null;
        foreach(var player in GameSystem.Instance.players)
        {
            if(player.netId == targetNetId)
            {
                target = player;
                break;
            }
        }
        if(target != null)
        {
            RpcTeleport(target.transform.position); // 킬대상 위치로 임포스터가 순간이동해야 함.
            target.Dead(playerColor); // 킬대상 죽이는 기능은 임포스터가 호출
            killCoolDown = GameSystem.Instance.killCoolDown; 
        }

    }
    public void Dead(EPlayerColor imposterColor) 
    {
        playerType |= EPlayerType.Ghost;
        RpcDead(imposterColor, playerColor); // 죽는 애니메이션 띄우기는 크루원측에서 호출
        var manager = NetworkRoomManager.singleton as AmongUsRoomManager;
        var deadbody = Instantiate(manager.spawnPrefabs[1], transform.position, transform.rotation).GetComponent<DeadBody>();
        NetworkServer.Spawn(deadbody.gameObject);
        deadbody.RpcSetColor(playerColor);
    }

    [ClientRpc]
    private void RpcDead(EPlayerColor imposterColor, EPlayerColor crewColor) // 죽는처리는 크루원이 호출
    {
        if(isOwned)
        {
            animator.SetBool("isGhost", true);
            InGameUIManager.Instance.KillUI.Open(imposterColor, crewColor);

            // 내가 죽었을때는 유령플레이어들을 보이게 만들어줘야함.
            var players = GameSystem.Instance.players;
            foreach(var player in players)
            {
                if(EPlayerType.Ghost == (player.playerType & EPlayerType.Ghost)) // 나를 제외한 유령들만 보이게끔 처리.
                {
                    player.SetVisibility(true);
                }
            }
        }
        else
        {
            var myPlayer = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as InGameCharacterMover;
            if(((int)myPlayer.playerType & 0x02) != (int)EPlayerType.Ghost) // 아직 내가 죽지 않았는데 상대방이 Ghost라면, 상대방이 나에게 보여서는 안됨.
            {
                SetVisibility(false); // 상대방이 죽었으니 나에게는 보이지 않게끔 처리.
            }
        }

        var collider = GetComponent<BoxCollider2D>();
        if(collider){
            collider.enabled = false; // 죽은 크루원은 충돌처리 안되게끔 처리.
        }
    }

    public void Report()
    {
        CmdReport(foundDeadbodyColor);
    }
    [Command] // cmdReport 함수를 통해, 서버로 전달된 deadbody색을 다른 플레이어에게 전파한다.
    public void CmdReport(EPlayerColor deadbodyColor)
    {
        GameSystem.Instance.StartReportMeeting(deadbodyColor);
    }

    public void SetVisibility(bool isVisible)
    {
        if(isVisible)
        {
            var color = PlayerColor.GetColor(playerColor);
            color.a = 1f;
            spriteRenderer.material.SetColor("_PlayerColor", color);
            nicknameText.text = nickname;
        }
        else
        {
            var color = PlayerColor.GetColor(playerColor);
            color.a = 0f; // 아예 보이지 않게 알파값 0으로 설정. 이 값은 Shader(M_Crew) 를 통해서 최종출력결과물이 Alpha 0 으로 나가게끔 관여함.
            spriteRenderer.material.SetColor("_PlayerColor", color); // 죽은 크루원은 보이지 않게끔 처리.
            nicknameText.text = ""; // 닉네임도 보이지 않게 숨김처리
        }
    }


}
