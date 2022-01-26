using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextAvoidBehavior : BaseContextBehavior
{

    [SerializeField] private float m_MaxAvoidDistance;

    private GameObject[] m_AvoidTargets;
    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();

        m_AvoidTargets = GameObject.FindGameObjectsWithTag("AvoidTarget");

    }
    public override List<float> GetDangerMap(Vector2 agentPosition, ref List<Vector2> directions)
    {

        m_DangerMap = new List<float>(new float[directions.Count]);

        foreach (GameObject avoidTarget in m_AvoidTargets)
        {
            Vector2 targetPos = avoidTarget.transform.position;
            Vector2 toTarget = targetPos - agentPosition;

            if (toTarget.magnitude > m_MaxAvoidDistance)
                continue;

            for (int i = 0; i < directions.Count; i++)
            {
                float dangerAmount = Vector2.Dot(toTarget, directions[i]) / toTarget.magnitude;
                Debug.Log(i);


                if (dangerAmount > m_DangerMap[i])
                {
                    m_DangerMap[i] = dangerAmount;
                }
            }
        }

        return m_DangerMap;
    }

    public override List<float> GetInterestMap(Vector2 agentPostion, ref List<Vector2> directions)
    {
        return m_InterestMap;
    }
}
