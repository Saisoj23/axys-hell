using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartArrowController : ArrowController
{
    bool visible = false;
    protected override IEnumerator MoveTo ()
    {
        bool looking = false;
        while (rb.position != Vector2.zero)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position -rb.position.normalized * 0.2f, -rb.position.normalized);
            looking = hit.collider.CompareTag("Shield");
            if (!visible || (visible && !looking))
            {
                rb.MovePosition(Vector2.MoveTowards(rb.position, Vector2.zero, speed * Time.deltaTime));
            }
            yield return null;
        }
    }

    void OnBecameVisible ()
        {
            visible = true;
        }
}
