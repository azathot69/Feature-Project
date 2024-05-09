using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour
{
    #region
    //Variables
    public int score;

    //Singleton
    public static PlayerData Instance { get; private set; }

    //Score Text
    public TMP_Text scoreText;
    #endregion

    private void Awake()
    {
        if (Instance!=null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        //Initialize Text
        scoreText.text = "Score: " + score;
    }

    private void Update()
    {
        scoreText.text = "Score: " + score;
    }
    //draw Score

    /// <summary>
    /// Resets the score to '0'
    /// </summary>
    public void ResetScore() {  score = 0; }


}
