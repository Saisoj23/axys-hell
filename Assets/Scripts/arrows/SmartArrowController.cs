using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartArrowController : ArrowController
{
    [Header("Special Values")]
    public float smartDistance;
    public Color lookingColor;

    //LineRenderer line; 
    public GameObject line;
    SpriteRenderer lineSpr;

    override protected void Awake ()
    {
        base.Awake();
        //line = GetComponent<LineRenderer>();
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
        //line.enabled = true;
        lineSpr.enabled = true;
        Color startColor = spr.color;
        while (rb.position != Vector2.zero)
        {
            //line.SetPositions(new Vector3[] {transform.position, new Vector3(0f, 0f, 0.11f)});
            line.transform.localScale = new Vector3(Mathf.Abs(transform.position.x + transform.position.y) * 2.1f, 0.1f, 1);
            RaycastHit2D hit = Physics2D.Raycast(rb.position -rb.position.normalized * 0.2f, -rb.position.normalized);
            looking = hit.collider.CompareTag("Shield");
            if (looking)
            {
                //line.startColor = lookingColor;
                //line.endColor = lookingColor;
                lineSpr.color = lookingColor;
                spr.color = lookingColor;
            }
            else
            {
                //line.startColor = startColor;
                //line.endColor = startColor;
                lineSpr.color = startColor;
                spr.color = startColor;
                rb.MovePosition(Vector2.MoveTowards(rb.position, Vector2.zero, secondSpeed * Time.deltaTime));
            }
            yield return null;
        }
    }

    public override void MoveAndDestroy ()
    {
        //line.enabled = false;
        lineSpr.enabled = false;
        StartCoroutine("MoveToCenter");
    }

    public override void DestroyArrow ()
    {
        mask.enabled = false;
        //line.enabled = false;
        lineSpr.enabled = false;
        box.enabled = false;
        anim.SetTrigger("Destroy");
        anim.speed = speed / 2;
        StopAllCoroutines();
        Destroy(gameObject, 0.5f);
    }

    public override void Stop ()
    {
        spr.color = stopColor;
        //line.startColor = stopColor;
        //line.endColor = stopColor;
        lineSpr.color = stopColor;
        StopAllCoroutines();
    }
}
