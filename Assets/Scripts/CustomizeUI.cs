using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CustomizeUI : MonoBehaviour
{
    [SerializeField]
    private Button colorButton;
    [SerializeField]
    private GameObject colorPanel;

    [SerializeField]
    private Button gameRuleButton;
    [SerializeField]
    private GameObject gameRulePanel;

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
        ActiveColorPanel();
        UpdateColorButtonAll();
        var roomSlots = (NetworkManager.singleton as AmongUsRoomManager).roomSlots;
        foreach(var player in roomSlots){
            var aPlayer = player as AmongUsRoomPlayer;
            if(aPlayer.isLocalPlayer){
                UpdatePreviewColor(aPlayer.playerColor);
                break;
            }
        }
    }

    public void UpdateColorButtonAll() // Hook을 통해 Client가 호출하는 함수.
    {
        var roomSlots = (NetworkManager.singleton as AmongUsRoomManager).roomSlots;
        /*
        RoomManager에 Player가 등록되는 시점은, RoomPlayer의 Start 함수가 실행될 때이다.
        서버에서는 이 RoomSlots 등록과 Playercolor 변경이 순서대로 일어나는 보장이 있다.
        그러나 클라이언트에서는 이 순서가 보장되지 않는다.
        따라서, 클라이언트에서는 새로 생성된 플레이어가 RoomSlots에 등록되기 전에 updateColorButton 함수를 호출할수도 있다.
        따라서, 버튼 전체를 Update하는 것보다 필요한 버튼 1개만 업데이트하는 방식으로 하는게 좋다.
            그러므로, UpdateColorButtonInvalid을 따로 만들자.
        */
        for(int i=0; i<colorSelectButtons.Count; i++)
        {
            colorSelectButtons[i].SetInteractable(true);
        }

        foreach(var player in roomSlots){
            Debug.Log("UpdateColorButtonAll - Current My PlayerId is " + AmongUsRoomPlayer.MyRoomPlayer.netId);
            Debug.Log("UpdateColorButtonAll - Current Target PlayerId is " + player.netId);
            var aPlayer = player as AmongUsRoomPlayer;
            colorSelectButtons[(int)aPlayer.playerColor].SetInteractable(false);
            Debug.Log("UpdateColorButtonAll - SetInteractable(false): " + aPlayer.playerColor);
        }
    }
    public void UpdateColorButtonInvalid(EPlayerColor playerColor)
    {
        colorSelectButtons[(int)playerColor].SetInteractable(false);
    }
    public void UpdateColorButtonValid(EPlayerColor playerColor)
    {
        colorSelectButtons[(int)playerColor].SetInteractable(true);
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
        AmongUsRoomPlayer.MyRoomPlayer.myCharacter.IsMovable = false;
        gameObject.SetActive(true);
    }

    public void Close(){
        AmongUsRoomPlayer.MyRoomPlayer.myCharacter.IsMovable = true;
        gameObject.SetActive(false);
    }


    public void ActiveColorPanel()
    {
        colorButton.image.color = new Color(0f, 0f, 0f, 0.75f);
        gameRuleButton.image.color = new Color(0f, 0f, 0f, 0.25f);
        colorPanel.SetActive(true);
        gameRulePanel.SetActive(false);
    }

    public void ActiveGameRulePanel()
    {
        colorButton.image.color = new Color(0f, 0f, 0f, 0.25f);
        gameRuleButton.image.color = new Color(0f, 0f, 0f, 0.75f);
        colorPanel.SetActive(false);
        gameRulePanel.SetActive(true);
    }

}
