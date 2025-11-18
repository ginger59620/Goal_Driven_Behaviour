using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node
{
    public Node parent;
    public float cost;
    public Dictionary<string, int> state;
    public GAction action;

    public Node(Node parent, float cost, Dictionary<string, int> allstates, GAction action)
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, int>(allstates);
        this.action = action;
    }
}

public class GPlanner
{
    public Queue<GAction> plan(List<GAction> actions, Dictionary<string, int> goal, WorldState states)
    {
        List<GAction> usableAcions = new List<GAction>();
        foreach(GAction a in actions)
        {
            if (a.IsAchievable())
                usableAcions.Add(a);
        }

        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0, GWorld.Instance.GetWorld().GetStates(), null);

        bool success = BuildGraph(start, leaves, usableAcions, goal);

        if(!success)
        {
            Debug.Log("NO PLAN");
            return null;
        }

        Node cheapest = null;
        foreach(Node leaf in leaves)
        {
            if (cheapest == null)
                cheapest = leaf;
            else
            {
                if (leaf.cost < cheapest.cost)
                    cheapest = leaf;
            }
        }

        List<GAction> result = new List<GAction>();
        Node n = cheapest;
        while(n != null)
        {
            if(n.action != null)
            {
                result.Insert(0, n.action);
            }
            n = n.parent; 
        }

        Queue<GAction> queue = new Queue<GAction>();
        foreach(GAction a in result)
        {
            queue.Enqueue(a);
        }

        Debug.Log("THE PLAN IS: ");
        foreach(GAction a in queue)
        {
            Debug.Log("Q: " + a.actionName);
        }
        return queue;
    }
}
