using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public State state;
    public Node parent;
    public Action.Actions action;
    public bool isInitial;
    public int steps; // steps until we reached this node. For A*

    public Node(State _state) // for initial State.
    {
        state = _state;
        isInitial = true;
        steps = 0;
    }

    public Node(State _state, Node _parent, Action.Actions _action) // for all other states
    {
        state = _state;
        parent = _parent;
        action = _action;
        isInitial = false;
    }

    public Node(State _state, Node _parent, Action.Actions _action, int _steps) // for all other states
    {
        state = _state;
        parent = _parent;
        action = _action;
        isInitial = false;
        steps = _steps;
    }
}
