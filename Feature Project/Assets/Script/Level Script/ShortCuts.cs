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
    public Vector3 toEntrance;


    //The other end of the Short Cut (if applicable)
    [SerializeField]
    private GameObject otherEntrance;
    [SerializeField]
    private GameObject player;
    #endregion

    /// <summary>
    /// Send player to other side
    /// </summary>
    public void UseShortCut()
    {
        //Check to see which end of the short cut the player is checking
        if (isExit)
        {
            //Check to see if path is unlocked.
            if (isActivated)
            {
                //If Unlocked, move player
                player.GetComponent<PlayerController>().targetGridPos.x = toEntrance.x;
                player.GetComponent<PlayerController>().targetGridPos.z = toEntrance.z;
                player.transform.position = toEntrance;
                return;
            }
            else
            {
                //If locked, deny passage
                print("This passage is blocked");
                return;
            }
        }

        //Check to see if it's only 1 way
        if (isOneWay)
        {
            //If it's the 1W's exit, deny entry
            if (isExit)
            {
                print("You can't get through here");
                return;
            }
            else
            {
                //If it's 1W's entrance, proceed
                player.GetComponent<PlayerController>().targetGridPos.x = toEntrance.x;
                player.GetComponent<PlayerController>().targetGridPos.z = toEntrance.z;
                player.transform.position = toEntrance;
                return;
            }
        }

        //If it's a 2W & Not Exit
        if (!isExit && !isOneWay)
        {
            //If the other entrance is something, proceed
            if (otherEntrance == null) { return; }

            //Activate other entrance
            otherEntrance.GetComponent<ShortCuts>().isActivated = true;

            //Proceed with Path
            player.GetComponent<PlayerController>().targetGridPos.x = toEntrance.x;
            player.GetComponent<PlayerController>().targetGridPos.z = toEntrance.z;
            player.transform.position = toEntrance;
        }

    }
}
