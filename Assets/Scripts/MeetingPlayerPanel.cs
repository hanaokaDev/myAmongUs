using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeetingPlayerPanel : MonoBehaviour
{
    [SerializeField]
    private Image characterImage;

    [SerializeField]
    private Text nicknameText;

    [SerializeField]
    private GameObject deadPlayerBlock;

    [SerializeField]
    private GameObject reportSign;

    [SerializeField]
    private GameObject voteButtons;

    [HideInInspector]
    public InGameCharacterMover targetPlayer;

    [SerializeField]
    private GameObject voteSign;
    [SerializeField]
    private GameObject voterPrefab;
    [SerializeField]
    private Transform voterParentTransform;

    public void UpdatePanel(EPlayerColor voterColor)
    {
        var voter = Instantiate(voterPrefab, voterParentTransform).GetComponent<Image>();
        voter.material = Instantiate(voter.material);
        voter.material.SetColor("_PlayerColor", PlayerColor.GetColor(voterColor));
        voterParentTransform.gameObject.SetActive(true);
    }

    public void UpdateVoteSign(bool isVoted)
    {
        voteSign.SetActive(isVoted);
    }

    public void SetPlayer(InGameCharacterMover target)
    {
        Material inst = Instantiate(characterImage.material);
        characterImage.material = inst;
        targetPlayer = target;

        characterImage.material.SetColor("_PlayerColor", PlayerColor.GetColor(targetPlayer.playerColor));
        nicknameText.text = targetPlayer.nickname;

        var myCharacter = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as InGameCharacterMover;
        if(((myCharacter.playerType & EPlayerType.Imposter) == EPlayerType.Imposter)
        && ((targetPlayer.playerType & EPlayerType.Imposter) == EPlayerType.Imposter))
        {
            // 임포스터끼리는 닉네임 색깔을 빨간색으로 표시
            nicknameText.color = Color.red;
        }
        
        bool isDead = (targetPlayer.playerType & EPlayerType.Ghost) == EPlayerType.Ghost;
        deadPlayerBlock.SetActive(isDead);
        GetComponent<Button>().interactable = !isDead; // 자신이 죽은 유령플레이어라면, MeetingPlayerPanel에서 클릭기능자체를 제거함.
        reportSign.SetActive(targetPlayer.isReporter);

    }

    public void OnClickPlayerPanel()
    {
        var myCharacter = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as InGameCharacterMover;
        if(myCharacter.isVote) return; // 이미 투표를 한 상태라면, 더이상 투표를 할 수 없다.


        if((myCharacter.playerType & EPlayerType.Ghost) != EPlayerType.Ghost){
            // 지금 Panel의 player가 생존했을때만 투표대상이 된다
            InGameUIManager.Instance.MeetingUI.SelectPlayerPanel(); // 기존 O/X 창은 싹다 없애고
            voteButtons.SetActive(true); // 지금 클릭한 O/X 창만 활성화한다.
        }
    }

    public void Select()
    {
        var myCharacter = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as InGameCharacterMover;
        myCharacter.CmdVoteEjectPlayer(targetPlayer.playerColor); // targetPlayer를 클릭하였음을 알린다.
        Unselect();
    }

    public void Unselect()
    {
        voteButtons.SetActive(false);
    }
}
