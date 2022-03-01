using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeManager
{
    public int GRID_WIDTH;
    public int GRID_HEIGHT;

    public int mazeSeed;

    const int NORTH = 0;
    const int EAST = 1;
    const int SOUTH = 2;
    const int WEST = 3;

    public char[,] grid;
    public int[,] stateScores;

    public State.StateCoords startCoords;
    public State.StateCoords endCoords;

    public void MakeMaze(int width, int height, int seed, bool isRandomSAndE)
    {
        GRID_WIDTH = width;
        GRID_HEIGHT = height;

        grid = new char[GRID_WIDTH, GRID_HEIGHT];
        stateScores = new int[GRID_WIDTH, GRID_HEIGHT];

        int mazeSeed = seed;
        UnityEngine.Random.InitState(mazeSeed);

        ResetGrid();
        Visit(1, 1);
        AddStartAndEnd(isRandomSAndE);
        AddStateScores();
    }

    void ResetGrid()
    {
        // Fills the grid with walls
        for (int i = 0; i < GRID_WIDTH; i++)
        {
            for (int j = 0; j < GRID_HEIGHT; j++)
            {
                grid[i, j] = '#';
            }
        }
    }

    bool IsInBounds(int x, int y)
    {
        // Returns "true" if x and y are both in-bounds
        if (x < 0 || x >= GRID_WIDTH) return false;
        if (y < 0 || y >= GRID_HEIGHT) return false;
        return true;
    }

    void Visit(int x, int y)
    {
        // Starting at the given index, recursively visits every direction in a
        // randomized order.
        // Set my current location to be an empty passage.
        grid[x, y] = ' ';

        int[] dirs = new int[4];
        dirs[0] = NORTH;
        dirs[1] = EAST;
        dirs[2] = SOUTH;
        dirs[3] = WEST;

        for (int i = 0; i < 4; i++)
        {
            int r = (int)UnityEngine.Random.Range(0, 4);
            int temp = dirs[r];
            dirs[r] = dirs[i];
            dirs[i] = temp;
        }

        // Loop through every direction and attempt to Visit that direction.
        for (int i = 0; i < 4; ++i)
        {
            // dx,dy are offsets from current location. Set them based
            // on the next direction I wish to try.
            int dx = 0, dy = 0;
            switch (dirs[i])
            {
                case NORTH: dy = -1; break;
                case SOUTH: dy = 1; break;
                case EAST: dx = 1; break;
                case WEST: dx = -1; break;
            }
            // Find the (x,y) coordinates of the grid cell 2 spots
            // away in the given direction.
            int x2 = x + (dx * 2);
            int y2 = y + (dy * 2);
            if (IsInBounds(x2, y2))
            {
                if (grid[x2, y2] == '#')
                {
                    // (x2,y2) has not been visited yet... knock down the
                    // wall between my current position and that position
                    grid[x2 - dx, y2 - dy] = ' ';
                    // Recursively Visit (x2,y2)
                    Visit(x2, y2);
                }
            }
        }

    }

    void AddStartAndEnd(bool isRandomSAndE)
    {
        if (isRandomSAndE)
        {
            while (true)
            {
                int startX = (int)UnityEngine.Random.Range(1, GRID_WIDTH);
                int startY = (int)UnityEngine.Random.Range(1, GRID_HEIGHT);

                if (grid[startX, startY] == ' ')
                {
                    grid[startX, startY] = 'S';
                    startCoords = new State.StateCoords(startX, startY);
                    break;
                }
            }

            while (true)
            {
                int endX = (int)UnityEngine.Random.Range(1, GRID_WIDTH);
                int endY = (int)UnityEngine.Random.Range(1, GRID_HEIGHT);

                if (grid[endX, endY] == ' ')
                {
                    grid[endX, endY] = 'E';
                    endCoords = new State.StateCoords(endX, endY);
                    break;
                }
            }
        }
        else
        {
            grid[1, 1] = 'S';
            grid[GRID_WIDTH - 2, GRID_HEIGHT - 2] = 'E';
        }

    }

    void AddStateScores()
    {
        for (int i = 0; i < GRID_WIDTH; i++)
        {
            for (int j = 0; j < GRID_HEIGHT; j++)
            {
                if(grid[i, j] != '#')
                {
                    int score = Mathf.Abs(i - endCoords.x) + Mathf.Abs(j - endCoords.y);
                    stateScores[i, j] = score;
                }
            }
        }
    }


}
