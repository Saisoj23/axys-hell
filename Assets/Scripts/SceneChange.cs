using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    string nextScene;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void ChangeScene(string scene)
    {
        GameObject.Find("EventSystem").SetActive(false);
        nextScene = scene;
        Time.timeScale = 1;
        anim.SetTrigger("Black");
        StartCoroutine(SceneLoad(1f));
    }

    public void ChangeSceneSmooth(string scene)
    {
        GameObject.Find("EventSystem").SetActive(false);
        nextScene = scene;
        Time.timeScale = 1;
        anim.SetTrigger("BlackSmooth");
        StartCoroutine(SceneLoad(6f));
    }

    IEnumerator SceneLoad (float wait)
    {
        yield return new WaitForSeconds(wait);
        SceneManager.LoadScene(nextScene);
    }

    public void Exit ()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
