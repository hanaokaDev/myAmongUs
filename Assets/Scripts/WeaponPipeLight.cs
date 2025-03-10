using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPipeLight : MonoBehaviour
{
    private Animator animator;
    private WaitForSeconds waitTime = new WaitForSeconds(0.15f);

    private List<WeaponPipeLight> lights = new List<WeaponPipeLight>();

    void Start()
    {
        animator = GetComponent<Animator>();
        for(int i=0; i<transform.childCount; i++){
            var child = transform.GetChild(i).GetComponent<WeaponPipeLight>();
            if(child != null){
                lights.Add(child);
            }
        }
    }

    public void TurnOnLight()
    {
        animator.SetTrigger("on");
        StartCoroutine(TurnOnLightAtChild()); 
    }    

    private IEnumerator TurnOnLightAtChild()
    {
        yield return waitTime;
        foreach(var child in lights){
            child.TurnOnLight();
        }
    }
}
