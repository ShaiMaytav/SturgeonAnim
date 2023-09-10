using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShukiController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float walkAnimSpeedMul;
    [SerializeField] private float turnAnimSpeedMul;
    [SerializeField] private Animator animator;

    private Vector3 lastPos;
    private float lastRotY;

    private void Start()
    {
        lastPos = transform.position;
        lastRotY = transform.rotation.eulerAngles.y;
    }

    private void Update()
    {
        Movement();
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

    //private IEnumerator WalkingSpeedSync()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForFixedUpdate();
    //        float speed = Vector3.Distance(lastPos, transform.position) * walkAnimSpeedMul;
    //        float angle = Mathf.Abs(Vector3.Angle((transform.position - lastPos).normalized , transform.forward));
    //        speed = (angle >= 179 && angle <= 181) ? speed * -1 : speed;
    //        animator.SetFloat("MoveSpeed", speed);
    //        animator.SetFloat("Forward", Mathf.Clamp(speed, -1, 1));
    //        lastPos = transform.position;
    //    }
    //}
    private IEnumerator WalkingSpeedSync()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            float speed = Vector3.Distance(lastPos, transform.position) * walkAnimSpeedMul;
            float rot = (transform.rotation.eulerAngles.y - lastRotY) * turnAnimSpeedMul;
            //float angle = Mathf.Abs(Vector3.Angle((transform.position - lastPos).normalized , transform.forward));
            //speed = (angle >= 179 && angle <= 181) ? speed * -1 : speed;
            animator.SetFloat("MoveSpeed", speed);
            animator.SetFloat("StrafeSpeed", rot / 2);
            //animator.SetFloat("Forward", Mathf.Clamp(speed, -1, 1));
            lastPos = transform.position;
            lastRotY = transform.rotation.eulerAngles.y;
        }
    }

    private void Movement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 newPos = transform.position += transform.forward * speed * Time.deltaTime;
            transform.position = newPos;
        }

        Vector3 newRot = transform.rotation.eulerAngles;

        if (Input.GetKey(KeyCode.A))
        {
            newRot.y -= turnSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            newRot.y += turnSpeed * Time.deltaTime;
        }

        transform.rotation = Quaternion.Euler(newRot);
    }
}

