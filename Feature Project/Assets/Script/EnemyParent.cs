using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

enum Movement
{
    Bounce,
    Turn
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
    #endregion

    private void Start()
    {
        targetGridPos = Vector3Int.RoundToInt(transform.position);
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
        if (PlayerController.Instance.playerTurn && AtRest)
        {
            switch (moveBehave)
            {
                //Move in opposite direction when facing a wall
                case Movement.Bounce:

                    
                    if (facingWall == false)
                    {
                        print("Moving");
                        targetGridPos += (transform.forward * moveMulti);
                    }
                    if (facingWall == true)
                    {
                        //Turn 180 degrees
                        print("Turning");
                        transform.RotateAround(transform.position, transform.up, 180f);
                        targetGridPos += (transform.forward * moveMulti);
                    }
                    

                    break;

                //Turn right and move when facing wall
                case Movement.Turn:
                    if (facingWall == true)
                    {
                        //Turn 90 degrees
                        transform.RotateAround(transform.position, transform.up, 90f);
                    }
                    targetGridPos += (transform.forward * moveMulti);
                    break;


            }
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
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
