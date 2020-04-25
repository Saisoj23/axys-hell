﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{

    protected Rigidbody2D rb;
    protected BoxCollider2D box;
    protected Animator anim;
    protected SpriteChange spriteChange;

    [Header("Generic Values")]
    public float speed;
    public float secondSpeed;
    public float finalSpeed;
    float[] originalValues;
    public Color stopColor;

    protected Vector2 inicialPos;
    protected Color inicialColor;
    protected bool visible = false;
    protected bool useAnim = true;

    protected virtual void Awake ()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        spriteChange = GetComponent<SpriteChange>();
    }

    void Start ()
    {    
        inicialPos = rb.transform.position;
        rb.MoveRotation(Vector2.SignedAngle(Vector2.right, inicialPos.normalized));
        StartCoroutine("Move"); 
    }

    public void StartValues (float speed, float secondSpeed, int sprite)
    {
        this.speed = speed;
        this.secondSpeed = secondSpeed;
    }

    protected virtual IEnumerator Move ()
    {
        while (rb.position != Vector2.zero)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, Vector2.zero, speed * Time.deltaTime));
            yield return null;
        }
    }

    public void Pause (bool pause)
    {
        if (pause)
        {
            originalValues = new float[] {speed, secondSpeed, finalSpeed};
            speed = 0f;
            secondSpeed = 0f;
            finalSpeed = 0f;
            anim.speed = 0f;
        }
        else
        {
            speed = originalValues[0];
            secondSpeed = originalValues[1];
            finalSpeed = originalValues[2];
            anim.speed = 1f;
        }
    }

    public virtual void Stop ()
    {
        spriteChange.ChangeSprite(1);
        StopAllCoroutines();
    }

    public bool CollisionWithShield ()
    {
        RaycastHit2D hit = Physics2D.Raycast(rb.position.normalized, -rb.position.normalized);
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

    protected virtual IEnumerator Destroy ()
    {
        for (float i = 0; i < 0.5f; i += Time.deltaTime)
        {
            yield return null;
        }
        Destroy(gameObject);
    }

    public virtual void DestroyArrow ()
    {
        box.enabled = false;
        if (useAnim) anim.SetTrigger("Destroy");
        StopAllCoroutines();
        StartCoroutine("Destroy");
    }

    void OnBecameVisible ()
    {
        visible = true;
    }
}
