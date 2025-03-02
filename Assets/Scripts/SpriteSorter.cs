using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpriteSorter : MonoBehaviour
{
    [SerializeField]
    private Transform Back;
    [SerializeField]
    private Transform Front;

    public int GetSortingOrder(GameObject obj){
        // return (int)(transform.position.y * -100);
        float objDist = Mathf.Abs(Back.position.y - obj.transform.position.y);
        float totalDist = Mathf.Abs(Back.position.y - Front.position.y);
        return (int)(Mathf.Lerp(System.Int16.MinValue, System.Int16.MaxValue, objDist/totalDist));
    }

}
