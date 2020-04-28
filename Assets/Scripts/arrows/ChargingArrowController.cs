using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingArrowController : ArrowController
{
    [Header("Special Values")]
    public float laserTime;

    ShieldController shield;
    PlayerController player;
    SpriteRenderer chargingSpr;
    SpriteRenderer laserSpr;
    SpriteChange chargingChange;
    SpriteChange laserChange;
    public GameObject charging;
    public GameObject laser;

    override protected void Awake ()
    {
        base.Awake();     
        chargingSpr = charging.GetComponent<SpriteRenderer>();
        laserSpr = laser.GetComponent<SpriteRenderer>();
        chargingChange = charging.GetComponent<SpriteChange>();
        laserChange = laser.GetComponent<SpriteChange>();
        player = FindObjectOfType<PlayerController>();
        shield = FindObjectOfType<ShieldController>();
    }

    protected override IEnumerator Move ()
    {
        Vector2 target = rb.position.normalized * actionDistance;
        while (rb.position != target)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * 2 * Time.deltaTime));
            yield return null;
        }
        Vector3 startChargePosition = charging.transform.position;
        actionDistance += (inicialPos.magnitude - actionDistance) / 2;
        float chargeTime = 0f;
        float chargeDistanceTime = 0f;
        while (chargeTime < 1)
        {
            chargeTime = Mathf.InverseLerp(0f, actionDistance, chargeDistanceTime);
            chargeDistanceTime += speed * Time.deltaTime; 
            charging.transform.position = Vector3.Lerp(startChargePosition, transform.position + Vector3.forward, chargeTime);
            yield return null;
        }
        RaycastHit2D hit = Physics2D.Raycast(rb.position, -rb.position.normalized);
        bool isShield = hit.collider.CompareTag("Shield");
        if (isShield) 
        {
            laser.transform.localScale = new Vector3((Mathf.Abs(laser.transform.position.x + laser.transform.position.y) - 0.375f) * 2, 1, 1);
        }
        else 
        {
            laser.transform.localScale = new Vector3(Mathf.Abs(laser.transform.position.x + laser.transform.position.y) * 2, 1, 1);
        }
        laserSpr.enabled = true;
        if (isShield) shield.Defend();
        else player.Hurt();
        for (float t = 0f; t < laserTime; t += Time.deltaTime)
        {
            laserSpr.color = new Color(1f, 1f, 1f, Mathf.InverseLerp(laserTime, 0f, t));
            laser.transform.localScale = new Vector3(laser.transform.localScale.x, Mathf.InverseLerp(0, laserTime, t) + 0.5f, 1f);
            yield return null;
        }
        laserSpr.enabled = false;
        DestroyArrow();
    }

    public override void MoveAndDestroy ()
    {
        laserSpr.enabled = false;
        if (visible)
        StartCoroutine("MoveToCenter");
        else
        {
            useAnim = false;
            DestroyArrow();
        }
    }

    public override void Stop ()
    {
        spriteChange.ChangeSprite(1);
        laserChange.ChangeSprite(1);
        chargingChange.ChangeSprite(1);
        StopAllCoroutines();
    }

    public override void DestroyArrow ()
    {
        chargingSpr.enabled = false;
        box.enabled = false;
        if (useAnim) anim.SetTrigger("Destroy");
        StopAllCoroutines();
        StartCoroutine("Destroy");
    }
}
