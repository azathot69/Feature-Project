using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool facingWall = false;
    public float moveMulti = 2f;

    Vector3 targetGridPos, prevTargetGridPos, targetRotate;

    [SerializeField]
    private Movement moveBehave;

    private float raycastDistance = 1.5f;
    private Vector3 raycastDir = Vector3.forward;
    #endregion

    private void Awake()
    {
        targetGridPos = Vector3Int.RoundToInt(transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveEnemy();

        //Move Enemy
        if (PlayerController.Instance.playerTurn == true && AtRest)
        {
            CheckWall();
            switch (moveBehave)
            {
                case Movement.Bounce:
                    if (facingWall==false)
                    {
                        targetGridPos += transform.forward * moveMulti;
                    }
                    break;

                case Movement.Turn:

                    break;


            }
        }
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, raycastDir * raycastDistance, Color.green);
    }

    private void CheckWall()
    {
        RaycastHit hitFront;
        if (Physics.Raycast(transform.position, raycastDir, out hitFront, raycastDistance))
        {

            switch (hitFront.collider.gameObject.tag)
            {
                case "Wall":
                    targetRotate -= Vector3.up * 180f;
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
