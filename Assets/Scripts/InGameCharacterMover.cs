using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public enum EPlayerType
{
    Crew,
    Imposter
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
            InGameUIManager.Instance.KillUI.Open(imposterColor, crewColor);
        }
    }
}
