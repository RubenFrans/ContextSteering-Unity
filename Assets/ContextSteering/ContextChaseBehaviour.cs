using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextChaseBehaviour : BaseContextBehavior
{

    [SerializeField] private float m_ChaseDistance;

    [SerializeField] private GameObject m_Target;
    public GameObject ChaseTarget { get { return m_Target;  } set { m_Target = value; } }

    public override List<float> GetDangerMap()
    {
        throw new System.NotImplementedException();
    }

    public override List<float> GetInterestMap(Vector2 agentPosition, ref List<Vector2> directions)
    {
        Vector2 targetPos = m_Target.transform.position;
        Vector2 toTarget = targetPos - agentPosition;
        m_InterestMap.Clear();
        for (int i = 0; i < directions.Count; i++)
        {
            float interestAmount = Vector2.Dot(toTarget, directions[i]);
           // Mathf.Clamp(interestAmount, 0.0f, interestAmount);

            m_InterestMap.Insert(i, interestAmount);
        }

        return m_InterestMap;
    }
}
