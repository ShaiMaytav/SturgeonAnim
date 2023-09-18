using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShukiController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float turnSens;
    [SerializeField] private float speedSens;
    [SerializeField] private float walkAnimSpeedMul;
    [SerializeField] private float turnAnimSpeedMul;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] float angle;
    [SerializeField] float lookAngleRange;
    [SerializeField] float lookSpeed;
    [SerializeField] GameObject neck;
    [SerializeField] GameObject target;


    private Vector3 lastPos;
    private float lastRotY;

    private void Start()
    {
        lastPos = transform.position;
        lastRotY = transform.rotation.eulerAngles.y;
        Walk();
    }

    private void LateUpdate()
    {
        LookAtTarget();
    }

    private void LookAtTarget()
    {
        Vector3 targetDir = (target.transform.position - transform.position).normalized;

        if (Vector3.Angle(transform.forward, targetDir) <= lookAngleRange)
        {
            neck.transform.forward = Vector3.Lerp(neck.transform.forward, targetDir, lookSpeed * Time.deltaTime);
        }
        else if (neck.transform.forward != transform.forward)
        {
            neck.transform.forward = Vector3.Lerp(neck.transform.forward, transform.forward, lookSpeed * Time.deltaTime);
        }
    }


    [ContextMenu("Walk")]
    public void Walk()
    {
        animator.SetBool("IsMoving", true);
        StartCoroutine(WalkingSpeedSync());
    }


    [ContextMenu("StopWalk")]
    public void StopWalk()
    {
        animator.SetBool("IsMoving", false);
        StopCoroutine(WalkingSpeedSync());
    }


    float currentRot;
    float currentSpeed;

    private IEnumerator WalkingSpeedSync()
    {
        currentRot = 0;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            Vector3 speedVec = new Vector3(navMeshAgent.desiredVelocity.x * transform.forward.x, navMeshAgent.desiredVelocity.y * transform.forward.y, navMeshAgent.desiredVelocity.z * transform.forward.z);
            float rot = (transform.rotation.eulerAngles.y - lastRotY) * turnAnimSpeedMul;
            rot = Mathf.Clamp(rot, -1, 1);
            float speed = Mathf.Clamp(speedVec.magnitude, -1, 1);
            currentRot = Mathf.Lerp(currentRot, rot, Time.deltaTime * turnSens);
            currentSpeed = Mathf.Lerp(currentSpeed, speed, Time.deltaTime * speedSens);
            animator.SetFloat("MoveSpeed", currentSpeed);
            animator.SetFloat("StrafeSpeed", currentRot);
            lastPos = transform.position;
            lastRotY = transform.rotation.eulerAngles.y;
        }
    }

    
}

