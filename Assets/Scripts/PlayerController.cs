using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float originalSpeed = 15.0f;
    private float speed ;
    private float speedIncreaseLastTick;
    private float speedIncreaseTime=2.5f;
    private float speedIncreaseAmount = 0.1f;

    public float jumpForce=10.0f;

    private bool isRunning = false;
    private bool isGrounded;

    //private Rigidbody rb;
    private CharacterController controller;
    private float gravity = 12.0f;
    private float verticalVelocity;
    Animator anim;
    private int desiredLane=1;
    private float laneDist = 2f;
    public float turnSpeed = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
      speed=  originalSpeed;
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame

    private void Update()
    {
        if (!isRunning)
            return;


        if (MobileInput.Instance.SwipeLeft)
        {
            moveLane(false);
        }
        else if (MobileInput.Instance.SwipeRight)
        {
            moveLane(true);
        }
        isGrounded = IsGrounded();
        anim.SetBool("grounded", isGrounded);

        if (isGrounded)
        {
            verticalVelocity = -0.1f;
            if (MobileInput.Instance.SwipeUp)
            {
                anim.SetTrigger("jump");
                verticalVelocity = jumpForce;
            }
            else if (MobileInput.Instance.SwipeDown)
            {
                StartSliding();
                Invoke("StopSliding", 1.0f);
            }
        }
        else
        {
            verticalVelocity -= (gravity * Time.deltaTime);
            if (MobileInput.Instance.SwipeDown)
            {
                verticalVelocity = -jumpForce;
            }
        }
    }

    void FixedUpdate()
    {
        if (!isRunning)
            return;

        if (Time.time - speedIncreaseLastTick > speedIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            GameFlow.instance.UpdateModifier(speed-originalSpeed);
        }

        Vector3 targetPos = Vector3.forward * transform.position.z;

        if (desiredLane == 0)
        {
            targetPos += Vector3.left * laneDist;
        }else if (desiredLane == 2)
        {
            targetPos += Vector3.right * laneDist;
        }

        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPos - transform.position).normalized.x * speed;

       
        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        controller.Move(moveVector * Time.deltaTime);

        Vector3 dir = controller.velocity;
        if (dir != Vector3.zero)
        {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, turnSpeed);
        }
    }

    private void StartSliding()
    {
        anim.SetBool("Sliding", true);

        controller.height /= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y/2, controller.center.z);
    }

    private void StopSliding()
    {
        anim.SetBool("Sliding", false);

        controller.height *= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y*2, controller.center.z);
    }

    private void moveLane(bool lane)
    {
        desiredLane += lane ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }

    private bool IsGrounded()
    {
        Ray groundRay = new Ray(new Vector3(controller.bounds.center.x,(controller.bounds.center.y-controller.bounds.extents.y)+0.2f,controller.bounds.center.z),Vector3.down);
        return Physics.Raycast(groundRay,0.2f+0.1f);
    }

    public void StartRunning()
    {
        isRunning = true;
        anim.SetTrigger("StartRunning");
    }

    private void crash()
    {
        anim.SetTrigger("Death");
        isRunning = false;
        GameFlow.instance.OnDeath(); 
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.gameObject.tag)
        {
            case "Obstacles":
                crash();
                break;
            case "Sea":
                crash();
                break;
        }
    }

    
}
