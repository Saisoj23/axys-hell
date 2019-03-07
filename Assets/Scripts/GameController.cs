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
    public string filePath;
    string jsonString;

    [Header("Attaks Values")]
    public float speedModifier;
    public int indexToTest = -1;

    [Header("UI objects")]
    public TMPro.TMP_Text timeText;
    public TMPro.TMP_Text bestText;

    [Header("Game Status")]
    public bool gamePlaying = false;
    public bool editing = false;
    int bestScore;
    float time = 0f;
    [HideInInspector] public float speedByTime = 1;


    void Start()
    {
        #if UNITY_EDITOR
        jsonString = File.ReadAllText(filePath);
        attack = JsonUtility.FromJson<AttackList>(jsonString);
        #else
        PlayAttaks();
        #endif
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        bestText.text = bestScore.ToString();
    }

    void Update ()
    {
        if (gamePlaying)
        {
            time += Time.deltaTime;
            timeText.text = ((int)time).ToString();
            speedByTime = Mathf.Clamp((1 + (time * speedModifier)), 1f, 3f);
        }
    }

    //corrutina de juego
    IEnumerator Attack ()
    {
        time = 0;
        while (gamePlaying && attack.attacks.Count > 0)
        {
            if (indexToTest >= 0 && indexToTest < attack.attacks.Count) spawns = attack.attacks[indexToTest];
            else spawns = attack.attacks[Random.Range(0, attack.attacks.Count)];
            indexToTest = -1;
            int inverse = Random.Range(0, 2);
            int perpendicular = Random.Range(0, 2);
            foreach (Spawn i in spawns.spawns)
            {
                int intArrow = (int)i.arrow;
                int intPosition = (int)i.position;
                Vector3 thisPosition = positions[intPosition];
                if (perpendicular == 1) thisPosition = Vector2.Perpendicular(thisPosition);
                if (inverse == 1) thisPosition = -thisPosition;
                ArrowController arrow = Instantiate(arrows[intArrow], thisPosition, Quaternion.identity).GetComponent<ArrowController>();
                arrow.speed = i.speed * speedByTime;
                arrow.secondSpeed = i.secondSpeed * speedByTime;
                if (i.spawnTime > 0) yield return new WaitForSeconds(i.spawnTime / speedByTime);
            }
        }
    }

    //corrutina de testeo
    IEnumerator Spawner ()
    {
        time = 0;
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

    public void WriteJson ()
    {
        jsonString = JsonUtility.ToJson(attack);
        File.WriteAllText(filePath, jsonString);
    }

    public void UploadToList ()
    {
        attack.attacks.Add(spawns);
    }

    public void PlayAttaks ()
    {
        if (gamePlaying)
        {
            StopAllCoroutines();
            gamePlaying = false;
        }
        else 
        {
            gamePlaying = true;
            StartCoroutine("Attack");
        }
    }

    public void TestAttack ()
    {
        StartCoroutine("Spawner");
    }

    //game events
    public void Damage ()
    {
        if (!editing) GameOver();
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
