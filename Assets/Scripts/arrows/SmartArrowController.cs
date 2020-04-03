﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartArrowController : ArrowController
{
    [Header("Special Values")]
    public float smartDistance;
    public Color lookingColor;
    public float secondSpeedModifier;

    bool onParent = false;

    override protected void Awake ()
    {
        base.Awake();
    }

    protected override IEnumerator Move ()
    {
        Vector2 target = rb.position.normalized * smartDistance;
        while (rb.position != target)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime));
            yield return null;
        }
        GameObject pivot = new GameObject("Pivot");
        transform.parent = pivot.transform;
        //Rigidbody2D pivotRb = pivot.AddComponent<Rigidbody2D>();
        //pivotRb.isKinematic = true;
        bool looking = false;
        Color startColor = spr.color;
        bool spining = false;
        float orbitalTime = 0f;
        float initialRot = pivot.transform.eulerAngles.z;
        while (rb.position != Vector2.zero)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position -rb.position.normalized * 0.2f, -rb.position.normalized);
            looking = hit.collider.CompareTag("Shield");
            if (looking)
            {
                spr.color = lookingColor;
                spining = true;
            }
            else 
            {
                spr.color = startColor;
            }
            if  (spining)
            {
                orbitalTime += Time.deltaTime * secondSpeed * secondSpeedModifier;
                pivot.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(initialRot, initialRot + 90, orbitalTime));
                //rb.MovePosition(Vector2.MoveTowards(rb.position, Vector2.zero, speed * Time.deltaTime));
                //pivotRb.SetRotation(Mathf.Lerp(initialRot, initialRot + 90, orbitalTime));
                if (orbitalTime >= 1f)
                {
                    initialRot = pivot.transform.eulerAngles.z;
                    spining = false;
                    orbitalTime = 0;
                }
            }
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, Vector2.zero, speed * Time.deltaTime);
            print(Vector2.Distance(Vector2.zero, rb.position));
            yield return null;
        }
    }

    public override void MoveAndDestroy ()
    {
        if (visible)
        StartCoroutine("MoveToCenter");
        else
        {
            useAnim = false;
            DestroyArrow();
        }
    }

    public override void DestroyArrow ()
    {
        mask.enabled = false;
        box.enabled = false;
        if (useAnim) anim.SetTrigger("Destroy");
        anim.speed = speed / 2;
        StopAllCoroutines();
        StartCoroutine("Destroy");
    }

    public override void Stop ()
    {
        spr.color = stopColor;
        StopAllCoroutines();
    }

    protected override IEnumerator Destroy ()
    {
        for (float i = 0; i < 0.5f; i += Time.deltaTime * speed)
        {
            yield return null;
        }
        if (onParent)
        {
            Destroy(transform.parent.gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }
}
