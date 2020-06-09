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
    List<List<SpawnController.Spawn>> spawnsFixedTime;

    [Header("Attaks Values")]
    public float arrowSpanwDistance;

    [Header("Game Status")]
    public int indexToTest;
    public bool gamePlaying = false;
    public bool pause = false;
    public bool editing = false;
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
        #if UNITY_EDITOR
        spawn.ReadJson();
        #endif
        OverrideInspector();
    }

    void Update ()
    {
        if (gamePlaying && !pause)
        {
            time = Time.time - timeBeforePlay;
            score = time * 10;
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
        spawnsFixedTime = new List<List<SpawnController.Spawn>>();
        int index = 0;
        foreach (SpawnController.Spawns listItem in spawn.attacks)
        {
            spawnsFixedTime.Add(new List<SpawnController.Spawn>());
            foreach (SpawnController.Spawn item in listItem.SpawnList)
            {
                SpawnController.Spawn spawnItem = (SpawnController.Spawn)item.Clone();
                spawnItem.spawnTime -= arrowSpanwDistance / spawnItem.speed;
                spawnsFixedTime[index].Add(spawnItem);
                print("override");
            }
            spawnsFixedTime[index].Sort((x, y) => x.spawnTime.CompareTo(y.spawnTime));
            index += 1;
        }
    }

    IEnumerator Attack ()
    {
        timeBeforePlay = Time.time;
        time = 0f;
        int indexCount = spawnsFixedTime.Count;
        
        float firstSpawn = 0f;
        float lastCollision = 0f;

        while (gamePlaying)
        {
            print("while");
            int newIndex = 0;
            if (indexToTest >= 0 && indexToTest < indexCount) newIndex = indexToTest;
            else newIndex = Random.Range(0, indexCount);
            int inverse = Random.Range(0, 2);
            int perpendicular = Random.Range(0, 2);
            int mirror = Random.Range(0, 2);

            float attackSpawnTime = spawnsFixedTime[newIndex][0].spawnTime;
            if (lastCollision - time > -attackSpawnTime && time > 0)
            {
                print("attackDelay " + (lastCollision - attackSpawnTime));
                yield return new WaitForSeconds(lastCollision - time + attackSpawnTime);
            }

            firstSpawn = time;
            lastCollision = 0;
            if (attackSpawnTime < 0) firstSpawn -= attackSpawnTime;
            foreach (SpawnController.Spawn i in spawnsFixedTime[newIndex])
            {
                if (i.spawnTime > time - firstSpawn)
                {
                    print("spawnDelay " + (i.spawnTime - (time - firstSpawn)));
                    yield return new WaitForSeconds(i.spawnTime - (time - firstSpawn));
                }
                int intArrow = (int)i.arrow;
                int intPosition = (int)i.position;
                Vector3 thisPosition = positions[intPosition];
                if (perpendicular == 1) thisPosition = Vector2.Perpendicular(thisPosition);
                if (inverse == 1) thisPosition = -thisPosition;
                if (mirror == 1) thisPosition = thisPosition * new Vector2(-1, 1);
                ArrowController arrow = Instantiate(arrows[intArrow], thisPosition, Quaternion.Euler(0, 0, 180)).GetComponent<ArrowController>();
                arrow.StartValues(i.speed, i.actionDistance);

                float thisAttack = (arrowSpanwDistance / i.speed) + i.spawnTime + firstSpawn;
                lastCollision = thisAttack > lastCollision ? thisAttack : lastCollision;
                print (i.arrow + " " + time);
            }
        }
        yield return null;
    }

    public void PlayAttaks (bool active)
    {
        if (pause)
        {
            Pause();
            //sound.Pause();
            return;
        }
        gamePlaying = active;
        if (active)
        {
            //sound.Play();
            DestroyAllBullets();
            StartCoroutine("Attack");
        }
        else
        {
            //sound.Stop();
            StopCoroutine("Attack");
        }
        ui.Play(active);
    }

    public void Pause ()
    {
        pause = !pause;
        if (pause) 
        {
            Time.timeScale = 0;
        }
        else 
        {
            Time.timeScale = 1;
        }
        ui.Pause(pause);
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

    void DestroyAllBullets ()
    {
        ArrowController[] actualArrows = GameObject.FindObjectsOfType<ArrowController>();
            foreach (ArrowController item in actualArrows)
            {
                item.MoveAndDestroy();
            }
    }
}
