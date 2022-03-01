using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public int x;
    public int y;
    public GameObject mazeBlock;

    public int score; // for GBFS and A*

    public enum StateType
    {
        Path,
        Wall,
        Start,
        End
    }

    public StateType stateType;

    public State(int _x, int _y, GameObject _mazeBlock, StateType _stateType)
    {
        x = _x;
        y = _y;
        mazeBlock = _mazeBlock;
        stateType = _stateType;
    }

    public State(int _x, int _y, GameObject _mazeBlock, StateType _stateType, int _score)
    {
        x = _x;
        y = _y;
        mazeBlock = _mazeBlock;
        stateType = _stateType;
        score = _score;
    }

    public struct StateCoords
    {
        public int x;
        public int y;

        public StateCoords(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
