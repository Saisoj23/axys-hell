using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    bool canContinue = true;
    int indexMenu = 0;
    int indexCount;
    
    public Animator[] levelAnim;

    Camera cam;
    SceneChange scene;

    public float minTouchSpeed;
    public float continueColddown;

    Vector2 direccion;
    Vector2 screenMiddle = new Vector2(Screen.width, Screen.height) / 2;
    bool lastPresed;
    Vector2 lastMousePosition;
    Vector2 actualMousePosition;

    void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        scene = GameObject.FindObjectOfType<SceneChange>();
        indexCount = levelAnim.Length;
        levelAnim[0].SetTrigger("InitialEnable");
        levelAnim[0].SetTrigger("InitialUnbloqued");
        levelAnim[2].SetTrigger("InitialUnbloqued");
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

        canContinue = false;
        StartCoroutine(ContinueColddown());
    }

    public void Play ()
    {
        if (!canContinue) return;
        switch (indexMenu)
        {
            case 0: 
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
