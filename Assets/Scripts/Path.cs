using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public bool isExplored;
    public bool isOptimal;

    public bool isHighlighted; // checks if the path is colored with the explored state color.
    public bool showScoreOnHover;

    SpriteRenderer spriteRenderer;

    static bool isAStar;
    int score;
    int steps;

    private void Start()
    {
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        isAStar = false;
        score = 0;
        steps = 0;

        isExplored = false;
        isOptimal = false;
        isHighlighted = false;

        showScoreOnHover = false;
    }

    public void Explored()
    {
        isExplored = true;
        isHighlighted = true;
        spriteRenderer.color = new Color(1f, 0.97f, 0.57f); // 255, 247, 146
    }

    public void ReverseExplored() // reverses path to original color without making isExplored = false;
    {
        isHighlighted = false;
        spriteRenderer.color = new Color(0.73f, 0.73f, 0.73f); // 185, 185, 185
    }

    public void Optimal()
    {
        isOptimal = true;
        // spriteRenderer.color = new Color(0.57f, 0.97f, 0.44f); // green 146, 248, 113
        spriteRenderer.color = new Color(0.76f, 1f, 0.73f); // green 193, 255, 185
        // spriteRenderer.color = new Color(1f, 0.6f, 0.6f); // red 255, 153, 153
    }

    public void Normal()
    {
        isExplored = false;
        isOptimal = false;
        isHighlighted = false;
        spriteRenderer.color = new Color(0.73f, 0.73f, 0.73f); // 185, 185, 185
    }

    public void UpdateScoreText(bool makeVisible, MazeManager mazeManager, int x, int y, int algo)
    {
        if (algo == 3)
        {
            isAStar = true;
            if(makeVisible)
                AddStepsToScore();
            else
                showScoreOnHover = false;
        }
        else if (algo == 2)
        {
            isAStar = false;
        }

        if (makeVisible)
        {
            score = mazeManager.stateScores[x, y];
            transform.GetChild(0).GetComponent<TextMesh>().text = score.ToString();
        } else
        {
            transform.GetChild(0).GetComponent<TextMesh>().text = "";
        }
    }


    // since steps are relative to the node, not the state, we can not save it permanently in this object, so we have to pull it directly from the node. Hence, we call the function again once we find the optimal path to make sure that the steps saved here are not from an alternative path that the search algorithm took during its solving process.
    public void AddStepsToScore(int _steps)
    {
        Path.isAStar = true;
        showScoreOnHover = true;

        steps = _steps;
        int newScore = score + steps;
        transform.GetChild(0).GetComponent<TextMesh>().text = newScore.ToString();
    }

    public void AddStepsToScore()
    {
        Path.isAStar = true;
        showScoreOnHover = true;
        int newScore = score + steps;
        transform.GetChild(0).GetComponent<TextMesh>().text = newScore.ToString();
    }

    // Showing the format (Score + Steps) when hovering over path.
    private void OnMouseEnter()
    {
        if (isAStar && showScoreOnHover)
        {
            transform.GetChild(0).GetComponent<TextMesh>().text = score.ToString() + "\n+\n" + steps.ToString();
            transform.GetChild(0).GetComponent<TextMesh>().characterSize = 0.1f;
        }
    }

    private void OnMouseExit()
    {
        if (isAStar && showScoreOnHover)
        {
            int newScore = score + steps;
            transform.GetChild(0).GetComponent<TextMesh>().text = newScore.ToString();
            transform.GetChild(0).GetComponent<TextMesh>().characterSize = 0.17f;
        }
    }
}
