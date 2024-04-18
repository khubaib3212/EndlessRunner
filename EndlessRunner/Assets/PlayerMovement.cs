
using UnityEngine;
using DG.Tweening;
using System;
using UnityEditor.Rendering;
using UnityEngine.SubsystemsImplementation;
using System.Collections;

public enum GoingInDirection
{
    West, South, East, North
}
public class PlayerMovement : MonoBehaviour
{
    public static GoingInDirection currentDirection;
    public float moveSpeed = 5f;
    public float forwardSpeed = 5f;
    public float jumpForce = 10f;
    public float swipeThreshold = 50f;
    public Animator playerAnimator;
    [SerializeField] float rayLength = 1;
    [SerializeField] LayerMask layerOfGround;
    [SerializeField] Transform camTransform;
    [SerializeField] float rotationSpeed;


    private Rigidbody rb;
    private bool isGrounded = true;
    private Vector2 startTouchPosition, endtouchPosition;
    private bool isSwiping = false;
    private int currentLane = 1; // Initial lane index
    private bool canTurn = false;
    GoingInDirection n;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentDirection = GoingInDirection.South;
        n = currentDirection;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        HandleSwipeInput();
        Debug.DrawRay(transform.position, transform.up * rayLength, Color.yellow);
        if (n != currentDirection)
        {
            n = currentDirection;
            Debug.Log(currentDirection);
        }
    }

    private void HandleSwipeInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endtouchPosition = Input.mousePosition;
            Vector3 swipeDistance = startTouchPosition - endtouchPosition;
            if (Mathf.Abs(swipeDistance.x) > Mathf.Abs(swipeDistance.y))
            {
                if (!canTurn)
                {
                    SwitchLane(swipeDistance.normalized.x);
                }
                else
                {
                    RotatePlayer(swipeDistance.normalized.x);
                    //camTransform.GetComponent<CameraFollow>().turnTime = 0.2f;
                    //camTransform.GetComponent<CameraFollow>().RotateAroundPlayer();
                    //camTransform.RotateAround(transform.position, Vector3.up, 90);
                    //transform.Rotate(0, 90, 0);
                    //camTransform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                    //currentDirection = GoingInDirection.West;
                    //transform.rotation = Quaternion.Euler(0, 90, 0);
                }
            }
            else
            {
                JumpOrDuck(swipeDistance.normalized.y);
            }
        }
    }
    
    private void SwitchLane(float swipe)
    {
        Debug.Log("Swipe = " + swipe);
        if (swipe < 0)
        {
            if (currentLane == 0) return;
            swipe = -2;
            currentLane--;
        }
        else
        {
            if (currentLane == 2) return;
            swipe = 2;
            currentLane++;
        }
        Vector3 temp = transform.localPosition;
        switch (currentDirection)
        {
            case GoingInDirection.South:
                temp.x -= swipe;
                temp.z += 2;
                break;
            case GoingInDirection.West:
                temp.z += swipe;
                temp.x += 2;
                break;
            case GoingInDirection.North:
                temp.x += swipe;
                temp.z -= 2;
                break;
            case GoingInDirection.East:
                temp.z -= swipe;
                temp.x -= 2;
                break;
        }
        transform.DOMove(temp, 0.2f);
    }
    private void RotatePlayer(float swipe)
    {
        if (swipe < 0)
        {
            float rotTo=transform.eulerAngles.y;
            rotTo += 90;
            transform.DORotate(new Vector3(0, rotTo, 0), 0.2f);
            //transform.Rotate(0, 90, 0);
            currentDirection = currentDirection - 1;
            if ((int)currentDirection < 0)
                currentDirection = (GoingInDirection)3;
        }
        else
        {
            float rotTo = transform.eulerAngles.y;
            rotTo -= 90;
            transform.DORotate(new Vector3(0, rotTo, 0), 0.2f);
            //transform.Rotate(0, -90, 0);
            currentDirection = currentDirection + 1;
            if ((int)currentDirection > 3)
                currentDirection = (GoingInDirection)0;
        }
    }
    private void JumpOrDuck(float swipe)
    {
        Debug.Log("Jumping or ducking...");
        if (swipe > 0)
        {
            Debug.Log("Ducking");
            Duck();
        }
        else
        {
            Debug.Log("Jumping");
            Jump();
        }
    }

    private void Duck()
    {
        playerAnimator.SetTrigger("Slide");
    }

    private void Jump()
    {
        Debug.Log("Checking if grounded...");
        if (IsGrounded())
        {
            playerAnimator.SetTrigger("Jump");
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, transform.up, rayLength, layerOfGround);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Turn"))
        {
            canTurn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Turn"))
        {
            canTurn = false;
        }
    }
}
