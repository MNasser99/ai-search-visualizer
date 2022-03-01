using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackFrontier
{
    Stack<Node> frontier;

    public StackFrontier()
    {
        frontier = new Stack<Node>();
    }

    public void Add(Node node)
    {
        frontier.Push(node); // Push() adds object to end of stack.
    }

    public bool IsEmpty()
    {
        return (frontier.Count <= 0);
    }

    public void Empty()
    {
        frontier.Clear();
    }

    public Node Remove()
    {
        if (this.IsEmpty())
        {
            Debug.Log("Frontier Empty");
            return null;
        } else
        {
            Node node = this.frontier.Pop(); // Pop() gives us the last element in the stack while removing it from the stack at the same time.
            return node;
        }
    }

}
