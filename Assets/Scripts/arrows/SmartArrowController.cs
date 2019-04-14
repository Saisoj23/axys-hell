using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartArrowController : ArrowController
{
    [Header("Special Values")]
    public float smartDistance;
    public Color lookingColor;

    public GameObject line;
    SpriteRenderer lineSpr;

    override protected void Awake ()
    {
        base.Awake();
        lineSpr = line.GetComponent<SpriteRenderer>();
    }

    protected override IEnumerator Move ()
    {
        Vector2 target = rb.position.normalized * smartDistance;
        while (rb.position != target)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime));
            yield return null;
        }
        bool looking = false;
        lineSpr.enabled = true;
        float yScale = line.transform.localScale.y;
        Color startColor = spr.color;
        while (rb.position != Vector2.zero)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position -rb.position.normalized * 0.2f, -rb.position.normalized);
            line.transform.localScale = new Vector3(Mathf.Abs(transform.position.x + transform.position.y) * 2.1f, yScale, 1);
            looking = hit.collider.CompareTag("Shield");
            if (looking)
            {
                line.transform.localPosition = Vector3.forward / 2;
                lineSpr.color = lookingColor;
                spr.color = lookingColor;
            }
            else
            {
                line.transform.localPosition = -Vector3.forward / 2;
                lineSpr.color = startColor;
                spr.color = startColor;
                rb.MovePosition(Vector2.MoveTowards(rb.position, Vector2.zero, secondSpeed * Time.deltaTime));
            }
            yield return null;
        }
    }

    public override void MoveAndDestroy ()
    {
        lineSpr.enabled = false;
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
        lineSpr.enabled = false;
        box.enabled = false;
        if (useAnim) anim.SetTrigger("Destroy");
        anim.speed = speed / 2;
        StopAllCoroutines();
        StartCoroutine("Destroy");
    }

    public override void Stop ()
    {
        spr.color = stopColor;
        lineSpr.color = stopColor;
        StopAllCoroutines();
    }
}
