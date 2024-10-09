using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    public Rigidbody rb;

    public Vector3 verticalVelocity => Vector3.Project(rb.velocity, rb.transform.up);

    public float verticalSpeed => Vector3.Dot(rb.velocity, rb.transform.up);

    public Vector3 horizontalVelocity => Vector3.ProjectOnPlane(rb.velocity, rb.transform.up);

    public LayerMask layerMask;

    [SerializeField] float gravity;

    public float speed => horizontalVelocity.magnitude;

    public Transform modelTransform;



    //fixedUpdate

    public Action onPlayerPhysicsUpdate;

    void FixedUpdate()
{

        onPlayerPhysicsUpdate?.Invoke();

        if (!groundInfo.ground)
        Gravity();

        if (groundInfo.ground && verticalSpeed < rb.sleepThreshold)
            rb.velocity = horizontalVelocity;

        StartCoroutine(LateFixedUpdateRoutine());

        IEnumerator LateFixedUpdateRoutine()
        {
            yield return new WaitForFixedUpdate();

            LateFixedUpdate();
        }
    }

    //Gravity
    void Gravity()
    {
        rb.velocity -= Vector3.up * gravity * Time.deltaTime;

    }

    //late fixed update
    void LateFixedUpdate()
    {
        Ground();
        Snap();

        if (groundInfo.ground)
        {
            rb.velocity = horizontalVelocity;
            RotateToMovementDirection(); // Add this line to rotate the character
        }
    }

    void RotateToMovementDirection()
    {
        if (speed > 0) // Check if there is horizontal movement
        {
            // Calculate the target rotation based on horizontal velocity
            Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity.normalized, groundInfo.normal);

            // Smoothly rotate the model towards the target rotation
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, Time.deltaTime * 15f);
        }
    }


    //ground

    [SerializeField] float groundDistance;

    public struct GroundInfo
    {
        public Vector3 point;

            public Vector3 normal;

        public bool ground;
    }

[HideInInspector] public GroundInfo groundInfo;

    public Action onGroundEnter;

    public Action onGroundExit;
    void Ground()
    {
        float maxDistance = Mathf.Max(rb.centerOfMass.y, 0) + (rb.sleepThreshold * Time.deltaTime);

        if (groundInfo.ground && verticalSpeed < rb.sleepThreshold)
            maxDistance += groundDistance;

      bool ground = Physics.Raycast(rb.worldCenterOfMass, -rb.transform.up, out RaycastHit hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore);
    
      Vector3 point = ground ? hit.point : rb.transform.position;

        Vector3 normal = ground ? hit.normal : Vector3.up;

        if (ground != groundInfo.ground)
        {
            if (ground)
                onGroundEnter?.Invoke();
            else
                onGroundExit?.Invoke();
        }

        groundInfo = new()
        {
            point = point,
            normal = normal,
            ground = ground
        };
    }

    //snap
    void Snap()
    {
        rb.transform.up = groundInfo.normal;

        Vector3 goal = groundInfo.point;

        Vector3 difference = goal - rb.transform.position;

        if (rb.SweepTest(difference, out _, difference.magnitude, QueryTriggerInteraction.Ignore)) return;

        rb.transform.position = goal;
    }

    //Ring sound
    public AudioSource RingSound;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ring"))
        {
            RingSound.Play();
        }
    }
    }
