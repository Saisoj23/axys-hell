using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SkipIntro ()
    {
        print("Skip");
        anim.SetTrigger("Skip");
    }
}
