using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartArrowController : ArrowController
{
    [Header("Special Values")]
    public float turnSpeed;

    bool onParent = false;

    protected override IEnumerator Move ()
    {
        Vector2 target = rb.position.normalized * actionDistance;
        while (rb.position != target)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
        }
        GameObject pivot = new GameObject("Pivot");
        /*Rigidbody2D pivotRb = pivot.AddComponent<Rigidbody2D>();
        pivotRb.isKinematic = true;*/
        transform.parent = pivot.transform;
        onParent = true;
        bool looking = false;
        bool spining = false;
        float orbitalTime = 0f;
        float initialRot = pivot.transform.eulerAngles.z;
        while (rb.position != Vector2.zero)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position -rb.position.normalized * 0.2f, -rb.position.normalized);
            looking = hit.collider.CompareTag("Shield");
            if (looking && !spining)
            {
                spriteChange.ChangeSprite(2);
                spining = true;
            }
            if  (spining)
            {
                orbitalTime += Time.fixedDeltaTime * turnSpeed;
                pivot.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(initialRot, initialRot + 90, orbitalTime));
                if (orbitalTime >= 1f)
                {
                    spriteChange.ChangeSprite(0);
                    initialRot = pivot.transform.eulerAngles.z;
                    spining = false;
                    orbitalTime = 0;
                }
            }
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, Vector2.zero, speed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
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
