using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextChaseBehaviour : BaseContextBehavior
{

    const string CHASE_TARGETS_TAG = "ChaseTarget";

    [SerializeField] private float m_MaxChaseDistance;

    [SerializeField] private GameObject m_Target;
    public GameObject ChaseTarget { get { return m_Target;  } set { m_Target = value; } }

    private GameObject[] m_ChaseTargets;

    [SerializeField] private bool m_CenterBetweenTargets;

    private new void Start()
    {
        base.Start();
        m_ChaseTargets = GameObject.FindGameObjectsWithTag(CHASE_TARGETS_TAG);
    }

    public override List<float> GetDangerMap(Vector2 agentPosition, ref List<Vector2> directions)
    {
        return m_DangerMap;
    }

    public override List<float> GetInterestMap(Vector2 agentPosition, ref List<Vector2> directions)
    {
        m_InterestMap = new List<float>(new float[directions.Count]);
        foreach(GameObject target in m_ChaseTargets)
        {
            Vector2 targetPos = target.transform.position;
            Vector2 toTarget = targetPos - agentPosition;

            if (toTarget.magnitude > m_MaxChaseDistance)
                continue;

            for (int i = 0; i < directions.Count; i++)
            {
                float interestAmount = Vector2.Dot(toTarget, directions[i]) / toTarget.magnitude;
                Debug.Log(i);

                if (m_CenterBetweenTargets)
                {
                    m_InterestMap[i] = m_InterestMap[i] + interestAmount;

                }
                else
                {
                    if(interestAmount > m_InterestMap[i])
                    {
                        m_InterestMap[i] = interestAmount;
                    }
                }

            }

        }

        return m_InterestMap;
    }

    public void FillInterestMap(Vector2 agentPosition, ref List<Vector2> directions, ref List<float> interestMap)
    {

        foreach (GameObject target in m_ChaseTargets)
        {
            Vector2 targetPos = target.transform.position;
            Vector2 toTarget = targetPos - agentPosition;

            if (toTarget.magnitude > m_MaxChaseDistance)
                continue;

            for (int i = 0; i < directions.Count; i++)
            {
                float interestAmount = Vector2.Dot(toTarget, directions[i]) / toTarget.magnitude;
                Debug.Log(i);

                if (m_CenterBetweenTargets)
                {
                    interestMap[i] = interestMap[i] + interestAmount;

                }
                else
                {
                    if (interestAmount > interestMap[i])
                    {
                        interestMap[i] = interestAmount;
                    }
                }

            }

        }
    }
}
