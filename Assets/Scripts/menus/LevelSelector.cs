using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    bool canContinue = true;
    int indexMenu = 0;
    int indexCount;
    
    public Animator[] levelAnim;
    public TMPro.TMP_Text[] bestScores;
    public TMPro.TMP_Text[] lastScores;
    public Color[] textColor;

    Camera cam;
    SceneChange scene;
    CustomLevel custom;
    MusicAndData musicAndData;

    public float minTouchSpeed;
    public float continueColddown;

    int difficulty;
    int completedLevels;
    int unbloquedLevels;

    Vector2 screenMiddle = new Vector2(Screen.width, Screen.height) / 2;
    bool lastPresed;
    Vector2 lastMousePosition;
    Vector2 actualMousePosition;

    void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        scene = FindObjectOfType<SceneChange>();
        musicAndData = FindObjectOfType<MusicAndData>();
        custom = FindObjectOfType<CustomLevel>();
        indexCount = levelAnim.Length;

        if (musicAndData != null) musicAndData.PlayMusic(true);

        difficulty = PlayerPrefs.GetInt("Difficulty", 0);
        for (int i = 0; i < bestScores.Length; i++)
        {
            int bestScore = (PlayerPrefs.GetInt(difficulty + "BestScore" + i, 0));
            bestScores[i].text = bestScore.ToString();
            if (bestScore >= 1000) bestScores[i].color = textColor[0];
        }
        for (int i = 0; i < lastScores.Length; i++)
        {
            lastScores[i].text = (PlayerPrefs.GetInt(difficulty + "LastScore" + i, 0)).ToString();
            if (PlayerPrefs.GetInt(difficulty + "SavedScore" + i, 0) != 0) lastScores[i].color = textColor[1];
        }

        indexMenu = PlayerPrefs.GetInt("LastOpenLevel", 0);
        levelAnim[indexMenu].SetTrigger("InitialEnable");

        completedLevels = PlayerPrefs.GetInt(difficulty + "CompletedLevels", -1);
        unbloquedLevels = PlayerPrefs.GetInt(difficulty + "UnbloquedLevels", 0);

        for (int i = 0; i < levelAnim.Length - 1; i++)
        {
            if (i <= unbloquedLevels) levelAnim[i].SetTrigger("InitialUnbloqued");
        }
        levelAnim[levelAnim.Length - 1].SetTrigger("InitialUnbloqued");

        if (completedLevels == unbloquedLevels && unbloquedLevels < levelAnim.Length - 2)
        {
            unbloquedLevels++;
            PlayerPrefs.SetInt(difficulty + "UnbloquedLevels", unbloquedLevels);
            levelAnim[indexMenu].SetTrigger("ExitLeft");
            levelAnim[unbloquedLevels].SetTrigger("UnblockRight");
            indexMenu++;
            canContinue = false;
            PlayerPrefs.SetInt("LastOpenLevel", indexMenu);
            StartCoroutine(ContinueColddown());
        }
    }

    void Update()
    {
        Vector2 input = Vector2.zero;
        RaycastHit hit;
        if (Input.GetMouseButton(0) && Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 20f)) 
        {
            if (hit.collider.CompareTag("Background"))
            {
                actualMousePosition = hit.point;
                if (lastMousePosition != null && lastPresed && Vector2.Distance(actualMousePosition, lastMousePosition) * Time.deltaTime > minTouchSpeed)
                {
                    input = -(lastMousePosition - actualMousePosition).normalized; 
                }
                lastMousePosition = actualMousePosition;
                lastPresed = true;
            }
        }
        else lastPresed = false;
        
        Vector2 axisInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        if (axisInput.magnitude != 0 && Physics.Raycast(cam.ScreenPointToRay(screenMiddle + (axisInput.normalized * 100)), out hit, 20f))
        {
            input = hit.point.normalized; 
        }

        if (input.x != 0 && Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            if (input.x > 0) Prev();
            else Next();
        }
    }
    
    public void Next()
    {
        if (!canContinue) return;
        levelAnim[indexMenu].SetTrigger("ExitLeft");
        indexMenu++;
        if (indexMenu >= indexCount) indexMenu = 0;
        levelAnim[indexMenu].SetTrigger("EntryRight");

        PlayerPrefs.SetInt("LastOpenLevel", indexMenu);
        canContinue = false;
        StartCoroutine(ContinueColddown());
    }

    public void Prev()
    {
        if (!canContinue) return;
        levelAnim[indexMenu].SetTrigger("ExitRight");
        indexMenu--;
        if (indexMenu < 0) indexMenu = indexCount - 1;
        levelAnim[indexMenu].SetTrigger("EntryLeft");

        PlayerPrefs.SetInt("LastOpenLevel", indexMenu);
        canContinue = false;
        StartCoroutine(ContinueColddown());
    }

    public void Play ()
    {
        if (!canContinue || (indexMenu > unbloquedLevels && indexMenu != levelAnim.Length - 1)) return;
        print("firstIfPassed");
        switch (indexMenu)
        {
            case 0: 
                if (PlayerPrefs.GetInt(difficulty + "Progress", 0) == 0)
                {
                    scene.ChangeScene("text_scene");
                    break;
                }
                scene.ChangeScene("level_normal");
                break;
            case 1: 
                scene.ChangeScene("level_ice");
                break;
            case 2: 
                scene.ChangeScene("level_fire");
                break;
            case 3: 
                scene.ChangeScene("level_space");
                break;
            case 4:
                if (custom.canEnter)
                {
                    print("secondIfPassed");
                    musicAndData.EnterInCustomLevel = true;
                    indexMenu = custom.themeActive;
                    Play();
                }
                break;
            default:
                break;
        }

    }
    
    IEnumerator ContinueColddown ()
    {
        yield return new WaitForSeconds(continueColddown);
        canContinue = true;
    }
}
