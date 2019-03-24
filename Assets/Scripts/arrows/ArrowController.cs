using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{

    protected Rigidbody2D rb;
    protected BoxCollider2D box;
    protected Animator anim;
    protected SpriteRenderer spr;
    protected SpriteMask mask;

    [Header("Generic Values")]
    public float speed;
    public float secondSpeed;
    public float finalSpeed;
    public Sprite[] sprites;
    public Color stopColor;

    protected Vector2 inicialDir;
    protected Color inicialColor;
    protected bool visible = false;
    protected bool useAnim = true;

    protected virtual void Awake ()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        mask = GetComponent<SpriteMask>();
    }

    void Start ()
    {    
        inicialDir = rb.transform.position.normalized;
        rb.MoveRotation(Vector2.SignedAngle(Vector2.right, -inicialDir));
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
        spr.color = stopColor;
        StopAllCoroutines();
    }

    public bool CollisionWithShield ()
    {
        RaycastHit2D hit = Physics2D.Raycast(inicialDir, -inicialDir);
        return hit.collider.CompareTag("Shield");
    }

    public virtual void MoveAndDestroy ()
    {
        if (visible)
        StartCoroutine("MoveToCenter");
        else
        {
            useAnim = false;
            DestroyArrow();
        }
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
        mask.enabled = false;
        box.enabled = false;
        if (useAnim) anim.SetTrigger("Destroy");
        anim.speed = speed / 2;
        StopAllCoroutines();
        Destroy(gameObject, 0.5f);
    }

    void OnBecameVisible ()
    {
        Debug.Log("visible");
        visible = true;
    }

    public virtual void ChangeSprite (int sprite)
    {
        spr.sprite = sprites[sprite];
    }
}
