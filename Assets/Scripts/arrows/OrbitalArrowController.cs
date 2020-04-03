using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalArrowController : ArrowController
{

    [Header("Special Values")]
    public float orbitalDistance;
    public float secondSpeedModifier;

    bool onParent = false;

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
        do 
        {
            orbitalTime += Time.deltaTime * secondSpeed * secondSpeedModifier;
            pivot.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, 360f, orbitalTime));
            yield return null;
        } while (orbitalTime <= 1f);
        while (rb.position != Vector2.zero)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, Vector2.zero, speed * 3 * Time.deltaTime));
            yield return null;
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
