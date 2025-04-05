using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeetingUI : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPanelPrefab;

    [SerializeField]
    private Transform playerPanelParent;

    [SerializeField]
    private GameObject voterPrefab;
    [SerializeField]
    private GameObject skipVoteButton;
    [SerializeField]
    private GameObject skipVotePlayers;
    [SerializeField]
    private Transform skipVoteParentTransform;

    [SerializeField]
    private Text meetingTimeText;

    private EMeetingState meetingState;

    private List<MeetingPlayerPanel> meetingPlayerPanels = new List<MeetingPlayerPanel>();

    public void ChangeMeetingState(EMeetingState nextState)
    {
        meetingState = nextState;
    }

    public void Open()
    {
        var myCharacter = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as InGameCharacterMover;
        var myPanel = Instantiate(playerPanelPrefab, playerPanelParent).GetComponent<MeetingPlayerPanel>();
        myPanel.SetPlayer(myCharacter);
        meetingPlayerPanels.Add(myPanel);

        gameObject.SetActive(true);

        var players = FindObjectsOfType<InGameCharacterMover>();
        foreach (var player in players)
        {
            if(player != myCharacter)
            {
                var panel = Instantiate(playerPanelPrefab, playerPanelParent).GetComponent<MeetingPlayerPanel>();
                panel.SetPlayer(player);
                meetingPlayerPanels.Add(panel);
            }
        }

    }

    public void SelectPlayerPanel()
    {
        foreach(var panel in meetingPlayerPanels)
        {
            panel.Unselect();
        }
    }

    public void UpdateVote(EPlayerColor voterColor, EPlayerColor ejectColor)
    {
        foreach(var panel in meetingPlayerPanels)
        {
            if(panel.targetPlayer.playerColor == ejectColor)
            {
                panel.UpdatePanel(voterColor);
            }

            if(panel.targetPlayer.playerColor == voterColor)
            {
                panel.UpdateVoteSign(true);
            }
        }
    }
    public void UpdateSkipVotePlayer(EPlayerColor skipVotePlayerColor)
    {
        foreach(var panel in meetingPlayerPanels)
        {
            if(panel.targetPlayer.playerColor == skipVotePlayerColor)
            {
                panel.UpdateVoteSign(true);
            }
        }
        var voter = Instantiate(voterPrefab, skipVoteParentTransform).GetComponent<Image>();
        voter.material = Instantiate(voter.material);
        voter.material.SetColor("_PlayerColor", PlayerColor.GetColor(skipVotePlayerColor));
        skipVoteButton.SetActive(false);
    }

    public void OnClickSkipVoteButton()
    {
        var myCharacter = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as InGameCharacterMover;
        if(myCharacter.isVote) return; // 이미 투표를 한 상태라면, 더이상 투표를 할 수 없다.
        myCharacter.CmdSkipVote(); // 투표를 skip하였음을 알린다.
        SelectPlayerPanel();
    }

    public void CompleteVote()
    {
        foreach(var panel in meetingPlayerPanels)
        {
            panel.OpenResult();
        }
        skipVotePlayers.SetActive(true); // 기권한 플레이어 노출은 투표종료시에만.
    }

    private void Update()
    {
        if(meetingState == EMeetingState.Meeting)
        {
            meetingTimeText.text = string.Format("회의시간: {0}s", (int)Mathf.Clamp(GameSystem.Instance.remainTime, 0f, float.MaxValue));
        }
        else if(meetingState == EMeetingState.Vote)
        {
            meetingTimeText.text = string.Format("투표시간: {0}s", (int)Mathf.Clamp(GameSystem.Instance.remainTime, 0f, float.MaxValue));
        }
    }
}

public enum EMeetingState
{
    None,
    Meeting,
    Vote,
}