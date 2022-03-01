using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Searcher : MonoBehaviour
{
    public MazePrinter mazePrinter;

    public Dropdown algorithmDropdown;

    public List<State> exploredStates;
    State initialState;

    bool isInstant;
    bool isAutomatic;
    int stepDelay;

    int steps;

    bool isManuallySearching; // whether the program should look for N key input to get the next step;
    bool isNextStepDone; // for the invoker. Checks if NextStep() function is done.

    public bool toShowScore; // whether score should be labeled on the states.

    public Toggle instantToggle;
    public Dropdown stepDropdown;
    public InputField delayInput;

    public Text StepCounterText;

    Frontier frontier;

    private void Start()
    {
        isInstant = true;
        isAutomatic = true;
        stepDelay = 500;

        isManuallySearching = false;
        isNextStepDone = true;

        toShowScore = true; // initially true for A* and GBFS

        steps = 0;
        UpdateStepCounter();
    }

    private void Update()
    {
        // checking if the user is pressing N while a manual pathfinding session is in action.
        if (isManuallySearching)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                bool shouldStop = NextStep();
                isManuallySearching = !shouldStop;
            }
        }

        // For the invoker. We check if NextStep() function is done so we can mark the search as done and stop the Invoker.
        if (isNextStepDone && IsInvoking("NextStep"))
        {
            CancelInvoke();
            Debug.Log("Stopped Invoker");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            toShowScore = !toShowScore;
            PrintStateScores(algorithmDropdown.value);
        }

        // Checking if the user clicks K to show/hide explored cells (only works after a search is done.
        if (isNextStepDone)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                if(exploredStates != null)
                {
                    for(int i = 0; i < exploredStates.Count; i++)
                    {
                        if(exploredStates[i].stateType == State.StateType.Path)
                        {
                            if (exploredStates[i].mazeBlock.GetComponent<Path>().isHighlighted)
                            {
                                if (!exploredStates[i].mazeBlock.GetComponent<Path>().isOptimal) // making sure block is not optimal
                                    exploredStates[i].mazeBlock.GetComponent<Path>().ReverseExplored();
                            }
                            else
                            {
                                if (!exploredStates[i].mazeBlock.GetComponent<Path>().isOptimal) // making sure block is not optimal
                                    exploredStates[i].mazeBlock.GetComponent<Path>().Explored();
                            }
                        }
                    }
                }
            }
        }
    }

    public void StartSearch()
    {
        ResetSearch();

        exploredStates = new List<State>();

        int startX = mazePrinter.startCoords.x;
        int startY = mazePrinter.startCoords.y;

        initialState = mazePrinter.mazeBlocks[startX, startY];
        Node initialNode = new Node(initialState);

        // Getting data from Search Options inputs.
        isInstant = instantToggle.isOn;
        isAutomatic = false;
        if (stepDropdown.value == 0)
        {
            isAutomatic = true;

            try
            {
                stepDelay = int.Parse(delayInput.text);
            }
            catch (FormatException)
            {
                Debug.Log("String not accepted!");
                stepDelay = 500; // if they put an invalid input, we replace it with 500ms or 0.5 seconds.
            }

            if(stepDelay < 5)
            {
                stepDelay = 5;
                delayInput.text = stepDelay.ToString();
            }
        }

        frontier = new Frontier(algorithmDropdown.value);
        PrintStateScores(algorithmDropdown.value);

        isNextStepDone = false;

        frontier.Add(initialNode);

        if (isInstant)
        {
            while (true)
            {
                if (NextStep())
                {
                    UpdateStepCounter();
                    break;
                }

            }
        }
        else
        {
            if (isAutomatic)
            {
                // we use the delay to change the steps automatically.
                float repeatEvery = ((float)stepDelay) / 1000f;
                Debug.Log(repeatEvery);
                InvokeRepeating("NextStep", 0.0f, repeatEvery);

            }
            else
            {
                // we use space bar to change the steps.
                isManuallySearching = true;
            }
        }

    }

    public bool NextStep()
    {
        if (frontier.IsEmpty())
        {
            // TODO: Add a message box saying no path found.
            Debug.Log("Couldn't find end!");
            isNextStepDone = true;
            return true;
        }
        else
        {
            steps++;
            if (!isInstant) // if not instant, we update the step counter at every step.
                UpdateStepCounter();

            Node curNode = frontier.Remove();
            exploredStates.Add(curNode.state);
            if(curNode.state.stateType == State.StateType.Path && frontier.frontierType == 3) // adding the steps to the path score when explored when we're using A* search.
                curNode.state.mazeBlock.GetComponent<Path>().AddStepsToScore(curNode.steps);

            if (curNode.state.stateType == State.StateType.End)
            {
                // Backtracking the path and coloring it.
                Node optimalPathNode = curNode.parent;
                while (true)
                {

                    if (optimalPathNode.state.stateType == State.StateType.Start)
                    {
                        isNextStepDone = true;
                        return true;
                    }
                    else
                    {
                        optimalPathNode.state.mazeBlock.GetComponent<Path>().Optimal();
                        if (frontier.frontierType == 3) // A*
                            optimalPathNode.state.mazeBlock.GetComponent<Path>().AddStepsToScore(optimalPathNode.steps); // we call the function one more time during backtracking to make sure it has the steps of the node that is in the optimal path, and not another node that passed through the same state.
                        optimalPathNode = optimalPathNode.parent;
                    }
                }
            }
            else
            {
                // visualize current state as explored if not start.
                if (curNode.state.stateType != State.StateType.Start)
                    curNode.state.mazeBlock.GetComponent<Path>().Explored();

                // look for the nodes around the node
                State.StateCoords curNodeCords = new State.StateCoords(curNode.state.x, curNode.state.y);

                // directions for {up, right, down, left}
                int[] xDirs = { 1, 0, -1, 0 };
                int[] yDirs = { 0, 1, 0, -1 };

                for (int i = 0; i < 4; i++)
                {
                    State neighborState = mazePrinter.mazeBlocks[curNodeCords.x + xDirs[i], curNodeCords.y + yDirs[i]];

                    if (!exploredStates.Contains(neighborState)) // Making sure the state hasn't been explored before.
                    {
                        if (neighborState.stateType != State.StateType.Wall) // if the neighboring state is not a wall, we add it to the frontier.
                        {
                            Action.Actions action = (Action.Actions)i; // xDirs and yDirs match the index of the loop and so does the actions.
                            if(frontier.frontierType == 3) // A*
                            {
                                Node neighborNode = new Node(neighborState, curNode, action, curNode.steps+1);
                                frontier.Add(neighborNode);
                            } else
                            {
                                Node neighborNode = new Node(neighborState, curNode, action);
                                frontier.Add(neighborNode);
                            }
                            
                        }
                    }

                }
                isNextStepDone = false;
                return false;
            }
        }
    }

    public void ResetPathBlocks()
    {
        if (exploredStates != null)
        {
            if (exploredStates.Count > 0)
            {
                foreach (State state in exploredStates)
                {
                    if (state.stateType == State.StateType.Path) // making sure to change colors of path blocks only, not start and finish blocks.
                        state.mazeBlock.GetComponent<Path>().Normal();
                }
            }
        }
    }

    // Called when a new map is generated or when "Cancel Search" is pressed.
    public void ResetSearch()
    {
        ResetPathBlocks();

        if (exploredStates != null)
        {
            exploredStates.Clear();
        }

        if(frontier != null)
        {
            frontier.Empty();
        }

        isManuallySearching = false;
        isNextStepDone = true;

        if (IsInvoking("NextStep"))
        {
            CancelInvoke();
            Debug.Log("Stopped Invoker");
        }

        steps = 0;
        UpdateStepCounter();
    }

    public void MakeSearchInstant()
    {
        isInstant = instantToggle.isOn;
        if (isInstant)
        {
            // delayInput.text = "0";
            stepDropdown.interactable = false;
            // stepDropdown.value = 0;
            delayInput.interactable = false;
        } else
        {
            //delayInput.text = "";
            stepDropdown.interactable = true;
            if (isAutomatic)
                delayInput.interactable = true;
        }
    }

    public void ChangeStepMethod()
    {
        isAutomatic = false;
        if(stepDropdown.value == 0)
            isAutomatic = true;
        
        if (isAutomatic)
        {
            delayInput.interactable = true;
        }
        else
        {
            delayInput.interactable = false;
        }
    }

    
    private void PrintStateScores(int algorithm)
    {
        if (algorithm >= 2 && toShowScore) // if the solving algorithm is GBFS or A*, show state scores.
        {
            mazePrinter.StateScoresVisibility(true, algorithm);
        }
        else
        {
            mazePrinter.StateScoresVisibility(false, algorithm);
        }
    }

    public void UpdateStepCounter()
    {
        StepCounterText.text = "Steps: " + steps;
    }
}
