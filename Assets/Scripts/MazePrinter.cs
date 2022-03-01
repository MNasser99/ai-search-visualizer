using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazePrinter : MonoBehaviour
{
    public MazeManager mazeManager;
    public char[,] myGrid;

    public int lastWidth = 39;
    public int lastHeight = 39;

    int widthMin = 5;
    int widthMax = 201;
    int heightMin = 5;
    int heightMax = 201;

    public Searcher searcher;

    public Transform maze;
    public GameObject path;
    public GameObject wall;
    public GameObject start;
    public GameObject end;

    public InputField WidthTextInput;
    public InputField HeightTextInput;
    public InputField SeedTextInput;
    public Dropdown SAndEDropdown;

    public Camera mainCamera;

    public Text SeedTextLabel;

    public State[,] mazeBlocks;

    public State.StateCoords startCoords;
    public State.StateCoords endCoords;

    void Start()
    {
        BuildMaze(lastWidth, lastHeight, true);
    }

    public void GenerateMazeBtn()
    {
        // Removing the past generated maze before generating a new maze.
        foreach (Transform child in maze)
        {
            GameObject.Destroy(child.gameObject);
        }

        // clearing the searchers information from the prior map.
        searcher.ResetSearch();

        // If the width and height inputs are filled, we get their values. Else we use the last width and height values used.
        int widthText = lastWidth;
        int heightText = lastHeight;

        if (!WidthTextInput.text.Equals("") && !HeightTextInput.text.Equals(""))
        {
            // try catch to make sure the value inserted in the inputs is a number and not a string.
            try
            {
                widthText = int.Parse(WidthTextInput.text);
                heightText = int.Parse(HeightTextInput.text);

                if (widthText < widthMin || widthText > widthMax)
                {
                    if (widthText < widthMin)
                        widthText = widthMin;

                    else if (widthText > widthMax)
                        widthText = widthMax;

                    WidthTextInput.text = widthText.ToString();
                }

                if (heightText < heightMin || heightText > heightMax)
                {
                    if (heightText < heightMin)
                        heightText = heightMin;

                    else if (heightText > heightMax)
                        heightText = heightMax;

                    HeightTextInput.text = heightText.ToString();
                }


                lastWidth = widthText;
                lastHeight = heightText;
            }
            catch (FormatException)
            {
                Debug.Log("String not accepted!");
            }
        }
        


        // making sure width and height are odd numbers.
        if (widthText % 2 == 0)
            widthText++;

        if (heightText % 2 == 0)
            heightText++;

        // Checking if Start and End should be random or in the corners
        bool isSAndERandom = true;
        if (SAndEDropdown.value == 1)
        {
            isSAndERandom = false;
        }

        if (!SeedTextInput.text.Equals("")) // If the seed input is not empty, generate maze with seed.
        {
            int seedText = int.Parse(SeedTextInput.text);
            BuildMaze(widthText, heightText, seedText, isSAndERandom);
        } else // else, generate map based on width and height only.
        {
            BuildMaze(widthText, heightText, isSAndERandom);
        }
    }

    public void BuildMaze(int width, int height, bool isSAndERandom) // function for building the maze using width and height.
    {
        mazeManager = new MazeManager();

        int timeMax = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        int timeMin = (int)(new DateTime(1990, 1, 1) - new DateTime(1970, 1, 1)).TotalSeconds;
        int seed = (int)UnityEngine.Random.Range(timeMin, timeMax);
        UpdateSeedText(seed);

        mazeManager.MakeMaze(width, height, seed, isSAndERandom);
        myGrid = mazeManager.grid;
        PrintGrid(width, height);


        UpdateCameraSize(width, height);
    }

    public void BuildMaze(int width, int height, int seed, bool isSAndERandom) // function for building the maze using width, height and seed.
    {
        mazeManager = new MazeManager();

        UpdateSeedText(seed);

        mazeManager.MakeMaze(width, height, seed, isSAndERandom);
        myGrid = mazeManager.grid;
        PrintGrid(width, height);

        UpdateCameraSize(width, height);
    }

    void PrintGrid(int width, int height) // function for printing out the maze.
    {
        mazeBlocks = new State[width, height]; // an array to save all the maze elements' instances.

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int vecX = x - ((int)(0.5 * width));
                int vecY = y - ((int)(0.5 * height));

                if (myGrid[x, y] == ' ')
                {
                    GameObject mazeElementObject = Instantiate(path, new Vector3(vecX, vecY), Quaternion.identity, maze).gameObject;
                    mazeBlocks[x, y] = new State(x, y, mazeElementObject, State.StateType.Path, mazeManager.stateScores[x, y]);
                }
                else if (myGrid[x, y] == '#')
                {
                    GameObject mazeElementObject = Instantiate(wall, new Vector3(vecX, vecY), Quaternion.identity, maze).gameObject;
                    mazeBlocks[x, y] = new State(x, y, mazeElementObject, State.StateType.Wall);
                }
                else if (myGrid[x, y] == 'S')
                {
                    GameObject mazeElementObject = Instantiate(start, new Vector3(vecX, vecY), Quaternion.identity, maze).gameObject;
                    mazeBlocks[x, y] = new State(x, y, mazeElementObject, State.StateType.Start, mazeManager.stateScores[x, y]);
                    startCoords = new State.StateCoords(x, y);
                }
                else if (myGrid[x, y] == 'E')
                {
                    GameObject mazeElementObject = Instantiate(end, new Vector3(vecX, vecY), Quaternion.identity, maze).gameObject;
                    mazeBlocks[x, y] = new State(x, y, mazeElementObject, State.StateType.End, mazeManager.stateScores[x, y]);
                    endCoords = new State.StateCoords(x, y);
                }
            }
        }

    }

    public void StateScoresVisibility(bool makeVisible, int algo) // shows or hides state scores.
    {
        for (int i = 0; i < lastWidth; i++)
        {
            for (int j = 0; j < lastWidth; j++)
            {
                if(mazeBlocks[i, j].stateType == State.StateType.Path) // score only shows on path blocks. Start and End blocks also have score but it doesn't show.
                {
                    if (makeVisible)
                    {
                        mazeBlocks[i, j].mazeBlock.GetComponent<Path>().UpdateScoreText(true, mazeManager, i, j, algo);
                    } else if(!makeVisible)
                    {
                        mazeBlocks[i, j].mazeBlock.GetComponent<Path>().UpdateScoreText(false, mazeManager, i, j, algo);
                    }
                }
            }
        }
    }

    public void UpdateCameraSize(int width, int height)
    {
        float numb = (float)width;
        float numbMin = (float)widthMin;
        if (height > width)
        {
            numb = (float)height;
            numbMin = (float)heightMin;
        }
            

        float addition = 4f + ((numb - numbMin) * 0.5f); // 4 is the camera size for the minimum maze size which is 5. We add 0.5 for each +1 we add to the maze size.
        mainCamera.orthographicSize = addition;
    }

    public void UpdateSeedText(int seed)
    {
        SeedTextLabel.text = "Seed: " + seed;
    }
}
