using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DeadBody : NetworkBehaviour
{   
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    [ClientRpc]
    public void RpcSetColor(EPlayerColor playerColor)
    {
        spriteRenderer.material.SetColor("_PlayerColor", PlayerColor.GetColor(playerColor));
    }
}
