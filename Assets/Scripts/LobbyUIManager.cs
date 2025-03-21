using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Mirror;
using Unity.VisualScripting;

public class LobbyUIManager : MonoBehaviour
{
    public static LobbyUIManager Instance; // 싱글턴선언.
    [SerializeField]
    private CustomizeUI customizeUI;
    public CustomizeUI CustomizeUI{
        get{
            return customizeUI;
        }
    }

    [SerializeField]
    private GameRoomPlayerCounter gameRoomPlayerCounter;
    public GameRoomPlayerCounter GameRoomPlayerCounter{
        get{
            return gameRoomPlayerCounter;
        }
    }


    [SerializeField]
    private Button useButton;
    [SerializeField]
    private Sprite originUseButtonSprite;

    [SerializeField]
    private Button startButton;


    private void Awake()
    {
        Instance = this;
    }

    public void SetUseButton(Sprite sprite, UnityAction action)
    {
        useButton.image.sprite = sprite;
        useButton.onClick.AddListener(action);
        useButton.interactable = true;
    }

    public void UnsetUseButton()
    {
        useButton.image.sprite = originUseButtonSprite;
        useButton.onClick.RemoveAllListeners();
        useButton.interactable = false;
    }

    public void ActiveStartButton()
    {
        startButton.gameObject.SetActive(true);
    }

    public void SetInteractableStartButton(bool isInteractable) 
    {
        startButton.interactable = isInteractable;
    }

    public void OnClickStartButton()
    {
        Debug.Log("OnClickStartButton Called!");
        var manager = NetworkManager.singleton as AmongUsRoomManager;
        manager.gameRuleData = FindObjectOfType<GameRuleStore>().GetGameRuleData();

        var players = FindObjectsOfType<AmongUsRoomPlayer>();
        for(int i=0;i<players.Length;i++)
        {
            players[i].CmdChangeReadyState(true);
        }
        
        manager.ServerChangeScene(manager.GameplayScene);
        
    }
}
