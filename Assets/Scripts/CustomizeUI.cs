using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CustomizeUI : MonoBehaviour
{
    [SerializeField]
    private Image characterPreview;
    [SerializeField]
    private List<ColorSelectButton> colorSelectButtons;

    void Start()
    {
        // preview 원본 메터리얼에 영향을 미치지 않게 하기 위해서는
        // 따로 인스턴스를 생성해야 한다.
        var inst = Instantiate(characterPreview.material);
        characterPreview.material = inst;
    }

    private void OnEnable()
    {
        UpdateColorButton();
        var roomSlots = (NetworkManager.singleton as AmongUsRoomManager).roomSlots;
        foreach(var player in roomSlots){
            var aPlayer = player as AmongUsRoomPlayer;
            if(aPlayer.isLocalPlayer){
                UpdatePreviewColor(aPlayer.playerColor);
                break;
            }
        }
    }

    public void UpdateColorButton()
    {
        var roomSlots = (NetworkManager.singleton as AmongUsRoomManager).roomSlots;
        for(int i=0; i<colorSelectButtons.Count; i++)
        {
            colorSelectButtons[i].SetInteractable(true);
        }

        foreach(var player in roomSlots){
            var aPlayer = player as AmongUsRoomPlayer;
            colorSelectButtons[(int)aPlayer.playerColor].SetInteractable(false);
        }
    }

    public void UpdatePreviewColor(EPlayerColor playerColor)
    {
        characterPreview.material.SetColor("_PlayerColor", PlayerColor.GetColor(playerColor));
    }

    public void OnClickColorButton(int index)
    {
       if(colorSelectButtons[index].isInteractable){
            AmongUsRoomPlayer.MyRoomPlayer.CmdSetPlayerColor((EPlayerColor)index);
            UpdatePreviewColor((EPlayerColor)index);
       }
    }

    public void Open()
    {
        AmongUsRoomPlayer.MyRoomPlayer.lobbyPlayerCharacter.IsMovable = false;
        gameObject.SetActive(true);
    }

    public void Close(){
        AmongUsRoomPlayer.MyRoomPlayer.lobbyPlayerCharacter.IsMovable = true;
        gameObject.SetActive(false);
    }

}
