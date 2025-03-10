using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPipeLightStarter : MonoBehaviour
{
    private WaitForSeconds waitTime = new WaitForSeconds(1f);
    private List<WeaponPipeLight> lights = new List<WeaponPipeLight>();

    void Start()
    {
        for(int i=0; i<transform.childCount; i++){
            var child = transform.GetChild(i).GetComponent<WeaponPipeLight>();
            if(child != null){
                lights.Add(child);
            }
        }
        StartCoroutine(TurnOnPipeLight());
    }

    private IEnumerator TurnOnPipeLight()
    {
        while(true){
            yield return waitTime;
            foreach(var light in lights){
                light.TurnOnLight();
            }
        }
    }
}
