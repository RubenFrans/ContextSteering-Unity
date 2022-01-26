using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum LocalDirection
{
}

public class ContextDirectionalSteering : BaseContextBehavior
{
    private new void Start()
    {
        base.Start();
    }

    public override List<float> GetDangerMap(Vector2 agentPostion, ref List<Vector2> directions)
    {
        return m_DangerMap;
    }

    public override List<float> GetInterestMap(Vector2 agentPostion, ref List<Vector2> directions)
    {
        m_InterestMap = new List<float>(new float[directions.Count]);

        for (int i = 0; i < directions.Count; i++)
        {
            m_InterestMap[i] = Vector2.Dot(gameObject.transform.up * 0.8f, directions[i]);
        }
        
        return m_InterestMap;
    }

}
