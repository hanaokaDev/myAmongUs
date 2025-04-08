using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixWiringTaskObject : MonoBehaviour
{
    [SerializeField]
    private Sprite _UseButtonSprite;

    private SpriteRenderer _SpriteRenderer;

    // start is called before the first frame update
    void Start()
    {
        _SpriteRenderer = GetComponent<SpriteRenderer>();
        _SpriteRenderer.material = Instantiate(_SpriteRenderer.material);
    }


    // 플레이어 접근시 하이라이트
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var character = collision.GetComponent<InGameCharacterMover>();
        if(character != null && character.isOwned)
        {
            _SpriteRenderer.material.SetFloat("_Highlighted", 1f);
            InGameUIManager.Instance.SetUseButton(_UseButtonSprite, OnClickUse);
        }
    }
    // 플레이어 멀어지면 하이라이트 제거
    private void OnTriggerExit2D(Collider2D collision)
    {
        var character = collision.GetComponent<InGameCharacterMover>();
        if(character != null && character.isOwned)
        {
            _SpriteRenderer.material.SetFloat("_Highlighted", 0f);
            InGameUIManager.Instance.UnsetUseButton();
        }
    }

    private void OnClickUse()
    {
        InGameUIManager.Instance.FixWiringTaskUI.Open();
    }
}
