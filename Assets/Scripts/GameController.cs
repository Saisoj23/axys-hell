using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameController : MonoBehaviour
{
    string filePath;
    public string jsonString;

    public Vector3[] positions;
    public GameObject[] arrows;

    public enum Arrows {Arrow, Antiarrow, Inverse, Charging, Smart, Orbital}
    public enum Positions {Right, Left, Up, Down, Down_Right, Down_Left, Up_Right, Up_Left}

    public SpawnList spawns;
    public AttackList attack;

    public float timeBetweenAttacks;
    public int indexToTest = -1;

    bool gamePlaying = false;


    void Start()
    {
        #if UNITY_EDITOR
        filePath = Application.dataPath + "/JsonAttacks/4SideAttacks.json";
        jsonString = File.ReadAllText(filePath);
        #endif
        attack = JsonUtility.FromJson<AttackList>(jsonString);
        StartRandomAttacks();
    }

    IEnumerator Spawner ()
    {
        if (spawns.spawns.Length > 0)
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
    }

    IEnumerator Attack ()
    {
        while (gamePlaying && attack.attacks.Count > 0)
        {
            if (indexToTest >= 0 && indexToTest < attack.attacks.Count) spawns = attack.attacks[indexToTest];
            else spawns = attack.attacks[Random.Range(0, attack.attacks.Count)];
            foreach (Spawn i in spawns.spawns)
            {
                int intArrow = (int)i.arrow;
                int intPosition = (int)i.position;
                ArrowController arrow = Instantiate(arrows[intArrow], positions[intPosition], Quaternion.identity).GetComponent<ArrowController>();
                arrow.speed = i.speed;
                arrow.secondSpeed = i.secondSpeed;
                if (i.spawnTime > 0) yield return new WaitForSeconds(i.spawnTime);
            }
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    public void OverrideJson ()
    {
        jsonString = JsonUtility.ToJson(attack);
        File.WriteAllText(filePath, jsonString);
    }

    public void AddAttackToList ()
    {
        attack.attacks.Add(spawns);
    }

    public void StartRandomAttacks ()
    {
        if (gamePlaying)
        {
            gamePlaying = false;
        }
        else 
        {
            gamePlaying = true;
            StartCoroutine("Attack");
        }
    }

    public void TestActualAttack ()
    {
        StartCoroutine("Spawner");
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

    [System.Serializable]
    public struct  AttackList
    {
        public List<SpawnList> attacks;
    }
}
