using UnityEngine;
using UnityEngine.AI;

public class SimpleAgent : MonoBehaviour
{
    [SerializeField] Transform target;

    NavMeshAgent agent;

    void Awake()
    {
       agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target != null)
            agent.SetDestination(target.position);
    }
}
