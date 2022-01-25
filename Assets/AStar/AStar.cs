using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class GOAPAction
{
    private HashSet<KeyValuePair<string, object>> preconditions;
    private HashSet<KeyValuePair<string, object>> effects;

    private float actionCost = 1.0f;
    public float ActionCost { get { return actionCost; } set { actionCost = value; } }

    private bool inRange = false;

    public GameObject target;

    public void DoReset()
    {
        inRange = false;
        target = null;
        Reset();
    }

    public abstract void Reset();

    public abstract void IsDone();

    public abstract bool CheckPreconditions(GameObject agent);

    public abstract bool Perform();


    public bool IsInRange{get{ return IsInRange; } set{ IsInRange = value; } }

    public void AddPrecondition(string key, object value)
    {
        preconditions.Add(new KeyValuePair<string, object>(key, value));
    }

    public void RemovePrecondition(string key)
    {
        preconditions.RemoveWhere(x => x.Key == key);
    }

    public void AddEffect(string key, object value)
    {
        effects.Add(new KeyValuePair<string, object>(key, value));
    }

    public void RemoveEffect(string key)
    {
        effects.RemoveWhere(x => x.Key == key);
    }
    public HashSet<KeyValuePair<string, object>> Preconditions
    {
        get
        {
            return preconditions;
        }
    }

    public HashSet<KeyValuePair<string, object>> Effects
    {
        get
        {
            return effects;
        }
    }

}

public class MoveToAction : GOAPAction
{

    private NavMeshAgent navMeshAgent;

    public override bool CheckPreconditions(GameObject agent)
    {
        throw new System.NotImplementedException();
    }

    public override void IsDone()
    {
        throw new System.NotImplementedException();
    }

    public override bool Perform()
    {
        throw new System.NotImplementedException();
    }

    public override void Reset()
    {
        throw new System.NotImplementedException();
    }
}

public class AStar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
