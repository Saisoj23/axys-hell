using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Attaks Values")]
    public Vector3[] positions;
    public GameObject[] arrows;

    [Header("Player")]
    public PlayerController player;
    public ShieldController shield;

    public enum Arrows {Arrow, Antiarrow, Inverse, Charging, Smart, Orbital}
    public enum Positions {Right, Left, Up, Down, Down_Right, Down_Left, Up_Right, Up_Left}

    [Header("Attak Editor")]
    public SpawnList spawns;
    public AttackList attack;

    [Header("Json Values")]
    string filePath;
    public string jsonString;

    [Header("Attaks Values")]
    public float timeBetweenAttacks;
    public int indexToTest = -1;

    [Header("UI objects")]
    public Text lifesText;
    public Text timeText;
    public Text bestText;

    bool gamePlaying = false;
    int lifes = 10;
    int bestScore;
    float time = 0;


    void Start()
    {
        #if UNITY_EDITOR
        filePath = Application.dataPath + "/JsonAttacks/4SideAttacks.json";
        jsonString = File.ReadAllText(filePath);
        #endif
        attack = JsonUtility.FromJson<AttackList>(jsonString);
        StartRandomAttacks();
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        bestText.text = bestScore.ToString();
    }

    void Update ()
    {
        if (gamePlaying)
        {
            time += Time.deltaTime;
            timeText.text = ((int)time).ToString();
        }
    }

    //corrutina de testeo
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

    //corrutina de juego
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

    //botones de editor
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

    //game events
    public void Damage ()
    {
        lifes--;
        if (lifes < 0f)
        {
            GameOver();
        }
        else lifesText.text = lifes.ToString();
    }

    public void GameOver ()
    {
        if (time > bestScore)
        {
            bestScore = (int)time;
            PlayerPrefs.SetInt("BestScore", bestScore);
        }
        SceneManager.LoadScene(0);
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
