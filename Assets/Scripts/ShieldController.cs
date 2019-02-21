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
            Destroy(col.gameObject);
            Defend();
        } else if (col.gameObject.CompareTag("Anti Bullet"))
        {
            Destroy(col.gameObject);
            player.Hurt();
        }
    }

    public void Defend ()
    {
        anim.SetTrigger("Hurted");
        Debug.Log("defendido");
    }
}
