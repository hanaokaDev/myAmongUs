using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnPositions : MonoBehaviour
{
    [SerializeField]
    private Transform[] spawnPositions;

    private int index;

    public Vector3 GetSpawnPosition(){
        Vector3 pos = spawnPositions[index].position;
        index++;
        if(index >= spawnPositions.Length){
            index = 0;
        }
        return pos;
    }


}
