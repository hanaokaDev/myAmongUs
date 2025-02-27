using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using NUnit.Framework.Constraints;
using UnityEngine;

public class CrewFloater : MonoBehaviour
{    
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private List<Sprite> sprites;

    private bool[] crewStates = new bool[12];
    private float timer = 0.5f;
    private float distance = 11f;

    public void SpawnFloatingCrew(EPlayerColor playerColor, float dist){
        float angle = Random.Range(0f, 360f);
        Vector3 spawnPos = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f) * distance;
        var floatingCrew = Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    void Update()
    {
        SpawnFloatingCrew((EPlayerColor)Random.Range(0,12), distance);
    }
}
