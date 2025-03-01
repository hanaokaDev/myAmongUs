using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        for(int i=0; i<12; i++){
            SpawnFloatingCrew((EPlayerColor)Random.Range(0,12), distance);
            timer = 1f;
        }
    }


    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0f){
            timer = 1f;
            SpawnFloatingCrew((EPlayerColor)Random.Range(0,12), distance);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var crew = collision.GetComponent<FloatingCrew>();
        if(crew != null){
            crewStates[(int)crew.playerColor] = false;
            Destroy(crew.gameObject);
        }
    }

    public void SpawnFloatingCrew(EPlayerColor playerColor, float distance){
        if(crewStates[(int)playerColor]) return;

        crewStates[(int)playerColor] = true;
        float angle = Random.Range(0f, 360f);
        Vector3 spawnPos = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f) * distance;
        Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
        float floatingSpeed = Random.Range(2f, 4f);
        float rotateSpeed = Random.Range(-2f, 2f);

        var floatingCrew = Instantiate(prefab, spawnPos, Quaternion.identity).gameObject.GetComponent<FloatingCrew>();
        floatingCrew.SetFloatingCrew(
            sprites[Random.Range(0, sprites.Count)], 
            playerColor, 
            direction, 
            floatingSpeed,
            rotateSpeed,
            Random.Range(0.5f, 1f)
        );
    }

}
