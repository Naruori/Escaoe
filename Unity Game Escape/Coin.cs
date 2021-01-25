using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour // 장애물 없애기 제목
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "Player")
        {
            DestroySphere();
            Destroy(gameObject);
        }
    }

    void DestroySphere()
    {
        GameObject[] sphere = GameObject.FindGameObjectsWithTag("Sphere");
        for(int i=0; i<sphere.Length; i++)
        {
            Destroy(sphere[i]);
        }
    }
}
