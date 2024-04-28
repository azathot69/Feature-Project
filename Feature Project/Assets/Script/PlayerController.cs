using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool smoothTrans = false;
    public float moveSpeed = 10f;
    public float rotateSpeed = 500f;
    public bool facingWall = false, leftToWall = false, rightToWall = false, backToWall = false;
    [SerializeField]
    private float raycastDistance = 5f;
    private Vector3 raycastDir = Vector3.forward;
    private Vector3 raycastDirUL = Vector3.forward + Vector3.left;
    private Vector3 raycastDirUR = Vector3.forward + Vector3.right;
    private Vector3 raycastDirLeft = Vector3.left;
    private Vector3 raycastDirRight = Vector3.right;
    private Vector3 raycastDirBack = Vector3.back;
    private Vector3 raycastDirBL = Vector3.back + Vector3.left;
    private Vector3 raycastDirBR = Vector3.back + Vector3.right;

    Vector3 targetGridPos, prevTargetGridPos, targetRotate;
    PlayerController controller;
    InputAction moveAction;
    PlayerInput playerInput;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        targetGridPos = Vector3Int.RoundToInt(transform.position);

    }

    #region Movement
    //Rotate Player
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

    //Move Player
    public void MoveForward(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (AtRest && !facingWall)
            {
                targetGridPos += transform.forward;
            }
        }
    }
    public void MoveBack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (AtRest && !backToWall)
            {
                targetGridPos -= transform.forward;
            }
        }
    }
    public void MoveLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (AtRest && !leftToWall)
            {
                targetGridPos -= transform.right;
            }
        }
    }
    public void MoveRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (AtRest && !rightToWall)
            {
                targetGridPos += transform.right;
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
    }

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
}
