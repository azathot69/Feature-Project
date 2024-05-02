using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Movement
{
    Bounce,
    Loop
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
            targetGridPos += transform.forward * moveMulti;
        }
    }

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
