using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the player's movement and actions
/// Player can move in the four cardinal directions
/// Player can also rotate camera 90 degrees left or right
/// </summary>
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public bool smoothTrans = false;
    public float moveSpeed = 10f;
    public float rotateSpeed = 500f;
    private bool facingWall = false, leftToWall = false, rightToWall = false, backToWall = false;
    public bool playerTurn = false;
    public float moveMulti = 1.1f;
    [SerializeField]
    private float raycastDistance = 5f;
    private Vector3 raycastDir = Vector3.forward;
    private Vector3 raycastDirLeft = Vector3.left;
    private Vector3 raycastDirRight = Vector3.right;
    private Vector3 raycastDirBack = Vector3.back;


    Vector3 targetGridPos, prevTargetGridPos, targetRotate;
    PlayerController controller;
    InputAction moveAction;
    PlayerInput playerInput;

    private void Awake()
    {
        //Initialize Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        controller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        targetGridPos = Vector3Int.RoundToInt(transform.position);

    }

    #region Movement
    #region Turn Player
    /// <summary>
    /// Rotate Player 90 degrees to the left
    /// </summary>
    /// <param name="context"></param>
    public void RotateLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (AtRest)
            {
                targetRotate -= Vector3.up * 90f;
            }
        }
    }

    /// <summary>
    /// Rotate Player 90 degrees to the right
    /// </summary>
    /// <param name="context"></param>
    public void RotateRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (AtRest)
            {
                targetRotate += Vector3.up * 90f;
            }
        }
    }
    #endregion

    //Move Player

    /// <summary>
    /// Moves player towards the direction they're facing
    /// </summary>
    /// <param name="context"></param>
    public void MoveForward(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (AtRest && !facingWall)
            {
                playerTurn = true;
                targetGridPos += transform.forward * moveMulti;
                //playerTurn = false;
            }
        }
    }

    /// <summary>
    /// Moves player away from the direction they were facing
    /// </summary>
    /// <param name="context"></param>
    public void MoveBack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (AtRest && !backToWall)
            {
                playerTurn = true;
                targetGridPos -= transform.forward * moveMulti;
                //playerTurn = false;
            }
        }
    }

    /// <summary>
    /// Moves the player to their left
    /// </summary>
    /// <param name="context"></param>
    public void MoveLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (AtRest && !leftToWall)
            {
                playerTurn = true;
                targetGridPos -= transform.right * moveMulti;
                //playerTurn = false;
            }
        }
    }

    /// <summary>
    /// Moves the player to their right
    /// </summary>
    /// <param name="context"></param>
    public void MoveRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (AtRest && !rightToWall)
            {
                playerTurn = true;
                targetGridPos += transform.right * moveMulti;
                //playerTurn = false;

            }
        }
    }


    #endregion

    private void FixedUpdate()
    {
        MovePlayer();
        
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, raycastDir * raycastDistance, Color.green);
        Debug.DrawRay(transform.position, raycastDirLeft * raycastDistance, Color.red);
        Debug.DrawRay(transform.position, raycastDirRight * raycastDistance, Color.blue);
        Debug.DrawRay(transform.position, raycastDirBack * raycastDistance, Color.yellow);
        raycastDir = transform.TransformDirection(0, 0, 1);
        raycastDirLeft = transform.TransformDirection(-1, 0, 0);
        raycastDirRight = transform.TransformDirection(1, 0, 0);
        raycastDirBack = transform.TransformDirection(0, 0, -1);
        CheckForWalls();

        if (playerTurn == true) playerTurn = false;
    }

    /// <summary>
    /// Makes the player move and/or rotate
    /// </summary>
    void MovePlayer()
    {
        #region Turn
        if (targetRotate.y > 270f && targetRotate.y < 361f)
        {
            targetRotate.y = 0f;
        }
        if (targetRotate.y < 0f)
        {
            targetRotate.y = 270f;
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotate), Time.deltaTime * rotateSpeed);
        #endregion

        #region Move

        //Code is called every frame
        prevTargetGridPos = targetGridPos;
        Vector3 targetPos = targetGridPos;


        #region Move Smoothly
        if (!smoothTrans){
            transform.position = targetPos;
            transform.rotation = Quaternion.Euler(targetRotate);
            
        }else{
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);
        }
        #endregion
        

        #endregion
    }

    /// <summary>
    /// Check for walls via Raycast
    /// </summary>
    void CheckForWalls()
    {
        RaycastHit hitFront;
        RaycastHit hitLeft;
        RaycastHit hitRight;
        RaycastHit hitBack;

        #region Forward
        if (Physics.Raycast(transform.position, raycastDir, out hitFront, raycastDistance))
        {
            
            switch(hitFront.collider.gameObject.tag)
            {
                case "Wall":
                    facingWall = true;
                    break;

                default:
                    Debug.Log("You hit something...");
                    break;

                case null:

                    break;
            }
        }
        else
        {
            facingWall = false;
        }
        #endregion

        #region Left
        if (Physics.Raycast(transform.position, raycastDirLeft, out hitLeft, raycastDistance))
        {

            switch (hitLeft.collider.gameObject.tag)
            {
                case "Wall":
                    leftToWall = true;
                    break;

                default:
                    Debug.Log("You hit something...");
                    break;

                case null:

                    break;
            }
        }
        else
        {
            leftToWall = false;
        }
        #endregion

        #region Right
        if (Physics.Raycast(transform.position, raycastDirRight, out hitRight, raycastDistance))
        {

            switch (hitRight.collider.gameObject.tag)
            {
                case "Wall":
                    rightToWall = true;
                    break;

                default:
                    Debug.Log("You hit something...");
                    break;

                case null:

                    break;
            }
        }
        else
        {
            rightToWall = false;
        }
        #endregion

        #region Back
        if (Physics.Raycast(transform.position, raycastDirBack, out hitBack, raycastDistance))
        {

            switch (hitBack.collider.gameObject.tag)
            {
                case "Wall":
                    backToWall = true;
                    break;

                default:
                    Debug.Log("You hit something...");
                    break;

                case null:

                    break;
            }
        }
        else
        {
            backToWall = false;
        }
        #endregion
    }


    /// <summary>
    /// Makes sure the player isn't moving
    /// </summary>
    bool AtRest
    {
        get
        {
            if ((Vector3.Distance(transform.position, targetGridPos) < 0.05f) &&
                (Vector3.Distance(transform.eulerAngles, targetRotate) < 0.05f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }



    /*
    IEnumerator PlayerTurnCountdown()
    {

    }
    */
}
