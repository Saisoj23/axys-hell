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
    public Camera cam;

    public enum Arrows {Arrow, Antiarrow, Inverse, Charging, Smart, Orbital}
    public enum Positions {Right, Left, Up, Down, Down_Right, Down_Left, Up_Right, Up_Left}

    [Header("Attak Editor")]
    public AttackList attacks;

    #if UNITY_EDITOR
    [Header("Json Values")]
    public string filePath;
    string jsonString;
    #endif

    [Header("Attaks Values")]
    public float speedModifier;
    public int indexToTest = -1;

    [Header("UI objects")]
    public TMPro.TMP_Text timeText;
    public TMPro.TMP_Text bestText;
    public Button playButton;
    public Button nextButton;
    public Button prevButton;
    public Spin spin;

    [Header("Game Status")]
    public bool gamePlaying = false;
    public bool editing = false;
    int bestScore;
    float time = 0f;
    [HideInInspector] public float speedByTime = 1;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        #if UNITY_EDITOR
        jsonString = File.ReadAllText(filePath);
        attacks = JsonUtility.FromJson<AttackList>(jsonString);
        #endif
        if (Screen.width > Screen.height)
        {
            cam.orthographicSize *= Mathf.InverseLerp(0f, Screen.width, Screen.height);
        }
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        bestText.text = bestScore.ToString();
    }

    void Update ()
    {
        if (gamePlaying)
        {
            time += Time.deltaTime * 10;
            timeText.text = ((int)time).ToString();
            if ((int)time > bestScore) bestText.text = ((int)time).ToString();
            speedByTime = Mathf.Clamp((1 + (time * speedModifier)), 1f, 1.5f);
        }
        if (!gamePlaying && Input.GetKeyDown(KeyCode.Return))
        {
            PlayAttaks(true);
        }
    }

    //corrutina de juego
    IEnumerator Attack ()
    {
        time = 0;
        bool spining = false;
        while (gamePlaying && attacks.attacks.Length > 0)
        {
            if (time > 500 && !spining) 
            {
                spin.StartCoroutine("StartSpin");
                spining = true;
            }
            int newIndex = 0;
            if (indexToTest >= 0 && indexToTest < attacks.attacks.Length) newIndex = indexToTest;
            else newIndex = Random.Range(0, attacks.attacks.Length);
            indexToTest = -1;
            int inverse = Random.Range(0, 2);
            int perpendicular = Random.Range(0, 2);
            foreach (Spawn i in attacks.attacks[newIndex].spawns)
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

    #if UNITY_EDITOR
    public void WriteJson ()
    {
        jsonString = JsonUtility.ToJson(attacks);
        File.WriteAllText(filePath, jsonString);
    }
    #endif

    public void PlayAttaks (bool active)
    {
        gamePlaying = active;
        if (active)
        {
            DestroyAllBullets();
            spin.StartCoroutine("RestarSpin");
            StartCoroutine("Attack");
        }
        else
        {
            StopCoroutine("Attack");
        }
        playButton.enabled = !active;
        nextButton.enabled = !active;
        prevButton.enabled = !active;
        anim.SetBool("Playing", active);
    }

    //game events
    public void Damage ()
    {
        if (!editing)
        {
            if (time > bestScore)
            {
                bestScore = (int)time;
                PlayerPrefs.SetInt("BestScore", bestScore);
            }
            gamePlaying = false;
            PlayAttaks(false);
            PauseAllBullets();
            spin.StopAllCoroutines();
            anim.SetBool("Playing", gamePlaying);
        }
    }

    void PauseAllBullets ()
    {
        ArrowController[] arrows = GameObject.FindObjectsOfType<ArrowController>();
            foreach (ArrowController item in arrows)
            {
                item.Stop();
            }
    }

    void DestroyAllBullets ()
    {
        ArrowController[] arrows = GameObject.FindObjectsOfType<ArrowController>();
            foreach (ArrowController item in arrows)
            {
                item.MoveAndDestroy();
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

    [System.Serializable]
    public struct  AttackList
    {
        public SpawnList[] attacks;
    }
}
