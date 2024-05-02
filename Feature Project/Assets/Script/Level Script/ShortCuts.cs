using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The controll script of shortcuts.
/// Controls whether a shortcut is one way both ways.
/// Checks to see if it is active or not.
/// Set the distance the player will spawn after use.
/// </summary>
public class ShortCuts : MonoBehaviour
{
    #region Variables

    ///Bools
    //If Checked, Player can only use it one way
    public bool isOneWay;

    //If Checked, Player can use it
    public bool isActivated;

    //If Checked, this is the wrong way; player must reach other side
    public bool isExit;

    //Variables
    //The position the S.C. will place the player from the entrance
    public Vector2 fromEntrance;

    //The position to place the player from the exit
    public Vector2 fromExit;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
