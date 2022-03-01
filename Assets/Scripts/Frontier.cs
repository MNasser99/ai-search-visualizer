using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frontier
{

    Stack<Node> sFrontier;
    Queue<Node> qFrontier;
    List<Node> lFrontier;
    public int frontierType;

    public Frontier(int type) // 0: depth first, 1: breadth first.
    {
        frontierType = type;
        if (frontierType == 0)
        {
            sFrontier = new Stack<Node>();
        } else if (frontierType == 1)
        {
            qFrontier = new Queue<Node>();
        } else // GBFS and A* use lists
        {
            lFrontier = new List<Node>();
        }
        
    }

    public void Add(Node node)
    {
        if (frontierType == 0)
        {
            sFrontier.Push(node); // Push() adds object to end of stack.
        }
        else if (frontierType == 1)
        {
            qFrontier.Enqueue(node); // Enqueue() adds object to end of Queue.
        } else
        {
            lFrontier.Add(node);
        }
        
    }

    public bool IsEmpty() // checking if the frontier is empty
    {
        if (frontierType == 0)
        {
            return (sFrontier.Count == 0);
        }
        else if (frontierType == 1)
        {
            return (qFrontier.Count == 0);
        } else
        {
            return (lFrontier.Count == 0);
        }
        
    }

    public void Empty() // emptying the frontier
    {
        if (frontierType == 0)
        {
            sFrontier.Clear();
        }
        else if (frontierType == 1)
        {
            qFrontier.Clear();
        } else
        {
            lFrontier.Clear();
        }
        
    }

    public Node Remove()
    {
        if (this.IsEmpty())
        {
            Debug.Log("Frontier Empty");
            return null;
        }
        else
        {
            Node node;
            if (frontierType == 0) // DFS
            {
                node = sFrontier.Pop(); // Pop() gives us the last element in the stack while removing it from the stack at the same time.
            }
            else if (frontierType == 1) // BFS
            {
                node = qFrontier.Dequeue(); // Dequeue() gives us the first element in the Queue while removing it from the Queue at the same time.
            }
            else// GBFS and A*
            {
                // decide which node to remove from lFrontier based on state score only(GBFS), or state score and steps (A*)
                int bestScore = lFrontier[0].state.score;
                if (frontierType == 3) //A*
                    bestScore += lFrontier[0].steps;

                int bestScoreInd = 0;

                for(int i = 1; i < lFrontier.Count; i++)
                {
                    int curScore = lFrontier[i].state.score;
                    if (frontierType == 3) //A*
                        curScore += lFrontier[i].steps;


                    if (curScore < bestScore)
                    {
                        bestScore = curScore;
                        bestScoreInd = i;
                    }
                }

                node = lFrontier[bestScoreInd];
                lFrontier.RemoveAt(bestScoreInd);
            }

            return node;
        }
    }
}
