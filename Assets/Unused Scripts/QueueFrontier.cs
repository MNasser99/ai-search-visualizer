using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueFrontier
{
    Queue<Node> frontier;

    public QueueFrontier()
    {
        frontier = new Queue<Node>();
    }

    public void Add(Node node)
    {
        frontier.Enqueue(node); // Enqueue() adds object to end of Queue.
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
        }
        else
        {
            Node node = this.frontier.Dequeue(); // Dequeue() gives us the first element in the Queue while removing it from the Queue at the same time.
            return node;
        }
    }
}
