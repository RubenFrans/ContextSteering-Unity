using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextChaseBehaviour : BaseContextBehavior
{

    const string CHASE_TARGETS_TAG = "ChaseTarget";

    [SerializeField] private float m_ChaseDistance;

    [SerializeField] private GameObject m_Target;
    public GameObject ChaseTarget { get { return m_Target;  } set { m_Target = value; } }

    private GameObject[] m_ChaseTargets;

    private new void Start()
    {
        base.Start();
        m_ChaseTargets = GameObject.FindGameObjectsWithTag(CHASE_TARGETS_TAG);
    }

    public override List<float> GetDangerMap()
    {
        throw new System.NotImplementedException();
    }

    public override List<float> GetInterestMap(Vector2 agentPosition, ref List<Vector2> directions)
    {
        m_InterestMap.Clear();
        foreach(GameObject target in m_ChaseTargets)
        {
            Vector2 targetPos = target.transform.position;
            Vector2 toTarget = targetPos - agentPosition;
            for (int i = 0; i < directions.Count; i++)
            {
                float interestAmount = Vector2.Dot(toTarget, directions[i]);
                Debug.Log(i);
                if(i >= m_InterestMap.Count)
                {
                    m_InterestMap.Insert(i, interestAmount);

                }
                else
                {
                    m_InterestMap.Insert(i, m_InterestMap[i] + interestAmount);
                }

            }

        }


        return m_InterestMap;
    }
}
