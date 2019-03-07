using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public enum Axis {One, Two, Fourth};
    public Axis axis;

    public float rotateSpeed;
    public float minTouchSpeed;

    Vector2 direccion;
    Vector2 screenMiddle = new Vector2(Screen.width, Screen.height) / 2;
    bool lastPresed;
    Vector2 lastMousePosition;
    Vector2 actualMousePosition;
    float timeTurning;
    float lastAngle;
    Rigidbody2D rb;
    Animator anim;
    ShieldController shield;
    GameController game;

    void Awake ()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        shield = GetComponentInChildren<ShieldController>();
        game = GameObject.FindObjectOfType<GameController>();
    }

    void Update ()
    {
        Vector2 input = Vector2.zero;
        #if UNITY_EDITOR || UNITY_ANDROID
        if (Input.GetMouseButton(0)) 
        {
            actualMousePosition = Input.mousePosition;
            if (lastMousePosition != null && lastPresed && Vector2.Distance(actualMousePosition, lastMousePosition) * Time.deltaTime > minTouchSpeed)
            {
                input = -(lastMousePosition - actualMousePosition).normalized; 
            }
            lastMousePosition = actualMousePosition;
            lastPresed = true;
        }
        else lastPresed = false;
        #endif
        
        #if UNITY_EDITOR || !UNITY_ANDROID
        if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
        }
        #endif

        switch (axis)
        {
            case Axis.One:
                direccion = new Vector2(Mathf.Round(input.x), 0f);
                break;
            case Axis.Two:
                if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                {
                    direccion = new Vector2(Mathf.Round(input.x), 0f);
                } else
                {
                    direccion = new Vector2(0f, Mathf.Round(input.y));
                }
                break;
            default:
                direccion = new Vector2(Mathf.Round(input.x), Mathf.Round(input.y)).normalized;
                break;
        }
    }

    void FixedUpdate ()
    {

        if (direccion != Vector2.zero)
        {
            lastAngle = Vector2.SignedAngle(Vector2.right, direccion);
        }
        if (rb.rotation != lastAngle)
        {
            transform.eulerAngles = new Vector3(0f, 0f, Mathf.MoveTowardsAngle(rb.rotation, lastAngle, rotateSpeed * Time.deltaTime * game.speedByTime));
        }
    }

    void OnCollisionEnter2D (Collision2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            col.gameObject.GetComponent<ArrowController>().DestroyArrow();
            Hurt();
        }
        if (col.gameObject.CompareTag("Anti Bullet"))
        {
            col.gameObject.GetComponent<ArrowController>().DestroyArrow();
            shield.Defend();
        }
    }

    public void Hurt ()
    {
        if (game.editing)
        {
            anim.SetTrigger("Hurted");
        }
        else 
        {
            game.Damage();
        }
    }
}
