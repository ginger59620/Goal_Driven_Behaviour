using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SubGoal
{
    public Dictionary<string, int> sgoals;
    public bool remove;

    public SubGoal(string s, int i, bool r)
    {
        sgoals = new Dictionary<string, int>();
        sgoals.Add(s, i);
        remove = r;
    }
}
public class GAgent : MonoBehaviour
{
    public List<GAction> action = new List<GAction>();
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();

    GPlanner planner;
    Queue<GAction> actionsQueue;
    public GAction currentAction;
    SubGoal currentGoal;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        GAction[] acts = this.GetComponents<GAction>();
        foreach (GAction a in acts)
            action.Add(a);
    }

    bool invoke = false;
    void CompleteAction()
    {
        currentAction.running = false;
        currentAction.PostPerform();
        invoke = false;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if(currentAction != null && currentAction.running)
        {
            if(currentAction.agent.hasPath && currentAction.agent.remainingDistance < 1f)
            {
                if(!invoke)
                {
                    Invoke("CompleteAction", currentAction.duration);
                    invoke = true;
                }
            }
            return;
        }

        if(planner == null || actionsQueue == null )
        {
            planner = new GPlanner();

            var sortedGoals = from entry in goals orderby entry.Value descending select entry;

            foreach(KeyValuePair<SubGoal, int> sg in sortedGoals)
            {
                actionsQueue = planner.plan(action, sg.Key.sgoals, null);
                if(actionsQueue != null)
                {
                    currentGoal = sg.Key;
                    break;
                }
            }
        }

        if(actionsQueue != null && actionsQueue.Count == 0)
        {
            if(currentGoal.remove)
            {
                goals.Remove(currentGoal);
            }
            planner = null;
        }

        if(actionsQueue != null && actionsQueue.Count > 0)
        {
            currentAction = actionsQueue.Dequeue();
            if(currentAction.PrePerform())
            {
                if (currentAction.target == null && currentAction.targetTag != "")
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);

                if(currentAction.target != null)
                {
                    currentAction.running = true;
                    currentAction.agent.SetDestination(currentAction.target.transform.position);
                }
            }
            else
            {
                actionsQueue = null;
            }
        }
    }
  
}
