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
        
        deadPlayerBlock.SetActive((targetPlayer.playerType & EPlayerType.Ghost) == EPlayerType.Ghost);
        reportSign.SetActive(targetPlayer.isReporter);

    }

    public void OnClickPlayerPanel()
    {
        voteButtons.SetActive(true);
    }
}
