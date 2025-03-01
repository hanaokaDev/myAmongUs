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

}
