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

    private List<MeetingPlayerPanel> meetingPlayerPanels = new List<MeetingPlayerPanel>();

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
}
