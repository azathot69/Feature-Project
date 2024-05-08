using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


/// <summary>
/// Controls the player's movement and actions
/// Player can move in the four cardinal directions
/// Player can also rotate camera 90 degrees left or right
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region Variables
    public static PlayerController Instance;

    public bool smoothTrans = false;
    public float moveSpeed = 10f;
    public float rotateSpeed = 500f;
    public bool facingWall = false, leftToWall = false, rightToWall = false, backToWall = false;
    public bool enemyHit = false;
    public bool playerTurn = false;
    public float moveMulti = 1.1f;
    [SerializeField]
    private float raycastDistance = 5f;
    private Vector3 raycastDir = Vector3.forward;
    private Vector3 raycastDirLeft = Vector3.left;
    private Vector3 raycastDirRight = Vector3.right;
    private Vector3 raycastDirBack = Vector3.back;
    private Vector3 tileSense = Vector3.down;

    

    List<string> itemGathered = new List<string>();
    private Vector3 startingPos;

    Vector3 targetGridPos, prevTargetGridPos, targetRotate;
    PlayerController controller;
    InputAction moveAction;
    PlayerInput playerInput;
    #endregion

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

        startingPos = transform.position;

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

    #region Move Player

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
                StartCoroutine(PlayerTurnCountdown());
                targetGridPos += transform.forward * moveMulti;
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
                StartCoroutine(PlayerTurnCountdown());
                targetGridPos -= transform.forward * moveMulti;
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
                StartCoroutine(PlayerTurnCountdown());
                targetGridPos -= transform.right * moveMulti;
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
                StartCoroutine(PlayerTurnCountdown());
                targetGridPos += transform.right * moveMulti;
            }
        }
    }


    #endregion

    #region
    public void Action(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ActionCheck();
        }
    }
    #endregion

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
        Debug.DrawRay(transform.position, tileSense * raycastDistance, Color.magenta);
        raycastDir = transform.TransformDirection(0, 0, 1);
        raycastDirLeft = transform.TransformDirection(-1, 0, 0);
        raycastDirRight = transform.TransformDirection(1, 0, 0);
        raycastDirBack = transform.TransformDirection(0, 0, -1);
        CheckForWalls();

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            enemyHit = true;
            SceneManager.LoadScene(0);
            transform.position = startingPos;
        }
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
        RaycastHit enemyCheck;


        #region Forward
        if (Physics.Raycast(transform.position, raycastDir, out hitFront, raycastDistance))
        {
            
            switch(hitFront.collider.gameObject.tag)
            {
                case "Wall":
                    facingWall = true;
                    break;

                default:
                    facingWall = false;
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
                    leftToWall = false;
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
                    rightToWall = false;
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
                    backToWall = false;
                    break;
            }
        }
        else
        {
            backToWall = false;
        }
        #endregion

        # region Check for enemy
        if (Physics.Raycast(transform.position, raycastDir, out enemyCheck, raycastDistance/4))
        {
            switch (enemyCheck.collider.gameObject.tag)
            {
                //Check For Enemies
                case "Enemy":
                    //Game Over
                    enemyHit = true;
                    SceneManager.LoadScene(0);
                    
                    break;
            }
        }

        if (Physics.Raycast(transform.position, raycastDirLeft, out enemyCheck, raycastDistance / 4))
        {
            switch (enemyCheck.collider.gameObject.tag)
            {
                //Check For Enemies
                case "Enemy":
                    //Game Over
                    enemyHit = true;
                    SceneManager.LoadScene(0);

                    break;
            }
        }

        if (Physics.Raycast(transform.position, raycastDirRight, out enemyCheck, raycastDistance / 4))
        {
            switch (enemyCheck.collider.gameObject.tag)
            {
                //Check For Enemies
                case "Enemy":
                    //Game Over
                    enemyHit = true;
                    SceneManager.LoadScene(0);

                    break;
            }
        }

        if (Physics.Raycast(transform.position, raycastDirBack, out enemyCheck, raycastDistance / 4))
        {
            switch (enemyCheck.collider.gameObject.tag)
            {
                //Check For Enemies
                case "Enemy":
                    //Game Over
                    enemyHit = true;
                    SceneManager.LoadScene(0);

                    break;
            }
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

    //Checks below and infront of the player for any thing to do
    private void ActionCheck()
    {
        RaycastHit hitFront;
        RaycastHit tileCheck;

        //Check below player
        if (Physics.Raycast(transform.position, tileSense, out tileCheck, raycastDistance))
        {

            switch (tileCheck.collider.gameObject.tag)
            {
                //Check for Resources
                case "Resource":
                    //Check if can gather resources

                    break;
            }
        }

        //Check in front of player
        if (Physics.Raycast(transform.position, raycastDir, out hitFront, raycastDistance))
        {

            switch (hitFront.collider.gameObject.tag)
            {
                case "ShortCut":
                    //Activate Short Cut

                    break;

                default:

                    break;

            }
        }
    }

    /// <summary>
    /// For the enemies;
    /// When moving, temporary turn on "PlayerTurn" for a frame
    /// This allows the enemies to move
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerTurnCountdown()
    {
        playerTurn = true;
        yield return new WaitForSeconds(.1f);
        playerTurn = false;
    }
    
}
