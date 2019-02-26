using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameController : MonoBehaviour
{
    string filePath;
    string jsonString;

    public Vector3[] positions;
    public GameObject[] arrows;

    public enum Arrows {Arrow, Antiarrow, Inverse, Charging, Smart, Orbital}
    public enum Positions {Right, Left, Up, Down, Down_Right, Down_Left, Up_Right, Up_Left}

    public SpawnList spawns;


    void Start()
    {
        filePath = Application.dataPath + "/JsonAttacks/4SideAttacks.json";
        jsonString = File.ReadAllText(filePath);
        spawns = JsonUtility.FromJson<SpawnList>(jsonString);
        //jsonString = JsonUtility.ToJson(spawns);
        //File.WriteAllText(filePath, jsonString);
        
        StartCoroutine("Spawner");
    }

    IEnumerator Spawner ()
    {
        foreach (Spawn i in spawns.spawns)
        {
            int intArrow = (int)i.arrow;
            int intPosition = (int)i.position;
            ArrowController arrow = Instantiate(arrows[intArrow], positions[intPosition], Quaternion.identity).GetComponent<ArrowController>();
            arrow.speed = i.speed;
            arrow.secondSpeed = i.secondSpeed;
            if (i.spawnTime > 0) yield return new WaitForSeconds(i.spawnTime);
        }
    }

    [System.Serializable]
    public struct Spawn
    {
        public Arrows arrow;
        public Positions position;
        public float spawnTime;
        public float speed;
        public float secondSpeed;
    }

    [System.Serializable]
    public struct SpawnList
    {
        public Spawn[] spawns;
    }
}
