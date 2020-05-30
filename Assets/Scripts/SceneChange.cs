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
        nextScene = scene;
        StartCoroutine(SceneLoad());

    }

    IEnumerator SceneLoad ()
    {
        anim.SetTrigger("Black");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(nextScene);
    }
}
