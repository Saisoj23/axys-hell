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

    [Header("UI Texts")]
    public TMPro.TMP_Text timeText;
    public TMPro.TMP_Text bestText;
    public TMPro.TMP_Text tittleText;
    public TMPro.TMP_Text subTittleText;

    [Header("UI Objects")]
    public Button playButton;
    public Button pauseButton;
    public Image bestImage;
    public Image timeImage;
    public Image pauseImage;

    [Header("Game Status")]
    public bool gamePlaying = false;
    public bool pause = false;
    public bool editing = false;
    public int level = 0;
    public int pauseMultiplier = 1;
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
        if (gamePlaying && !pause)
        {
            time += Time.deltaTime * 10;
            timeText.text = ((int)time).ToString();
            if ((int)time > bestScore) bestText.text = ((int)time).ToString();
            speedByTime = Mathf.Clamp((1 + (time * speedModifier)), 1f, 1.5f);
        }
        if (!gamePlaying && Input.GetButtonDown("Submit"))
        {
            PlayAttaks(true);
        }
        if (gamePlaying && Input.GetButtonDown("Cancel"))
        {
            Pause();
        }
    }

    //corrutina de juego
    IEnumerator Attack ()
    {
        time = 0;
        speedByTime = 1;
        while (gamePlaying && attacks.attacks.Length > 0)
        {
            int newIndex = 0;
            if (indexToTest >= 0 && indexToTest < attacks.attacks.Length) newIndex = indexToTest;
            else newIndex = Random.Range(0, attacks.attacks.Length);
            int inverse = Random.Range(0, 2);
            int perpendicular = Random.Range(0, 2);
            foreach (Spawn i in attacks.attacks[newIndex].spawns)
            {
                int intArrow = (int)i.arrow;
                int intPosition = (int)i.position;
                Vector3 thisPosition = positions[intPosition];
                if (perpendicular == 1) thisPosition = Vector2.Perpendicular(thisPosition);
                if (inverse == 1) thisPosition = -thisPosition;
                ArrowController arrow = Instantiate(arrows[intArrow], thisPosition, Quaternion.Euler(0, 0, 180)).GetComponent<ArrowController>();
                arrow.StartValues(i.speed * speedByTime, i.secondSpeed * speedByTime, level);
                for (float t = 0f; t < i.spawnTime / speedByTime; t += Time.deltaTime * pauseMultiplier)
                {
                    yield return null;
                }
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
        if (pause)
        {
            Pause();
            return;
        }
        gamePlaying = active;
        if (active)
        {
            DestroyAllBullets();
            StartCoroutine("Attack");
        }
        else
        {
            StopCoroutine("Attack");
        }
        pauseButton.enabled = active;
        playButton.enabled = !active;
        anim.SetBool("Playing", active);
    }

    public void Pause ()
    {
        pause = !pause;
        if (pause) 
        {
            tittleText.SetText("Paused");
            subTittleText.SetText("Tap anywhere to\nresume");
            pauseMultiplier = 0;
        }
        else 
        {
            pauseMultiplier = 1;
        }
        anim.SetBool("Pause", pause);
        playButton.enabled = pause;
        PauseAllBullets();
    }

    //game events
    public void Damage ()
    {
        if (editing) return;
        if (time > bestScore)
        {
            bestScore = (int)time;
            PlayerPrefs.SetInt("BestScore", bestScore);
        }
        gamePlaying = false;
        PlayAttaks(false);
        StopAllBullets();
        anim.SetBool("Playing", gamePlaying);
        tittleText.SetText("Paused");
        subTittleText.SetText("Tap anywhere to\nresume");
        tittleText.SetText("You lose!");
        subTittleText.SetText("Tap anywhere to\nrestart");
    }

    void StopAllBullets ()
    {
        ArrowController[] actualArrows = GameObject.FindObjectsOfType<ArrowController>();
            foreach (ArrowController item in actualArrows)
            {
                item.Stop();
            }
    }

    void PauseAllBullets ()
    {
        ArrowController[] actualArrows = GameObject.FindObjectsOfType<ArrowController>();
            foreach (ArrowController item in actualArrows)
            {
                item.Pause(pause);
            }
    }

    void DestroyAllBullets ()
    {
        ArrowController[] actualArrows = GameObject.FindObjectsOfType<ArrowController>();
            foreach (ArrowController item in actualArrows)
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
