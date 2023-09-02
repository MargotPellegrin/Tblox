using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceGenerator : MonoBehaviour
{
    [SerializeField]
    PieceObject[] prefabs; //Contains the PieceObject that can be generated

    PieceObject current, next; //Will contain the current piece (in movement) and the next piece to be play

    Transform[,] grid = new Transform[10, 27]; //The grid that will contain simple cubes transform to make the calculations

    int score = 0; //The score that will increase destroying lines

    int countRotate = 0;

    //Access to the score value
    public int Score()
    {
        return score;
    }

    //Make the rotation between pieces, generate the new next piece
    public void NextPiece()
    {
        current = next; //The next piece become the current
        current.GetInGame(); //The current piece enter in the game
        next = GetInstance(); //The next piece is generated
    }

    //All we need to start a new game
    public void StartNewGame()
    {
        current = GetInstance(); //Create a current piece that has not been next
        current.GetInGame(); //Enter the current piece in the game
        next = GetInstance(); //Make the next piece
        score = 0; //reset the score to 0
    }

    //Enable to create a new instance of a random piece object
    PieceObject GetInstance()
    {
        PieceObject instance = prefabs[Random.Range(0, prefabs.Length)].GetInstance(); //Instanciate the random pieceObject
        instance.transform.SetParent(transform, false); //Enable the transform to be with the PieceGenerator parent and right place
        return instance; //return the instance
    }

    //When a piece cannot move anymore (is put on another piece), we add it to the grid
    public void AddToGrid()
    {
        foreach (Transform children in current.transform) //We do for every children of the pieceObject i.e every cube
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x); //Get the x position in int
            int roundedY = Mathf.RoundToInt(children.transform.position.y); //Get the y position in int
            grid[roundedX, roundedY] = children; //The positions are the coordinates of the grid so we place the child here
        }
    }

    //We are able to rotate the current PieceObject
    public void Rotate()
    {
        current.Rotate(1);  //We make the rotation (90°)
        if (!ValidMove())   //If the move is not possible :
        {
            current.Rotate(-1); //We make the reverse move
            countRotate += 1;   //We tried one rotation
            TryToRotate();      //When the rotation is near a wall, we could rotate but the piece need to translate
        }
        else                //If the move is possible :
        {
            countRotate = 0;    //We had a rotation so the count reset
        }
    }

    //Here is were we try to rotate the piece near a wall
    public void TryToRotate()
    {
        if (current.transform.position.x > 5 && countRotate <=2) //if the piece is near right wall and we are not stuck in a loop
        {
            Translate(-1);   //We try to translate left
            Rotate();        //We try to rotate, if not possible, it will retry to translate
        }
        else if (current.transform.position.x <= 5 && countRotate <=2) //if the piece is near left wall and we are not stuck in a loop
        {
            Translate(1);    //We try to translate right
            Rotate();        //We try to rotate, if not possible, it will retry to translate
        }
    }

    //Make the current piece fall
    public void Fall()
    {
        current.Fall(-1); //We make it fall
        if (!ValidMove()) //If the move is not valid it means the piece is on another piece or the floor
        {
            current.Fall(1);  //We reverse the move
            AddToGrid();      //We add the current piece to the grid
            CheckLine();      //We check if we made lines
            current.enabled = false; //We disable the current piece
            NextPiece();     //We make the rotation of pieces
        }
    }

    //Make the translation of the piece made by the player
    public void Translate(float x)
    {
        current.Translate(x); //We make the translation
        if (!ValidMove())     //If the move is not valid
        {
            current.Translate(-x);       //We reverse the move, nothing happens
        }
    }

    //Function to check if a move is valid or more likely if the current state id possible
    public bool ValidMove()
    {
        foreach (Transform children in current.transform) //for all the children in the current piece
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x); //We take the x position in int
            int roundedY = Mathf.RoundToInt(children.transform.position.y); //We take the y position in int

            //checking if the move is out of the arena
            if (roundedX < 0 || roundedX >= 10 || roundedY < 0 || roundedY >=27) //27 to enable the piece above
            {
                return false; //not valid
            }
            //if there is already a piece not valid
            else if(grid[roundedX,roundedY] != null) 
            {
                return false;
            }
        }
        return true; //else if no child where not valid, it is valid
    }

    //To check if we made lines
    public void CheckLine()
    {
        for (int i = 19; i>=0; i--) // for each line of the grid from top to bot if not, ordonance problem and doesn't work as expected
        {
            if (HasLine(i))        //check if it is a complete line if so :
            {
                DestroyLine(i);   //the line is destroyed
                LineFall(i);      //The other lines must fall
            }
        }
    }

    //Check is a line is complete
    public bool HasLine(int i)
    {
        for(int j = 0; j<10; j++) //for each place in the line
        {
            if(grid[j,i] == null) //check if it is null
            {
                return false; //if so, the line is incomplete
            }
        }
        return true; //if no one was null, the line is complete
    }

    //Make the lines above the destroyed line fall
    public void LineFall(int i)
    {
        for(int j=i+1; j<20; j++)  //for each line above
        {
            for(int n=0; n<10; n++) //for each cube in this line
            {
                if(grid[n,j] != null) //if the cube is not empty
                {
                    grid[n, j - 1] = grid[n, j]; //it falls into the grid
                    grid[n, j].position -= new Vector3(0, 1, 0); //it falls as a gameobject
                    grid[n, j] = null; //the cube above become null
                }
            }
        }
    }

    //Make the line destroyed
    public void DestroyLine(int i)
    {
        for (int j = 0; j<10; j++) //for each cube in the line
        {
            if(grid[j,i] != null)  //if the cube is not null (condition for the end of the party, when we want to destroy all)
            {
                Destroy(grid[j, i].gameObject); //destroy the cube as a gameObject
                grid[j, i] = null;              //make it null in the grid
            }
        }
        score += 10;   //add 10 to the score
    }

    //Check if we have a game over
    public bool CheckGameOver()
    {
        for(int i=20; i<27; i++) //for each line above 20 (above the limit)
        {
            for(int j=0; j<10; j++) //for each cube in this line
            {
                if(grid[j,i] != null)  //if we put a cube
                {
                    return true; //the game is over
                }
            }
        }
        return false; //if no cube above 20 then game not over
    }


    //What to do when the game is over
    public void EndGame()
    {
        Destroy(current.gameObject); //destroy the current piece to avoid the loop of next
        Destroy(next.gameObject); //destroy also the next object
        for (int i=0; i<27; i++) //for all the lines
        {
            DestroyLine(i); //destroy the line
        }
    }
}
