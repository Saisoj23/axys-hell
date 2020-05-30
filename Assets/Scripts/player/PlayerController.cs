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
    SpriteRenderer spr;
    ShieldController shield;
    GameController game;
    Camera cam;
    GameObject rot;

    void Awake ()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        shield = GetComponentInChildren<ShieldController>();
        rot = shield.transform.parent.gameObject;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        game = GameObject.FindObjectOfType<GameController>();
    }

    void Update ()
    {
        Vector2 input = Vector2.zero;
        RaycastHit hit;
        if (Input.GetMouseButton(0) && Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 20f)) 
        {
            if (hit.collider.CompareTag("Background"))
            {
                actualMousePosition = hit.point;
                if (lastMousePosition != null && lastPresed && Vector2.Distance(actualMousePosition, lastMousePosition) * Time.deltaTime > minTouchSpeed)
                {
                    input = -(lastMousePosition - actualMousePosition).normalized; 
                }
                lastMousePosition = actualMousePosition;
                lastPresed = true;
            }
        }
        else lastPresed = false;
        
        #if UNITY_EDITOR || !UNITY_ANDROID
        Vector2 axisInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        if (axisInput.magnitude != 0 && Physics.Raycast(cam.ScreenPointToRay(screenMiddle + (axisInput.normalized * 50)), out hit, 20f))
        {
            input = hit.point.normalized; 
        }
        #endif

        if (game.gamePlaying && !game.pause)
        {
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
    }

    void FixedUpdate ()
    {

        if (direccion != Vector2.zero)
        {
            lastAngle = Vector2.SignedAngle(Vector2.right, direccion);
        }
        if (rot.transform.eulerAngles.z != lastAngle)
        {
            rot.transform.eulerAngles = new Vector3(0f, 0f, Mathf.MoveTowardsAngle(rot.transform.eulerAngles.z, lastAngle, rotateSpeed * Time.deltaTime));
        }
    }

    void OnCollisionEnter2D (Collision2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            ArrowController arrow =  col.gameObject.GetComponent<ArrowController>();
            if (arrow.CollisionWithShield())
            {
                arrow.DestroyArrow();
                shield.Defend();
            }
            else 
            {
                arrow.DestroyArrow();
                Hurt();
            }
        }
        else if (col.gameObject.CompareTag("Anti Bullet"))
        {
            col.gameObject.GetComponent<ArrowController>().DestroyArrow();
            shield.Defend();
        }
        else if (col.gameObject.CompareTag("False Bullet"))
        {
            col.gameObject.GetComponent<ArrowController>().DestroyArrow();
        }
    }

    public void Hurt ()
    {
        anim.SetTrigger("Hurted");
        game.Damage();
    }
}
