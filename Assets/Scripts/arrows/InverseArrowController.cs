using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseArrowController : ArrowController
{

    public float turnDistance;

    override protected IEnumerator Move ()
    {
        box.enabled = false;
        Vector2 target = rb.position.normalized * turnDistance;
        while (rb.position != target)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime));
            yield return null;
        }
        speed *= 3;
        target = -target;
        while (rb.position != target)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime));
            yield return null;
        }
        box.enabled = true;
        while (rb.position != Vector2.zero)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, Vector2.zero, (speed * 1.1f) * Time.deltaTime));
            yield return null;
        }
    }
}
