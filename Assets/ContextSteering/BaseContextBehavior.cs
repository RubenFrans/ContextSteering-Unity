using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseContextBehavior : MonoBehaviour
{
    protected List<float> m_InterestMap;
    protected List<float> m_DangerMap;


    protected void Start()
    {
        m_InterestMap = new List<float>();
        m_DangerMap = new List<float>();
    }

    abstract public List<float> GetInterestMap(Vector2 agentPostion, ref List<Vector2> directions);

    abstract public List<float> GetDangerMap(Vector2 agentPostion, ref List<Vector2> directions);
}
