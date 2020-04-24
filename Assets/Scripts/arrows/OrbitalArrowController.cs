using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalArrowController : ArrowController
{

    [Header("Special Values")]
    public float orbitalDistance;
    public float colisionTime;

    TrailRenderer trail;

    bool onParent = false;

    override protected void Awake()
    {
        base.Awake();
        trail = GetComponentInChildren<TrailRenderer>();
        trail.enabled = false;
    }

    protected override IEnumerator Move () 
    {
        Vector2 target = rb.position.normalized * orbitalDistance;
        while (rb.position != target)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime));
            yield return null;
        }
        GameObject pivot = new GameObject("Pivot");
        transform.parent = pivot.transform;
        onParent = true;
        float orbitalTime = 0f;
        float orbitalDistanceTime = 0f;
        while (orbitalTime < 1f)
        {
            orbitalTime = Mathf.InverseLerp(0f, orbitalDistance, orbitalDistanceTime);
            orbitalDistanceTime += speed * Time.deltaTime;
            //orbitalTime += Time.deltaTime * secondSpeed * secondSpeedModifier;
            pivot.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, 360f, orbitalTime));
            yield return null;
        }
        trail.enabled = true;
        RaycastHit2D hit = Physics2D.Raycast(rb.position/*-rb.position.normalized * 0.2f*/, -rb.position.normalized);
        rb.MovePosition(hit.point);
        for (float t = 0f; t < colisionTime; t += Time.deltaTime)
        {
            yield return null;
        }
        trail.enabled = false;
    }

    /*public override void DestroyArrow ()
    {
        box.enabled = false;
        if (useAnim) anim.SetTrigger("Destroy");
        //anim.speed = speed / 2;
        StopAllCoroutines();
        StartCoroutine("Destroy");
    }*/

    protected override IEnumerator Destroy ()
    {
        for (float i = 0; i < 0.5f; i += Time.deltaTime/* * speed*/)
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
