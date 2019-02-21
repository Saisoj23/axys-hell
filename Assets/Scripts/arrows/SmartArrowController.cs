using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartArrowController : ArrowController
{
    protected override IEnumerator MoveTo ()
    {
        bool looking;
        while (rb.position != Vector2.zero)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position -rb.position.normalized * 0.2f, -rb.position.normalized);
            looking = hit.collider.CompareTag("Shield");
            Debug.Log(hit.collider);
            Debug.Log(looking);
            if (!looking)
            {
                rb.MovePosition(Vector2.MoveTowards(rb.position, Vector2.zero, speed * Time.deltaTime));
            }
            yield return null;
        }
    }
}
