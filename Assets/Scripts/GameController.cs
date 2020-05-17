using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Player")]
    public PlayerController player;
    public ShieldController shield;

    [Header("Attaks Values")]
    public Vector3[] positions;
    public GameObject[] arrows;

    [Header("Attak Editor")]
    SpawnController spawn;
    public List<SpawnController.Spawn> spawnsFixedTime;

    [Header("Attaks Values")]
    public float arrowSpanwDistance;
    public float secondToPlay = 0;

    [Header("Game Status")]
    public bool gamePlaying = false;
    public bool pause = false;
    public bool editing = false;
    public int pauseMultiplier = 1;
    public float levelTime;
    [HideInInspector] public float score;
    [HideInInspector] public int bestScore;
    [HideInInspector] public float time = 0f;
    float timeBeforePlay;
    AudioSource sound;
    UIController ui;

    void Start ()
    {   
        spawn = GetComponent<SpawnController>();
        ui = GetComponent<UIController>();
        sound = GetComponent<AudioSource>();
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        spawn.ReadJson();
        OverrideInspector();
    }

    void Update ()
    {
        if (gamePlaying && !pause)
        {
            time = Time.time - timeBeforePlay;
            score = Mathf.InverseLerp(0f, levelTime, time) * 100;
            if ((int)score > bestScore)
            {
                bestScore = (int)score;
            }
        }
        if (!gamePlaying && Input.GetButtonDown("Submit"))
        {
            PlayAttaks(true);
        }
        if (gamePlaying && Input.GetButtonDown("Cancel"))
        {
            PlayAttaks(false);
        }
    }

    public void OverrideInspector()
    {
        spawnsFixedTime = new List<SpawnController.Spawn>();
        foreach (SpawnController.Spawns listItem in spawn.attacks)
        {
            foreach (SpawnController.Spawn item in listItem.SpawnList)
            {
                SpawnController.Spawn spawnItem = (SpawnController.Spawn)item.Clone();
                spawnItem.spawnTime -= arrowSpanwDistance / spawnItem.speed;
                spawnsFixedTime.Add(spawnItem);
            }
        }
        spawnsFixedTime.Sort((x, y) => x.spawnTime.CompareTo(y.spawnTime));
    }

    IEnumerator Attack ()
    {
        time = secondToPlay;
        timeBeforePlay = Time.time - secondToPlay;
        int indexCount = spawnsFixedTime.Count;
        int actualIndex = 0;
        float lastWaitFor = spawnsFixedTime[0].spawnTime;
        if (secondToPlay != 0f)
        {
            for (int i = 0; i < indexCount; i++)
            {
                if (spawnsFixedTime[i].spawnTime >= secondToPlay)
                {
                    lastWaitFor = spawnsFixedTime[i].spawnTime;
                    actualIndex = i;
                    break;
                }
            }
        }
        
        while (gamePlaying)
        {
            if (time >= lastWaitFor && actualIndex >= 0)
            {
                SpawnController.Spawn thisArrow = spawnsFixedTime[actualIndex];
                ArrowController arrow = Instantiate(arrows[(int)thisArrow.arrow], 
                    positions[(int)thisArrow.position], 
                    Quaternion.Euler(0, 0, 180)).GetComponent<ArrowController>();
                arrow.StartValues(thisArrow.speed, thisArrow.actionDistance);
                actualIndex++;
                if (actualIndex < indexCount)
                {
                    print ("update last wait for " + actualIndex);
                    lastWaitFor = spawnsFixedTime[actualIndex].spawnTime;
                }
                else 
                {
                    print ("damage " + actualIndex);
                    actualIndex = -1;
                }
            }
            else 
            {
                if (time >= levelTime)
                {
                    PlayAttaks(false);
                }
                yield return null;
            }
        }
        yield return null;
    }

    public void PlayAttaks (bool active)
    {
        if (pause)
        {
            Pause();
            sound.Pause();
            return;
        }
        gamePlaying = active;
        if (active)
        {
            sound.time = secondToPlay;
            sound.Play();
            DestroyAllBullets();
            StartCoroutine("Attack");
        }
        else
        {
            sound.Stop();
            StopCoroutine("Attack");
        }
        ui.Play(active);
    }

    public void Pause ()
    {
        pause = !pause;
        if (pause) 
        {
            pauseMultiplier = 0;
        }
        else 
        {
            pauseMultiplier = 1;
        }
        ui.Pause(pause);
        PauseAllBullets();
    }

    //game events
    public void Damage ()
    {
        if (editing) return;
        PlayerPrefs.SetInt("BestScore", bestScore);
        gamePlaying = false;
        ui.Stop();
        PlayAttaks(false);
        StopAllBullets();
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
}
