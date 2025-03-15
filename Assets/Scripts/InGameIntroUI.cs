using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameIntroUI : MonoBehaviour
{
    [SerializeField]
    private GameObject shhhhObj;

    [SerializeField]
    private GameObject crewmateObj;

    [SerializeField]
    private Text playerType;

    [SerializeField]
    private Image gradientImage;

    [SerializeField]
    private IntroCharacter myCharacter;

    [SerializeField]
    private List<IntroCharacter> otherCharacters = new List<IntroCharacter>();

    [SerializeField]
    private Color crewColor;

    [SerializeField]
    private Color imposterColor;

    [SerializeField]
    private CanvasGroup canvasGroup;

    public IEnumerator ShowIntroSequence()
    {
        Debug.Log("ShowIntroSequence Called");
        shhhhObj.SetActive(true);
        yield return new WaitForSeconds(2f);
        Debug.Log("ShowIntroSequence WaitForSeconds Ended");
        shhhhObj.SetActive(false);

        ShowPlayerType();
        crewmateObj.SetActive(true);
    }

    public void ShowPlayerType()
    {
        var players = GameSystem.Instance.GetPlayerList();
        InGameCharacterMover myPlayer = null;
        foreach(var player in players)
        {
            if(player.isOwned)
            {
                myPlayer = player;
                break;
            }
        }
        myCharacter.SetIntroCharacter(myPlayer.nickname, myPlayer.playerColor); // SetActive는 안해도되나?

        
        if(myPlayer.playerType == EPlayerType.Imposter)
        {
            playerType.text = "임포스터";
            playerType.color = gradientImage.color = imposterColor;
            int i=0;
            foreach(var player in players)
            {
                if(!player.isOwned && player.playerType == EPlayerType.Imposter)
                {
                    otherCharacters[i].SetIntroCharacter(player.nickname, player.playerColor);
                    otherCharacters[i].gameObject.SetActive(true);
                    i++;
                }
            }
        }
        else
        {
            playerType.text = "크루원";
            playerType.color = gradientImage.color = crewColor;
            int i=0;
            foreach(var player in players)
            {
                if(!player.isOwned)
                {
                    otherCharacters[i].SetIntroCharacter(player.nickname, player.playerColor);
                    otherCharacters[i].gameObject.SetActive(true);
                    i++;
                }
            }
        }
    }

    public void Close()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    { // alpha값을 0으로 천천히 옮겨줌.
        float timer = 0f;
        while(timer <= 1f){
            yield return null;
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer);
        }
        gameObject.SetActive(false);
    }

}
