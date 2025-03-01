using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRoomSettingsUI : SettingUI
{
    public void ExitGameRoom()
    {
        var manager = AmongUsRoomManager.singleton;
        if(manager.mode == Mirror.NetworkManagerMode.Host)
        {
            manager.StopHost();
        }
        else if(manager.mode == Mirror.NetworkManagerMode.ClientOnly)
        {
            manager.StopClient();
        }
        
    }
}