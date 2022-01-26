using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextSteeringBehavior : MonoBehaviour
{
    [SerializeField] private float m_Max;
    [SerializeField] private float m_IntersetFallOff;
    [SerializeField] private int m_MapResolution;
    [SerializeField] private float m_MovementSpeed;

    [SerializeField] private BaseContextBehavior[] m_Behaviors;

    private List<Vector2> m_Directions;
    private List<float> m_InterestMap;
    private List<float> m_DangerMap;

    //ContextChaseBehaviour m_ContextChaseBehavior;
    //ContextAvoidBehavior m_ContextAvoidBehavior;
    Rigidbody2D m_Rigibody2D;

    [SerializeField] private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        m_Directions = new List<Vector2>();

        m_Behaviors = GetComponents<BaseContextBehavior>();

        foreach (BaseContextBehavior behavior in m_Behaviors)
        {
            behavior.InitializeContextMaps(m_MapResolution);
        }

        //m_ContextChaseBehavior = GetComponent<ContextChaseBehaviour>();
        //m_ContextAvoidBehavior = GetComponent<ContextAvoidBehavior>();

        //m_ContextChaseBehavior.InitializeContextMaps(m_MapResolution);
        //m_ContextAvoidBehavior.InitializeContextMaps(m_MapResolution);
        
        m_Rigibody2D = GetComponent<Rigidbody2D>();

        InitializeDirections();
    }

    void InitializeDirections()
    {
        float twoPi = Mathf.PI * 2;
        float directionInterval = twoPi / m_MapResolution;
        for (int i = 0; i < m_MapResolution; i++)
        {
            float currentAngle = i * directionInterval;

            m_Directions.Add(new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)));

        }

        m_InterestMap = new List<float>(new float[m_Directions.Count]);
        m_DangerMap = new List<float>(new float[m_Directions.Count]);

    }

    // Update is called once per frame
    void Update()
    {
        m_InterestMap = new List<float>(new float[m_Directions.Count]);
        m_DangerMap = new List<float>(new float[m_Directions.Count]);

        List<List<float>> interestMaps = new List<List<float>>();
        List<List<float>> dangerMaps = new List<List<float>>();

        foreach(BaseContextBehavior behavior in m_Behaviors)
        {
            interestMaps.Add(behavior.GetInterestMap(gameObject.transform.position, ref m_Directions));
            dangerMaps.Add(behavior.GetDangerMap(gameObject.transform.position, ref m_Directions));
        }


        for (int i = 0; i < m_InterestMap.Count; i++)
        {
           // m_InterestMap[i] = Mathf.Max(interestMaps[i].ToArray());

            float biggestInterestForThisSlot = 0;
            for (int k = 0; k < interestMaps.Count; k++)
            {
                if (interestMaps[k][i] > biggestInterestForThisSlot)
                    biggestInterestForThisSlot = interestMaps[k][i];
            }
            
            m_InterestMap[i] = biggestInterestForThisSlot;
        }

        for (int i = 0; i < m_DangerMap.Count; i++)
        {
            float biggestInterestForThisSlot = 0;
            for (int k = 0; k < dangerMaps.Count; k++)
            {
                if (dangerMaps[k][i] > biggestInterestForThisSlot)
                    biggestInterestForThisSlot = dangerMaps[k][i];
            }

            m_DangerMap[i] = biggestInterestForThisSlot;
        }



        for (int i = 0; i < m_DangerMap.Count; i++)
        {
            m_InterestMap[i] -= m_DangerMap[i];
        }


        float biggestInterest = Mathf.Max(m_InterestMap.ToArray());
        Debug.Log(biggestInterest);
        int indexOfBigges = m_InterestMap.FindIndex(x => (x == biggestInterest));

        Vector2 agentPosition = gameObject.transform.position;
        for (int i = 0; i < m_Directions.Count; i++)
        {
            Vector2 moveDirection = m_Directions[i] * m_InterestMap[i];
            Debug.DrawLine(gameObject.transform.position, agentPosition + moveDirection);
        }

        m_Rigibody2D.AddForce(m_Directions[indexOfBigges] * m_MovementSpeed * Time.deltaTime);
        // transform.LookAt(new Vector2(m_Rigibody2D.velocity.x, m_Rigibody2D.velocity.y), new Vector2(m_Rigibody2D.velocity.x, m_Rigibody2D.velocity.y));


        Vector2 lookdirection = (m_Rigibody2D.velocity + agentPosition) - agentPosition;

        float angle = Mathf.Atan2(lookdirection.y, lookdirection.x) * Mathf.Rad2Deg - 90.0f;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

    //private void RenderLines()
    //{
    //    List<float> intersetMap = m_ContextChaseBehavior.GetInterestMap(gameObject.transform.position, ref m_Directions);

    //    GL.PushMatrix();
    //    mat.SetPass(0);
    //    GL.LoadOrtho();

    //    GL.Begin(GL.LINES);
    //    GL.Color(Color.red);



    //    for (int i = 0; i < m_Directions.Count; i++)
    //    {
    //        Vector2 agentPosition = gameObject.transform.position;
    //        Vector2 moveDirection = m_Directions[i] * intersetMap[i];
    //        GL.Vertex(gameObject.transform.position);
    //        GL.Vertex(agentPosition + moveDirection);
    //    }
    //    GL.PopMatrix();
    //    GL.End();
    //}
}
