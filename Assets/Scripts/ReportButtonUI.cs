using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReportButtonUI : MonoBehaviour 
{
    [SerializeField]
    private Button reportButton;

    public void SetInteractable(bool isInteractable)
    {
        reportButton.interactable = isInteractable;
    }
    

    public void OnClickButton()
    {
        var myCharacter = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as InGameCharacterMover;
        myCharacter.Report();
    }
}
