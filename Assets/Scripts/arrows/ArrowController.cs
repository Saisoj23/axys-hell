using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{

    protected Rigidbody2D rb;
    protected BoxCollider2D box;
    protected Animator anim;
    protected SpriteRenderer spr;

    [Header("Generic Values")]
    public float speed;
    public float secondSpeed;
    public float finalSpeed;

    protected virtual void Awake ()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
    }

    void Start ()
    {
        rb.MoveRotation(Vector2.SignedAngle(Vector2.right, -rb.position.normalized));
        StartCoroutine("Move");
    }

    protected virtual IEnumerator Move ()
    {
        while (rb.position != Vector2.zero)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, Vector2.zero, speed * Time.deltaTime));
            yield return null;
        }
    }

    public virtual void Stop ()
    {
        spr.color = Color.grey;
        StopAllCoroutines();
    }

    public virtual void MoveAndDestroy ()
    {
        StartCoroutine("MoveToCenter");
    }

    protected IEnumerator MoveToCenter ()
    {
        tag = "False Bullet";
        while (rb.position != Vector2.zero)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, Vector2.zero, finalSpeed * Time.deltaTime));
            yield return null;
        }
        DestroyArrow();
    }

    public virtual void DestroyArrow ()
    {
        box.enabled = false;
        anim.SetTrigger("Destroy");
        anim.speed = speed / 2;
        StopAllCoroutines();
        Destroy(gameObject, 0.5f);
    }
}
