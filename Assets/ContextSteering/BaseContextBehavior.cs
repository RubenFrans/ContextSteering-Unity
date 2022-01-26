using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseContextBehavior : MonoBehaviour
{
    protected List<float> m_InterestMap;
    protected List<float> m_DangerMap;


    protected void Start()
    {
    }

    public void InitializeContextMaps(int amountOfDirections)
    {
        m_InterestMap = new List<float>(new float[amountOfDirections]);
        m_DangerMap = new List<float>(new float[amountOfDirections]);
    }

    abstract public List<float> GetInterestMap(Vector2 agentPostion, ref List<Vector2> directions);
    abstract public List<float> GetDangerMap(Vector2 agentPostion, ref List<Vector2> directions);
}
