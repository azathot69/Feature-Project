using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour
{
    #region

    //Singleton
    public static PlayerData Instance { get; private set; }


    //Variables
    public int score;

    public int endScore;

    [SerializeField]
    private bool isEnd = false;
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
        scoreText.text = "Score : $" + score;
    }

    private void Update()
    {
        if (!isEnd) {
            scoreText.text = "Score: $" + score;
        }
        else if(isEnd)
        {
            scoreText.text = "You earned $" + endScore + " En!";
        }
        
    }
    //draw Score

    /// <summary>
    /// Resets the score to '0'
    /// </summary>
    public void ResetScore() {  
        score = 0;
        endScore = 0;

}


}
