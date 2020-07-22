using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    Animator anim;
    AudioSource sound;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
    }

    public void SkipIntro ()
    {
        print("Skip");
        anim.SetTrigger("Skip");
    }

    public void PlayEffect ()
    {
        sound.Play();
    }
}
