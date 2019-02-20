using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    
    public GameObject bullet;
    public float spawnTime;
    public float spawnAceleration;
    public Vector3[] spawns;

    void Start()
    {
        StartCoroutine("Spawner");
    }

    IEnumerator Spawner ()
    {
        while (true)
        {
            Instantiate(bullet, spawns[Random.Range(0, spawns.Length)], Quaternion.identity);
            yield return new WaitForSeconds(spawnTime);
        }
    }
}
