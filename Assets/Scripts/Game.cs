using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    TextMeshPro countdownText, scoreText; //Texts of the middle for countdown and game over and the score text

	[SerializeField]
	PieceGenerator pieceGenerator; //The pieceGenerator which will generate de pieces

	[SerializeField, Min(1f)]
	float newGameDelay = 3f;  //The delay before a new game

	[SerializeField]
	float fallTime;    //The initial time between two falls of the current piece

	float countdownUntilNewGame; //the countdwons value

	bool isPlaying;  //to know when a game is active

	int scoredown = 0; //the score obtain by making the piece fall faster

	float previousTime; //to help the countdown

	//When we start a new game
	void StartNewGame()
	{
		isPlaying = true;    //the game is played
		pieceGenerator.StartNewGame();  //we generate the start of the pieces
		previousTime = Time.time;  //the previous time is the current time
		scoredown = 0;         //the scoredown is reinitialized
		scoreText.SetText("Score : \n0");   //the text too
	}


	//For eache frame
	void Update()
	{
		if (isPlaying)  //if a game is played
		{
			UpdateGame();  //update the game
		}
		else if (Input.GetKeyDown(KeyCode.Space) && !isPlaying && countdownUntilNewGame <=0f) //if a game not played and not in countdown and we pressed space
		{
			countdownUntilNewGame = newGameDelay; //start the countdown
		}
		else if (countdownUntilNewGame > 0f) //if in the countdown
        {
			UpdateCountdown(); //update the countdown
        }
	}

	//When we update the game
	void UpdateGame()
	{

		if (Input.GetKeyDown(KeyCode.Space)) //if we press space
		{
			pieceGenerator.Rotate();  //We rotate the current piece
        }
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) //if we press leftarrow
        {
			pieceGenerator.Translate(-1); //We translate the current piece to the left
        }
		else if (Input.GetKeyDown(KeyCode.RightArrow)) //if we press rightarrow
		{
			pieceGenerator.Translate(1); //We tranlate the current piece to the right
		}

		//if we are in the frame rate of the falltime if we press space it is 10 time faster
		if (Time.time - previousTime > (Input.GetKey(KeyCode.DownArrow) ? fallTime / 10 : fallTime))
        {
			pieceGenerator.Fall(); //We make the piece fall
			if (Input.GetKey(KeyCode.DownArrow))
			{
				scoredown += 1; //if we are accelerating, we earn a score point
			}
			if (pieceGenerator.CheckGameOver()) //if it is game over
            {
				EndGame(); //End the game
            }
			previousTime = Time.time; //reset the delta time
        }
		int score = scoredown + pieceGenerator.Score(); //calculate the total score
        if (fallTime > 0.1) //limit the falltime to 0.1 sec
        {
			fallTime -= score / 50000; //reduce the fallTime exponential depending on the score
		}
		scoreText.SetText("Score : \n"+ score.ToString()); //display the right score
	}

	//To update the countdown
	void UpdateCountdown()
	{
		countdownUntilNewGame -= Time.deltaTime; //substract the time that was spent
		if (countdownUntilNewGame <= 0f) //if the countdown is 0 or below
		{
			countdownText.gameObject.SetActive(false); //desactivate the text
			StartNewGame(); //Start a new game
		}
		else //if the countdown not over
		{
			float displayValue = Mathf.Ceil(countdownUntilNewGame); //calculate the value to display (bottom int of the float)
			if (displayValue < newGameDelay) //if the value is bellow the delay given
			{
				countdownText.SetText("{0}", displayValue); //display the value in the text
			}
		}
	}

	//Make the end game
	void EndGame()
	{
		pieceGenerator.EndGame(); //make it in the pieceGenerator
		countdownText.SetText("GAME OVER"); //display the text of game over
		countdownText.gameObject.SetActive(true); //make it visible
		isPlaying = false; //We are no more playing
	}
}
