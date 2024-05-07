using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

enum Movement
{
    Bounce,
    Turn,
    TurnOther
}
public class EnemyParent : MonoBehaviour
{
    #region Variables
    public bool smoothTrans = false;
    public float moveSpeed = 10f;
    public float rotateSpeed = 500f;
    [SerializeField]
    private bool facingWall = false;
    public float moveMulti = 2f;

    Vector3 targetGridPos, prevTargetGridPos, targetRotate;

    [SerializeField]
    private Movement moveBehave;

    private float raycastDistance = 1.5f;
    private Vector3 raycastDir = Vector3.forward;
    [SerializeField]
    private int startRotate;
    #endregion

    private void Start()
    {
        targetGridPos = Vector3Int.RoundToInt(transform.position);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + startRotate, 0);
        if (moveBehave == Movement.Turn)
        {
            targetRotate += Vector3.up * 90f;
        }
        if (moveBehave == Movement.TurnOther)
        {
            targetRotate += Vector3.up * -90f;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        MoveEnemy();

        
    }

    private void Update()
    {
        
        CheckWall();
        Debug.DrawRay(transform.position, raycastDir * raycastDistance, Color.green);
        raycastDir = transform.TransformDirection(0, 0, raycastDistance);



        //Move Enemy
        if (!PlayerController.Instance.playerTurn) { return; }


        switch (moveBehave)
        {
            //Move in opposite direction when facing a wall
            case Movement.Bounce:


                if (facingWall == false)
                {
                    if (!AtRest) return;
                    targetGridPos += (transform.forward * moveMulti);
                }
                if (facingWall == true)
                {
                    if (!AtRest) return;
                    //Turn 180 degrees
                    targetRotate += Vector3.up * 180f;
                }


                break;

            //Turn right and move when facing wall
            case Movement.Turn:
                if (facingWall == true)
                {
                    if (!AtRest) return;
                    //Turn 90 degrees
                    targetRotate += Vector3.up * 90f;
                }

                if (!AtRest) return;
                targetGridPos += (transform.forward * moveMulti);
                break;

            case Movement.TurnOther:
                if (facingWall == true)
                {
                    if (!AtRest) return;
                    //Turn 90 degrees
                    targetRotate += Vector3.up * 90f;
                }
                if (!AtRest) return;
                targetGridPos += (transform.forward * moveMulti);

                break;

        }

    }

    private void CheckWall()
    {
        RaycastHit hitFront;
        if (Physics.Raycast(transform.position, raycastDir, out hitFront, raycastDistance))
        {
            switch (hitFront.collider.gameObject.tag)
            {
                case "Wall":
                    facingWall = true;

                    break;

                case "EnemyLimit":
                    facingWall = true;
                    break;

                default:

                    break;
            }
        }
        else
        {
            facingWall = false;
        }

    }


    /// <summary>
    /// The enemy from one square to another
    /// </summary>
    private void MoveEnemy()
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



        prevTargetGridPos = targetGridPos;
        Vector3 targetPos = targetGridPos;


        #region Move Smoothly
        if (!smoothTrans)
        {
            transform.position = targetPos;
            transform.rotation = Quaternion.Euler(targetRotate);

        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);
        }
        #endregion
    }

    /// <summary>
    /// Checks to see if the instance is at rest (not Moving)
    /// </summary>
    bool AtRest
    {
        
        get
        {
            if ((Vector3.Distance(transform.position, targetGridPos) < 0.05f) &&
                (Vector3.Distance(transform.eulerAngles, targetRotate) < 0.05f))
            {
                //Debug.Log("At Rest: " + true);

                return true;
            }
            else
            {
                //Debug.Log("At Rest: " + false);

                return false;
            }
        }

    }
}
