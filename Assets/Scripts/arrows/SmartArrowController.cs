using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartArrowController : ArrowController
{
    [Header("Special Values")]
    public float smartDistance;
    public Color lookingColor;

    LineRenderer line; 

    override protected void Awake ()
    {
        base.Awake();
        line = GetComponent<LineRenderer>();
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
        line.enabled = true;
        Color startColor = spr.color;
        while (rb.position != Vector2.zero)
        {
            line.SetPositions(new Vector3[] {transform.position, new Vector3(0f, 0f, 0.11f)});
            RaycastHit2D hit = Physics2D.Raycast(rb.position -rb.position.normalized * 0.2f, -rb.position.normalized);
            looking = hit.collider.CompareTag("Shield");
            if (looking)
            {
                line.startColor = lookingColor;
                line.endColor = lookingColor;
                spr.color = lookingColor;
            }
            else
            {
                line.startColor = startColor;
                line.endColor = startColor;
                spr.color = startColor;
                rb.MovePosition(Vector2.MoveTowards(rb.position, Vector2.zero, secondSpeed * Time.deltaTime));
            }
            yield return null;
        }
    }

    public override void MoveAndDestroy ()
    {
        line.enabled = false;
        StartCoroutine("MoveToCenter");
    }

    public override void DestroyArrow ()
    {
        mask.enabled = false;
        line.enabled = false;
        box.enabled = false;
        anim.SetTrigger("Destroy");
        anim.speed = speed / 2;
        StopAllCoroutines();
        Destroy(gameObject, 0.5f);
    }

    public override void Stop ()
    {
        spr.color = stopColor;
        line.startColor = stopColor;
        line.endColor = stopColor;
        StopAllCoroutines();
    }
}
