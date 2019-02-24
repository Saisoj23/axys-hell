using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingArrowController : ArrowController
{
    
    public float chargeDistance;
    public float chargeSpeed;
    public float laserTime;

    SpriteRenderer sprite;
    LineRenderer line;
    ShieldController shield;
    PlayerController player;

    override protected void Awake ()
    {
        base.Awake();
        sprite = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();
        player = FindObjectOfType<PlayerController>();
        shield = FindObjectOfType<ShieldController>();
    }

    protected override IEnumerator MoveTo ()
    {
        Vector2 target = rb.position.normalized * chargeDistance;
        while (rb.position != target)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime));
            yield return null;
        }
        Color startColor = sprite.color;
        float colorTime = 0f;
        while (colorTime <= 1)
        {
            colorTime += Time.deltaTime * chargeSpeed;
            sprite.color = Color.Lerp(startColor, Color.white, colorTime);
            yield return null;
        }
        RaycastHit2D hit = Physics2D.Raycast(rb.position -rb.position.normalized * 0.2f, -rb.position.normalized);
        bool isShield = hit.collider.CompareTag("Shield");
        line.SetPositions(new Vector3[] {transform.position, new Vector3 (hit.point.x, hit.point.y, 0.11f)});
        line.enabled = true;
        if (isShield) shield.Defend();
        else player.Hurt();
        yield return new WaitForSeconds(laserTime);
        DestroyArrow();
    }
}
