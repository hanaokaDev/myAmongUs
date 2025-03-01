using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineUI : MonoBehaviour
{
    [SerializeField]
    private InputField nicknameInputField;
    [SerializeField]
    private GameObject createRoomUI;

    public void OnClickCreateRoomButton()
    {   
        Debug.Log("Create Room Button Clicked");
        Debug.Log("Nickname: " + nicknameInputField.text);
        if(nicknameInputField.text != "Enter Nickname"){
            PlayerSettings.nickname = nicknameInputField.text;
            createRoomUI.SetActive(true);
            gameObject.SetActive(false);
        }
        else{
            nicknameInputField.GetComponent<Animator>().SetTrigger("On");
        }
    }

    public void OnClickEnterGameRoomButton() 
    {
        // singelton으로 RoomManager를 찾아서 클라이언트를 시작하게 만들어줌.
        if(nicknameInputField.text != "Enter Nickname"){
            var manager = AmongUsRoomManager.singleton;
            manager.StartClient();
        }
        else{
            nicknameInputField.GetComponent<Animator>().SetTrigger("On");
        }
    }


}
