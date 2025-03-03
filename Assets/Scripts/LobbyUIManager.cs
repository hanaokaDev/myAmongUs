using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

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
    private void Awake()
    {
        Instance = this;
    }
}
