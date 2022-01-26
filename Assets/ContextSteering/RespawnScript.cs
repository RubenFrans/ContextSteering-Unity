using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnScript : MonoBehaviour
{

    private const string RESPAWN_POINT_TAG = "Respawn";
    private const string CHASE_TARGET_TAG = "ChaseTarget";
    private GameObject m_RespawnPoint;

    void Start()
    {
        m_RespawnPoint = GameObject.FindGameObjectWithTag(RESPAWN_POINT_TAG);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals(CHASE_TARGET_TAG))
        {
            gameObject.transform.position = m_RespawnPoint.transform.position;
        }
    }
}
