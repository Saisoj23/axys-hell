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

    void OnCollisionEnter2D (Collision2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            col.gameObject.GetComponent<ArrowController>().DestroyArrow();
            Defend();
        } 
        else if (col.gameObject.CompareTag("Anti Bullet"))
        {
            col.gameObject.GetComponent<ArrowController>().DestroyArrow();
            player.Hurt();
        }
        else if (col.gameObject.CompareTag("False Bullet"))
        {
            col.gameObject.GetComponent<ArrowController>().DestroyArrow();
        }
    }

    public void Defend ()
    {
        anim.SetTrigger("Hurted");
    }
}
