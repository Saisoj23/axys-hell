using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{

    Animator anim;
    PlayerController player;

    void Awake ()
    {
        anim = GetComponent<Animator>();
        player = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter2D (Collider2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            col.gameObject.GetComponent<ArrowController>().DestroyArrow();
            Defend();
        } else if (col.gameObject.CompareTag("Anti Bullet"))
        {
            col.gameObject.GetComponent<ArrowController>().DestroyArrow();
            player.Hurt();
        } 
    }

    public void Defend ()
    {
        anim.SetTrigger("Hurted");
    }
}
