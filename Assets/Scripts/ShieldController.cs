using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{

    Animator anim;

    void Awake ()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D (Collider2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            Destroy(col.gameObject);
            anim.SetTrigger("Hurted");
            Debug.Log("defendido");
        }
    }
}
