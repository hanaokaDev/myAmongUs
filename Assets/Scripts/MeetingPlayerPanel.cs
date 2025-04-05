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
        if((myCharacter.playerType & EPlayerType.Ghost) != EPlayerType.Ghost){
            // 지금 Panel의 player가 생존했을때만 투표대상이 된다
            InGameUIManager.Instance.MeetingUI.SelectPlayerPanel(); // 기존 O/X 창은 싹다 없애고
            voteButtons.SetActive(true); // 지금 클릭한 O/X 창만 활성화한다.
        }
    }

    public void Unselect()
    {
        voteButtons.SetActive(false);
    }
}
