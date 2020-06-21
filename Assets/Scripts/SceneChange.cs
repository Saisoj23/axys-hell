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
        StartCoroutine(SceneLoad());

    }

    IEnumerator SceneLoad ()
    {
        anim.SetTrigger("Black");
        yield return new WaitForSeconds(1f);
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
