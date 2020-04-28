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
    public float secondToTest = 0;

    [Header("Game Status")]
    public bool gamePlaying = false;
    public bool pause = false;
    public bool editing = false;
    public int pauseMultiplier = 1;
    public float levelTime;
    [HideInInspector] public float score;
    [HideInInspector] public int bestScore;
    [HideInInspector] public float time = 0f;
    UIController ui;

    void Start ()
    {   
        spawn = GetComponent<SpawnController>();
        ui = GetComponent<UIController>();
        bestScore = PlayerPrefs.GetInt("BestScore", 0);

        foreach (SpawnController.Spawns listItem in spawn.attacks)
        {
            foreach (SpawnController.Spawn item in listItem.SpawnList)
            {
                SpawnController.Spawn spawn = item;
                spawn.spawnTime -= arrowSpanwDistance / item.speed;
                spawnsFixedTime.Add(item);
            }
        }
        spawnsFixedTime.Sort((x, y) => x.spawnTime.CompareTo(y.spawnTime));
    }

    void Update ()
    {
        if (gamePlaying && !pause)
        {
            time += Time.deltaTime;
            score += Time.deltaTime * 10f;
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

    IEnumerator Attack ()
    {
        time = 0;
        #if UNITY_EDITOR
        time = secondToTest;
        #endif
        float lastWaitFor = spawnsFixedTime[0].spawnTime;
        int actualIndex = 0;
        int indexCount = spawnsFixedTime.Count;
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
            return;
        }
        gamePlaying = active;
        if (active)
        {
            score = 0;
            DestroyAllBullets();
            StartCoroutine("Attack");
        }
        else
        {
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
        if (time * 10 > bestScore)
        {
            bestScore = (int)score;
            PlayerPrefs.SetInt("BestScore", bestScore);
        }
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
