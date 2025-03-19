using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TaskListUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float offset;
    [SerializeField] private RectTransform TaskListUIRectTransform;
    private bool isOpen = true;
    private float timer;

    public void OnPointerClick(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(OpenAndHideUI());
    }
    
    private IEnumerator OpenAndHideUI()
    {
        isOpen = !isOpen;
        if(timer != 0f){
            timer = 1f - timer; // 열리는 도중 클릭하면 지금부터 매끄럽게 닫히는 것처럼 보이게 하기 위함.
        }
        while(timer <= 1f)
        {
            timer += Time.deltaTime * 2f;
            float start = isOpen ? -TaskListUIRectTransform.sizeDelta.x:offset;
            float end = isOpen ? offset:-TaskListUIRectTransform.sizeDelta.x;
            TaskListUIRectTransform.anchoredPosition = new Vector2(Mathf.Lerp(start, end, timer), TaskListUIRectTransform.anchoredPosition.y);
            yield return null;
        }
    }
}

