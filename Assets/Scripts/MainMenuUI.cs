using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public void OnClickOnlineButton()
    {
        Debug.Log("Online Button Clicked");
    }

    public void onClickQuitButton()
    {
        #if UNITY_EDITOR
        // 에디터에서 실행중이라면
        UnityEditor.EditorApplication.isPlaying = false;
        #else 
        // build된 상태라면
        Application.Quit();
        #endif
        
    }   
}
