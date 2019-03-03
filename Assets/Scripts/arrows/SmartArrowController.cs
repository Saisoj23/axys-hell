using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartArrowController : ArrowController
{
    [Header("Special Values")]
    public float smartDistance;
    public Color lookingColor;

    bool visible = false;

    SpriteRenderer sprite;
    LineRenderer line; 

    override protected void Awake ()
    {
        base.Awake();
        sprite = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();
    }

    protected override IEnumerator MoveTo ()
    {
        Vector2 target = rb.position.normalized * smartDistance;
        while (rb.position != target)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime));
            yield return null;
        }
        bool looking = false;
        line.enabled = true;
        Color startColor = sprite.color;
        while (rb.position != Vector2.zero)
        {
            line.SetPositions(new Vector3[] {transform.position, new Vector3(0f, 0f, 0.11f)});
            RaycastHit2D hit = Physics2D.Raycast(rb.position -rb.position.normalized * 0.2f, -rb.position.normalized);
            looking = hit.collider.CompareTag("Shield");
            if (looking)
            {
                line.startColor = lookingColor;
                line.endColor = lookingColor;
                sprite.color = lookingColor;
            }
            else
            {
                line.startColor = startColor;
                line.endColor = startColor;
                sprite.color = startColor;
                rb.MovePosition(Vector2.MoveTowards(rb.position, Vector2.zero, speed * Time.deltaTime));
            }
            yield return null;
        }
    }
}
