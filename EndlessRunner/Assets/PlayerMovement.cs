
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
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnDiedEvent;
    public static GoingInDirection currentDirection;
    public float forwardSpeed = 5f;
    public float jumpVelocity = 10f;
    public float swipeThreshold = 50f;
    public Animator playerAnimator;
    public bool isDead = false;
    [SerializeField] float rayLength = 5;
    [SerializeField] LayerMask layerOfGround;
    [SerializeField] Transform camTransform;
    [SerializeField] float rotationSpeed;


    private Rigidbody rb;
    private bool isGrounded = true;
    private Vector2 startTouchPosition, endtouchPosition;
    private bool isSwiping = false;
    private int currentLane = 1; // Initial lane index
    private bool canTurn = false;
    private Vector3 turnPosition;
    public bool freeRun = false;

    GoingInDirection n;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentDirection = GoingInDirection.South;
        n = currentDirection;
    }

    void Update()
    {
        if (!isDead && MainScript.instance.gameStarted)
        {
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
            HandleSwipeInput();
            if (n != currentDirection)
            {
                n = currentDirection;
                Debug.Log(currentDirection);
            }
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
        if (swipe < 0)
        {
            if (currentLane == 0) return;
            swipe = -1.1f;
            currentLane--;
        }
        else
        {
            if (currentLane == 2) return;
            swipe = 1.1f;
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
            float rotTo = transform.eulerAngles.y;
            rotTo += 90;
            transform.DORotate(new Vector3(0, rotTo, 0), 0.2f);
            currentDirection = currentDirection - 1;
            if ((int)currentDirection < 0)
                currentDirection = (GoingInDirection)3;
        }
        else
        {
            float rotTo = transform.eulerAngles.y;
            rotTo -= 90;
            transform.DORotate(new Vector3(0, rotTo, 0), 0.2f);
            currentDirection = currentDirection + 1;
            if ((int)currentDirection > 3)
                currentDirection = (GoingInDirection)0;
        }
        canTurn = false;
        StartCoroutine(SetPosAfterTurn());
    }
    private IEnumerator SetPosAfterTurn() // set the position of player to the middle lane
    {
        transform.position = new Vector3(turnPosition.x, transform.position.y, turnPosition.z);
        forwardSpeed = 0;
        yield return new WaitForSeconds(0.1f);
        forwardSpeed = 10;
        currentLane = 1;
    }
    private void JumpOrDuck(float swipe)
    {
        if (swipe > 0)
        {
            Duck();
        }
        else
        {
            Jump();
        }
    }

    private void Duck()
    {
        playerAnimator.SetTrigger("Slide");
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            playerAnimator.SetTrigger("Jump");
            //rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            rb.velocity = Vector2.up * jumpVelocity;
        }
    }

    private bool IsGrounded()
    {
        Debug.Log("qqqq");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, layerOfGround))
        {
            Debug.Log(hit.transform.name);
        }

        //Debug.DrawRay(transform.position, transform.up * rayLength, Color.yellow);
        return Physics.Raycast(transform.position, Vector3.down, rayLength, layerOfGround);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Turn"))
        {
            canTurn = true;
            other.gameObject.SetActive(false);
            turnPosition = other.transform.position;
            if (freeRun)
            {
                if (other.name == "turnRight")
                {
                    RotatePlayer(-1);
                }
                else
                {
                    RotatePlayer(1);
                }
            }
        }
        if (other.CompareTag("Obstacle") && !freeRun)
        {
            PlayerDied();
        }
    }

    private void PlayerDied()
    {
        playerAnimator.SetTrigger("Died");
        rb.velocity = -transform.forward * jumpVelocity;
        playerAnimator.transform.DOLocalMoveY(-0.6f, 0.5f).SetDelay(1);
        camTransform.LookAt(playerAnimator.transform.position);
        //StartCoroutine(RotateAroundPlayer());
        isDead = true;
        OnDiedEvent?.Invoke();
    }
    private IEnumerator RotateAroundPlayer()
    {
        //float rot = 0;
        yield return new WaitForSeconds(2f);
        //while (rot < 9)
        //{
        //    Debug.Log(rot);
        //    rot += 1;
        //    yield return new WaitForSeconds(0.1f);
        //    camTransform.RotateAround(playerAnimator.transform.position, Vector3.right, rot);
        //}

    }
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Turn"))
    //    {
    //        canTurn = false;
    //    }
    //}
}
