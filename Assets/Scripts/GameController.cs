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
    public int startSpin;

    [Header("UI objects")]
    public TMPro.TMP_Text timeText;
    public TMPro.TMP_Text bestText;
    public TMPro.TMP_Text tittleText;
    public TMPro.TMP_Text subTittleText;
    public Image bestImage;
    public Image timeImage;
    public Image pauseImage;
    public Image[] nextImage;
    public Button playButton;
    public Button[] nextButton;
    public SpriteRenderer[] lines;
    public Spin spin;

    [Header("Ui Resources")]
    public Sprite[] nextSprites;
    public Sprite[] bestSprites;
    public Sprite[] timeSprites;
    public Sprite[] pauseSprites;
    public TMPro.TMP_FontAsset[] fonts;
    public Color[] backgroundColors;

    [Header("Game Status")]
    public bool gamePlaying = false;
    public bool editing = false;
    public int level = 0;
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
            if (time > startSpin && !spining) 
            {
                spin.StartCoroutine("StartSpin");
                anim.SetTrigger("Spining");
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
                arrow.ChangeSprite(level);
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
        nextButton[0].enabled = !active;
        nextButton[1].enabled = !active;
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

    public void ChangeLevel (bool next)
    {
        level += next ? 1 : -1;
        if (level > 2) level = 0;
        else if (level < 0) level = 2;

        ArrowController[] actualArrows = GameObject.FindObjectsOfType<ArrowController>();
        foreach (ArrowController item in actualArrows)
        {
            item.ChangeSprite(level);
        }
        shield.ChangeSprite(level);
        player.ChangeSprite(level);
        bestImage.sprite = bestSprites[level];
        timeImage.sprite = timeSprites[level];
        pauseImage.sprite = pauseSprites[level];
        nextImage[0].sprite = nextSprites[level];
        nextImage[1].sprite = nextSprites[level];
        bestText.font = fonts[level];
        timeText.font = fonts[level];
        tittleText.font = fonts[level];
        subTittleText.font = fonts[level];
        cam.backgroundColor = backgroundColors[level];

        if (level == 1)
        {
            Debug.Log("deberia verse negro");
            nextImage[0].color = backgroundColors[0];
            nextImage[1].color = backgroundColors[0];
            pauseImage.color = backgroundColors[0];
            foreach (SpriteRenderer item in lines)
            {
                item.color = backgroundColors[0];
            }
        }
        else
        {
            nextImage[0].color = Color.white;
            nextImage[1].color = Color.white;
            pauseImage.color = Color.white;
            foreach (SpriteRenderer item in lines)
            {
                item.color = Color.white;
            }
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
