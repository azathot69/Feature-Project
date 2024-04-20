using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool smoothTrans = false;
    public float moveSpeed = 10f;
    public float rotateSpeed = 500f;

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
                Debug.Log("Turning Left");
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
                Debug.Log("Turning Right");
            }
        }
    }

    //Move Player
    public void MoveForward(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (AtRest)
            {
                targetGridPos += transform.forward;
                Debug.Log("Moving Forward");
            }
        }
    }
    public void MoveBack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (AtRest)
            {
                targetGridPos -= transform.forward;
                Debug.Log("Moving Back");
            }
        }
    }
    public void MoveLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (AtRest)
            {
                targetGridPos -= transform.right;
                Debug.Log("Moving Left");
            }
        }
    }
    public void MoveRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (AtRest)
            {
                targetGridPos += transform.right;
                Debug.Log("Moving Right");
            }
        }
    }


    #endregion

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if (true)
        {
            prevTargetGridPos = targetGridPos;
            Vector3 targetPos = targetGridPos;

            if (targetRotate.y > 270f && targetRotate.y < 361f)
            {
                targetRotate.y = 0f;
            }
            if (targetRotate.y < 0f)
            {
                targetRotate.y = 270f;
            }

            if (!smoothTrans)
            {
                transform.position = targetPos;
                transform.rotation = Quaternion.Euler(targetRotate);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotate), Time.deltaTime * rotateSpeed);
            }
        }
        else
        {
            targetGridPos = prevTargetGridPos;
        }
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
