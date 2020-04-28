using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseArrowController : ArrowController
{
    override protected IEnumerator Move ()
    {
        box.enabled = false;
        Vector2 target = rb.position.normalized * actionDistance;
        while (rb.position != target)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime));
            yield return null;
        }
        target = -target;
        spriteChange.ChangeSprite(2);
        spriteChange.ChangeOrder(-3);
        while (rb.position != target)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * 3 * Time.deltaTime));
            yield return null;
        }
        box.enabled = true;
        spriteChange.ChangeSprite(0);
        spriteChange.ChangeOrder(0);
        while (rb.position != Vector2.zero)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, Vector2.zero, speed * 3 * Time.deltaTime));
            yield return null;
        }
    }
}
