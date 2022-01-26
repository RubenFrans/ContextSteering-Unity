using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextSteeringBehavior : MonoBehaviour
{
    [SerializeField] private float m_Max;
    [SerializeField] private float m_IntersetFallOff;
    [SerializeField] private int m_MapResolution;
    [SerializeField] private float m_MovementSpeed;

    [SerializeField] private ContextChaseBehaviour[] m_Behaviors;

    private List<Vector2> m_Directions;
    private List<float> m_InterestMap;
    private List<float> m_DangerMap;

    ContextChaseBehaviour m_ContextChaseBehavior;
    ContextAvoidBehavior m_ContextAvoidBehavior;
    Rigidbody2D m_Rigibody2D;

    [SerializeField] private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        m_Directions = new List<Vector2>();
        
        m_ContextChaseBehavior = GetComponent<ContextChaseBehaviour>();
        m_ContextAvoidBehavior = GetComponent<ContextAvoidBehavior>();

        m_ContextChaseBehavior.InitializeContextMaps(m_MapResolution);
        m_ContextAvoidBehavior.InitializeContextMaps(m_MapResolution);
        
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

        m_InterestMap = new List<float>();
        m_DangerMap = new List<float>();

        for (int i = 0; i < m_Directions.Count; i++)
        {
            m_InterestMap.Add(0);
            m_DangerMap.Add(0);
        }

    }

    // Update is called once per frame
    void Update()
    {
        List<float> intersetMap = m_ContextChaseBehavior.GetInterestMap(gameObject.transform.position, ref m_Directions);
        List<float> dangerMap = m_ContextAvoidBehavior.GetDangerMap(gameObject.transform.position, ref m_Directions);

        for (int i = 0; i < dangerMap.Count; i++)
        {
            intersetMap[i] -= dangerMap[i];
        }


        float biggestInterest = Mathf.Max(intersetMap.ToArray());
        Debug.Log(biggestInterest);
        int indexOfBigges = intersetMap.FindIndex(x => (x == biggestInterest));

        for (int i = 0; i < m_Directions.Count; i++)
        {
            Vector2 agentPosition = gameObject.transform.position;
            Vector2 moveDirection = m_Directions[i] * intersetMap[i];
            Debug.DrawLine(gameObject.transform.position, agentPosition + moveDirection);
        }

        m_Rigibody2D.AddForce(m_Directions[indexOfBigges] * m_MovementSpeed * Time.deltaTime);

    }

    private void RenderLines()
    {
        List<float> intersetMap = m_ContextChaseBehavior.GetInterestMap(gameObject.transform.position, ref m_Directions);

        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();

        GL.Begin(GL.LINES);
        GL.Color(Color.red);



        for (int i = 0; i < m_Directions.Count; i++)
        {
            Vector2 agentPosition = gameObject.transform.position;
            Vector2 moveDirection = m_Directions[i] * intersetMap[i];
            GL.Vertex(gameObject.transform.position);
            GL.Vertex(agentPosition + moveDirection);
        }
        GL.PopMatrix();
        GL.End();
    }
}
