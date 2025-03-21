using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CreateRoomUI : MonoBehaviour
{
    [SerializeField]
    private List<Image> crewImgs;
    [SerializeField]
    private List<Button> imposterCountButtons;
    [SerializeField]
    private List<Button> maxPlayerCountButtons;

    private CreateGameRoomData roomData;

    void Start()
    {
        for(int i=0; i<crewImgs.Count; i++){
            Material materialInstance = new Material(crewImgs[i].material);
            crewImgs[i].material = materialInstance; // 승무원마다 Material 이 별개로 유지되어 색깔을 따로 관리할 수 있도록, 각각 인스턴스화하기.
        }
        roomData = new CreateGameRoomData(){
            imposterCount = 1,
            maxPlayerCount = 10
        };
        UpdateCrewImages();
    }

    public void UpdateImposterCount(int count){
        Debug.Log("Called UpdateImposterCount: " + count);
        roomData.imposterCount = count;
        for(int i=0; i<imposterCountButtons.Count; i++){
            if(i == count - 1){
                imposterCountButtons[i].image.color = new Color(1f, 1f, 1f, 1f);
            }
            else{
                imposterCountButtons[i].image.color = new Color(1f, 1f, 1f, 0f);
            }
        }

        int minimumMaxPlayer = count == 1 ? 4 : count == 2 ? 7 : 9;
        if(roomData.maxPlayerCount < minimumMaxPlayer){
            UpdateMaxPlayerCount(minimumMaxPlayer);
        }
        else{
            UpdateMaxPlayerCount(roomData.maxPlayerCount);
        }

        for(int i=0; i<maxPlayerCountButtons.Count; i++){
            var text = maxPlayerCountButtons[i].GetComponentInChildren<Text>();
            if(i < minimumMaxPlayer - 4){
                maxPlayerCountButtons[i].interactable = false;
                text.color = Color.gray;
            }
            else{
                maxPlayerCountButtons[i].interactable = true;
                text.color = Color.white;
            }
        }
    }

    public void UpdateMaxPlayerCount(int count){
        Debug.Log("Called UpdateMaxPlayerCount: " + count);
        roomData.maxPlayerCount = count;
        Debug.Log("maxPlayerCount: " + roomData.maxPlayerCount);
        for(int i=0; i<maxPlayerCountButtons.Count; i++){
            if(i == count - 4){
                maxPlayerCountButtons[i].image.color = new Color(1f, 1f, 1f, 1f);
            }
            else{
                maxPlayerCountButtons[i].image.color = new Color(1f, 1f, 1f, 0f);
            }
        }
        UpdateCrewImages();
    }

    private void UpdateCrewImages(){  // 갯수만큼의 랜덤한 임포스터 이미지를 빨간색으로 변경.
        for(int i=0; i<crewImgs.Count; i++){ // 일단 흰색으로 초기화.
            crewImgs[i].material.SetColor("_PlayerColor", Color.white);
        }

        int imposterCount = roomData.imposterCount;
        int idx = 0;
        while(imposterCount != 0){
            if(idx >= roomData.maxPlayerCount)
            {
                idx = 0;
            }
            if(crewImgs[idx].material.GetColor("_PlayerColor") != Color.red && Random.Range(0,5) == 0){
                crewImgs[idx].material.SetColor("_PlayerColor", Color.red);
                imposterCount--;
            }
            idx++;
        }
        for(int i=0; i<crewImgs.Count; i++){ // 개수를 초과한 이미지는 비활성화.
            if(i < roomData.maxPlayerCount){
                crewImgs[i].gameObject.SetActive(true);
            }
            else{
                crewImgs[i].gameObject.SetActive(false);
            }
        }
    }
    public void CreateRoom()
    {
        var manager = NetworkManager.singleton as AmongUsRoomManager;
        manager.minPlayerCount = roomData.imposterCount == 1 ? 4 : roomData.imposterCount == 2 ? 7 : 9;
        manager.imposterCount = roomData.imposterCount;
        manager.maxConnections = roomData.maxPlayerCount;
        manager.StartHost(); // 서버를 여는 동시에 클라이언트로써 게임에 참가하도록 만들어주는 함수.
    }
}

public class CreateGameRoomData
{
    public int imposterCount;
    public int maxPlayerCount;
}
