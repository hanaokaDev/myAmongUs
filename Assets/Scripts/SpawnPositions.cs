using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnPositions : MonoBehaviour
{
    [SerializeField]
    private Transform[] spawnPositions;

    private int index;
    public int Index{get {return index;}} // 외부에서 읽기만 가능

    public Vector3 GetSpawnPosition(){
        Vector3 pos = spawnPositions[index].position;
        index++;
        if(index >= spawnPositions.Length){
            index = 0;
        }
        return pos;
    }


}
