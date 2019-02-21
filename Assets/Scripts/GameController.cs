using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    
    public float spawnTime;
    //public float spawnAceleration;
    public Vector3[] spawns;
    public GameObject[] arrows;

    void Start()
    {
        StartCoroutine("Spawner");
    }

    IEnumerator Spawner ()
    {
        while (true)
        {
            int arrow = Random.Range(0, arrows.Length);
            int position = Random.Range(0, spawns.Length);
            Instantiate(arrows[arrow], spawns[position], Quaternion.identity);
            if (arrow == 1) yield return new WaitForSeconds(spawnTime + spawnTime / 2);
            else yield return new WaitForSeconds(spawnTime);
        }
    }
}
