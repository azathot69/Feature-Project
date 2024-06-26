using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;


/// <summary>
/// Controls the player's movement and actions
/// Player can move in the four cardinal directions
/// Player can also rotate camera 90 degrees left or right
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region Variables
    //Singleton
    public static PlayerController Instance;

    //Movement
    private bool smoothTrans = true;
    private float moveSpeed = 10f;
    private float rotateSpeed = 500f;
    private float moveMulti = 2f;
    private Vector3 startingPos;
    //---Keep Public!
    public Vector3 targetGridPos, prevTargetGridPos, targetRotate;

    //Collision
    private bool facingWall = false, leftToWall = false, rightToWall = false, backToWall = false;
    public bool playerTurn = false;
    
    //Raycast
    [SerializeField]
    private float raycastDistance = 5f;
    private Vector3 raycastDir = Vector3.forward;
    private Vector3 raycastDirLeft = Vector3.left;
    private Vector3 raycastDirRight = Vector3.right;
    private Vector3 raycastDirBack = Vector3.back;
    private Vector3 tileSense = Vector3.down;

    //List
    public List<string> itemGathered = new List<string>();
    
    //Player Data
    PlayerController controller;
    InputAction moveAction;
    PlayerInput playerInput;

    public PlayerData playerData;

    //Score Text
    //public TMP_Text scoreText;

    #endregion

    private void Awake()
    {
        //Initialize Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        startingPos = transform.position;

        controller = GetComponent<PlayerController>();

        playerData.GetComponent<PlayerData>().ResetScore();
    }

    private void Start()
    {
        targetGridPos = Vector3Int.RoundToInt(transform.position);
        //Initialize Text
        //scoreText.text = "Score : $" + PlayerData.Instance.score;
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
        //scoreText.text = "Score: $" + PlayerData.Instance.score;

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

    #region Movement Logic

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
    /// Check for walls and enemies via Raycast
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

                case "Home":
                    facingWall = true;
                    break;

                case "ShortCut":
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

                case "Home":
                    leftToWall = true;
                    break;

                case "ShortCut":
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

                case "Home":
                    rightToWall = true;
                    break;

                case "ShortCut":
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

                case "Home":
                    backToWall = true;
                    break;

                case "ShortCut":
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
                    PlayerData.Instance.ResetScore();
                    SceneManager.LoadScene(1);
                    
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
                    PlayerData.Instance.ResetScore();
                    SceneManager.LoadScene(1);

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
                    PlayerData.Instance.ResetScore();
                    SceneManager.LoadScene(1);

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
                    PlayerData.Instance.ResetScore();
                    SceneManager.LoadScene(1);

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

    #endregion

    /// <summary>
    /// Checks below and infront of the player for anything to do
    /// </summary>
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

                    //Assign Temp Variable
                    var _instance = tileCheck.collider.gameObject;

                    //Check Item
                    var itemGet = _instance.GetComponent<GatherSpot>().Gather();

                    //Check if can gather resources
                    if (itemGet == null) return;
                    itemGathered.Add(itemGet);

                    //Add to score
                    //PlayerData.Instance.score += 100;
                    PlayerData.Instance.GetComponent<PlayerData>().score += 100;

                    //Player Turn
                    StartCoroutine(PlayerTurnCountdown());

                    break;

                default:
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
                    //Assign Temp Variable
                    var _shortCut = hitFront.collider.gameObject;

                    if (_shortCut.GetComponent<ShortCuts>().isActivated == false) return;

                    _shortCut.GetComponent<ShortCuts>().UseShortCut();
                    StartCoroutine(PlayerTurnCountdown());
                    break;

                case "Home":
                    //Ask the player if they want to return home

                    playerData.GetComponent<PlayerData>().endScore = PlayerData.Instance.GetComponent<PlayerData>().score;

                    SceneManager.LoadScene(5);

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
