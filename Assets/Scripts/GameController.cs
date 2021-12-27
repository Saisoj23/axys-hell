using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int level;
    public int difficulty;
    public string spawnsString = "";
    public GameObject completedText;
    public GameObject savePointsText;
    public bool custom = false;
    int[] spawnsSlected = {1, 0, 0, 0};
    [Header("Player")]
    public PlayerController player;

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
    public float score;
    public int savedScore;
    public int bestScore;
    public float time = 0f;
    public float timeBeforePlay;
    AudioSource music;
    UIController ui;
    MusicAndData musicAndData;
    SceneChange scene;
    public SpinnCamera spinn;

    void Start ()
    {   
        spawn = GetComponent<SpawnController>();
        ui = GetComponent<UIController>();
        music = GetComponent<AudioSource>();
        musicAndData = FindObjectOfType<MusicAndData>();
        scene = FindObjectOfType<SceneChange>();
        completedText.SetActive(false);
        savePointsText.SetActive(false);
        difficulty = PlayerPrefs.GetInt("Difficulty", 0);
        #if !UNITY_EDITOR
        indexToTest = -1;
        gamePlaying = false;
        pause = false;
        editing = false;
        #endif

        if (musicAndData != null)
        {
            if (musicAndData.EnterInCustomLevel)
            {
                custom = true;
                musicAndData.EnterInCustomLevel = false;
                music.clip = musicAndData.clips[PlayerPrefs.GetInt(difficulty + "CustomMusic", 0)];
                spawn.attacks.Clear();
                for (int i = 0; i < musicAndData.spawns.Length; i++)
                {
                    spawnsSlected[i] = PlayerPrefs.GetInt(difficulty + "CustomSpawn" + i);
                    if (spawnsSlected[i] == 1 ? true : false)
                        for (int j = 0; j < musicAndData.spawns[i].attacks.Length; j++)
                        {
                            spawn.attacks.Add(musicAndData.spawns[i].attacks[j]);
                        }

                    spawnsString += spawnsSlected[i];
                }
                if (spawnsSlected[3] == 1) 
                {
                    player.axis = PlayerController.Axis.Fourth;
                    if (spinn != null)
                    {
                        spinn.sprites[2].enabled = true;
                        spinn.sprites[3].enabled = true;
                    }
                }
                else 
                {
                    player.axis = PlayerController.Axis.Two;
                    if (spinn != null)
                    {
                        spinn.sprites[2].enabled = false;
                        spinn.sprites[3].enabled = false;
                    }
                }
                bestScore = PlayerPrefs.GetInt(difficulty + "CustomBestScore" + spawnsString, 0);
                score = PlayerPrefs.GetInt(difficulty + "CustomLastScore" + spawnsString, 0);
            }
            else 
            {
                custom = false;
                bestScore = PlayerPrefs.GetInt(difficulty + "BestScore" + level, 0);
                score = PlayerPrefs.GetInt(difficulty + "LastScore" + level, 0);
                savedScore = PlayerPrefs.GetInt(difficulty + "SavedScore" + level, 0);
                if (savedScore != 0) ui.SetColor(1, 1);
                if (bestScore >= 1000) ui.SetColor(0, 1);
            }
        }

        OverrideInspector();
    }

    void Update ()
    {
        if (gamePlaying && !pause)
        {
            time = Time.time - timeBeforePlay;
            score = (time * 10) + savedScore;
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
                if (difficulty == 1 ? true : false) 
                    spawnItem.spawnTime = (spawnItem.spawnTime / 2) - (arrowSpanwDistance / spawnItem.speed) + 0.5f;
                else spawnItem.spawnTime -= arrowSpanwDistance / spawnItem.speed;
                spawnsFixedTime[index].Add(spawnItem);
                //print("override");
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
            //print("while");
            int newIndex = 0;
            if (indexToTest >= 0 && indexToTest < indexCount) newIndex = indexToTest;
            else newIndex = Random.Range(0, indexCount);
            print(newIndex);
            int inverse = Random.Range(0, 2);
            int perpendicular = Random.Range(0, 2);
            int mirror = Random.Range(0, 2);

            float attackSpawnTime = spawnsFixedTime[newIndex][0].spawnTime;
            if (lastCollision - time > -attackSpawnTime && time > 0)
            {
                //print("attackDelay " + (lastCollision - attackSpawnTime));
                yield return new WaitForSeconds(lastCollision - time + attackSpawnTime);
            }

            firstSpawn = time;
            lastCollision = 0;
            if (attackSpawnTime < 0) firstSpawn -= attackSpawnTime;
            foreach (SpawnController.Spawn i in spawnsFixedTime[newIndex])
            {
                if (i.spawnTime > time - firstSpawn)
                {
                    //print("spawnDelay " + (i.spawnTime - (time - firstSpawn)));
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
                //print (i.arrow + " " + time);
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
            music.Play();
            DestroyAllBullets();
            StartCoroutine("Attack");
        }
        else
        {
            music.Pause();
            StopCoroutine("Attack");
        }
        ui.Play(active);
    }

    public void Pause ()
    {
        pause = !pause;
        if (pause) 
        {
            music.Pause();
            Time.timeScale = 0;
        }
        else 
        {
            Time.timeScale = 1;
            music.Play();
        }
        ui.Pause(pause);
    }

    //game events
    public void Damage ()
    {
        if (editing || !gamePlaying) return;
        if (custom)
        {
            PlayerPrefs.SetInt(difficulty + "CustomBestScore" + spawnsString, (int)bestScore);
            PlayerPrefs.SetInt(difficulty + "CustomLastScore" + spawnsString, (int)score);
        }
        else
        {
            if (PlayerPrefs.GetInt(difficulty + "CompletedLevels", -1) < level && bestScore >= 1000)
            {
                savePointsText.SetActive(false);
                completedText.SetActive(true);
                ui.SetColor(0, 1);
                PlayerPrefs.SetInt(difficulty + "CompletedLevels", level);
                if (savedScore != 0)
                {
                    savedScore = 0;
                    PlayerPrefs.SetInt(difficulty + "SavedScore" + level, savedScore);
                    ui.SetColor(1, 0);
                }
                if (level == 3)
                {
                    PlayerPrefs.SetInt(difficulty + "BestScore" + level, (int)bestScore);
                    PlayerPrefs.SetInt(difficulty + "LastScore" + level, (int)score);

                    StartCoroutine(ui.EndingSFXPlay());
                    scene.ChangeSceneSmooth("text_scene");
                }
            }
            else 
            {
                #if UNITY_ANDROID || UNITY_EDITOR
                if (score > 250 && savedScore == 0) savePointsText.SetActive(true);
                else 
                {
                    savedScore = 0;
                    savePointsText.SetActive(false);
                    ui.SetColor(1, 0);
                    PlayerPrefs.SetInt(difficulty + "SavedScore" + level, savedScore);
                }
                #endif
                completedText.SetActive(false);
            }
            PlayerPrefs.SetInt(difficulty + "BestScore" + level, (int)bestScore);
            PlayerPrefs.SetInt(difficulty + "LastScore" + level, (int)score);
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

    void DestroyAllBullets ()
    {
        ArrowController[] actualArrows = GameObject.FindObjectsOfType<ArrowController>();
        foreach (ArrowController item in actualArrows)
        {
            item.MoveAndDestroy();
        }
    }

    public void StopGame ()
    {
        music.Stop();
        ArrowController[] actualArrows = GameObject.FindObjectsOfType<ArrowController>();
        foreach (ArrowController item in actualArrows)
        {
            item.StopAllCoroutines();
        }
    }
}
