using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnCamera : MonoBehaviour
{

    Camera cam;
    GameController game;
    public SpriteRenderer[] sprites;
    bool lastPlaying = false;
    bool spinning = false;

    public float fadeOnSpeed;
    public float fadeOffSpeed;
    public float sipinningFadeSpeed;
    public float spinnSpeed;
    public float restarSpinnSpeed;
    public float score;

    void Awake()
    {
        cam = GameObject.FindObjectOfType<Camera>();
        game = GameObject.FindObjectOfType<GameController>();
        foreach (SpriteRenderer item in sprites)
        {
            item.color = new Color(1,1,1,0);
        }
    }

    void Update()
    {
        if (!lastPlaying && game.gamePlaying)
        {
            StartCoroutine(FadeOn());
        }
        else if (!game.gamePlaying && lastPlaying)
        {
            StartCoroutine(FadeOff());
            if (spinning)
            {
                StopCoroutine(Spin());
                StartCoroutine(RestarSpin());
            }
        }
        if (game.gamePlaying && game.score > score && !spinning && lastPlaying)
        {
            StartCoroutine(Spin());
        }
        lastPlaying = game.gamePlaying;
    }

    /*public void Fade(bool active)
    {
        if (active)
        {
            StartCoroutine(FadeOn());
        }
        else 
        {
            StartCoroutine(FadeOff());
        }
    }*/

    IEnumerator FadeOn ()
    {
        for (float i = 0f; i < 1f; i += Time.deltaTime * fadeOnSpeed)
        {
            foreach (SpriteRenderer item in sprites)
            {
                item.color = new Color(1,1,1,i);
            }
            yield return null;
        }
        foreach (SpriteRenderer item in sprites)
        {
            item.color = new Color(1,1,1,1);
        }
    }

    IEnumerator FadeOff ()
    {
        for (float i = 1f; i > 0f; i -= Time.deltaTime * fadeOffSpeed)
        {
            foreach (SpriteRenderer item in sprites)
            {
                item.color = new Color(1,1,1,i);
            }
            yield return null;
        }
        foreach (SpriteRenderer item in sprites)
        {
            item.color = new Color(1,1,1,0);
        }
    }

    IEnumerator Spin ()
    {
        spinning = true;
        float timeFade = 0f;
        while (game.gamePlaying)
        {
            cam.transform.eulerAngles += new Vector3(0f, 0f, spinnSpeed * Time.deltaTime);
            timeFade += Time.deltaTime * sipinningFadeSpeed;
            foreach (SpriteRenderer item in sprites)
            {
                item.color = new Color(1,1,1,Mathf.Abs(Mathf.Cos(timeFade)));
            }
            yield return null;
        }
    }

    IEnumerator RestarSpin ()
    {
        spinning = false;
        Quaternion target = Quaternion.Euler(0f, 0f, Mathf.Round(cam.transform.eulerAngles.z / 90) * 90);
        while (cam.transform.rotation != target)
        {
            cam.transform.rotation = Quaternion.RotateTowards(cam.transform.rotation, target, restarSpinnSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
