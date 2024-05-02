using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParent : MonoBehaviour
{
    #region Variables
    public bool smoothTrans = false;
    public float moveSpeed = 10f;
    public float rotateSpeed = 500f;
    private bool facingWall = false, leftToWall = false, rightToWall = false, backToWall = false;
    public float moveMulti = 1.1f;

    Vector3 targetGridPos, prevTargetGridPos, targetRotate;

    public GameObject player;
    #endregion

    private void Awake()
    {
        //Assign Player Gameobject to get movement
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
