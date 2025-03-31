using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFinder : MonoBehaviour
{
    private CircleCollider2D circleCollider;
    public List<InGameCharacterMover> targets = new List<InGameCharacterMover>();

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void SetKillRange(float range)
    {
        circleCollider.radius = range;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<InGameCharacterMover>();
        if (player && player.playerType == EPlayerType.Crew)
        {
            if(!targets.Contains(player))
            {
                targets.Add(player);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var player = collision.GetComponent<InGameCharacterMover>();
        if (player && player.playerType == EPlayerType.Crew)
        {
            if(targets.Contains(player))
            {
                targets.Remove(player);
            }
        }
    }

}
