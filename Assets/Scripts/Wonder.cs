using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wonder : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] float radius;
    [SerializeField] float waitTime;
    [SerializeField] private Animator animator;
    [SerializeField] ShukiController controller;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DoWonder());
    }

    IEnumerator DoWonder()
    {
        while (true)
        {
            Vector3 random = Random.insideUnitCircle * radius;
            random.z = random.y;
            random.y = 0;
            agent.SetDestination(transform.position + random);
            yield return new WaitUntil(()=> agent.remainingDistance <= agent.stoppingDistance);
            yield return new WaitForSeconds(waitTime);
        }
    } 
}
