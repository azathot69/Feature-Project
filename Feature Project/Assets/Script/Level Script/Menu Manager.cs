using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    /// <summary>
    /// Goes to certain scene
    /// </summary>
    /// <param name="roomNum">Which room to go to</param>
    public void GoToScene(int roomNum)
    {
        SceneManager.LoadScene(roomNum);
    }

    /// <summary>
    /// Quits game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game End");
    }
}
