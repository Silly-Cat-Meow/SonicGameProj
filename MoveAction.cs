using UnityEngine;
using UnityEngine.InputSystem;

public class MoveAction : PlayerAction
{

    //Audio
    public AudioSource boostSound;
    // Movement
    Vector2 move;

    // Boost
    [SerializeField] float boostForce;
    bool isBoosting;

    // Sliding
    [SerializeField] float slideDuration = 1.0f; // Duration of the slide
    [SerializeField] float slideDeceleration = 5.0f; // Deceleration while sliding
    bool isSliding;
    float slideTimer;

    // Animator
    [SerializeField] Animator animator;

    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        move = callbackContext.ReadValue<Vector2>();
    }

    public void OnBoost(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            boostSound.Play();
            isBoosting = true;
        }
        else if (callbackContext.canceled)
        {
            isBoosting = false;
            boostSound.Stop();
        }
    }

    public void OnSlide(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started && !isSliding)
        {
            StartSlide();
        }
    }

    // On Enable
    void OnEnable()
    {
        PlayerPhysics.onPlayerPhysicsUpdate += Move;
    }

    // On Disable
    void OnDisable()
    {
        PlayerPhysics.onPlayerPhysicsUpdate -= Move;
    }

    // Movement
    [SerializeField] Transform cameraTransform;

    [SerializeField] float deceleration;
    [SerializeField] float acceleration;
    [SerializeField] float maxSpeed;
    [SerializeField] float minturnSpeed;
    [SerializeField] float maxturnSpeed;
    [SerializeField, Range(0f, 1f)] float turnDeceleration;
    [SerializeField] float brakeSpeed;
    [SerializeField] float brakeTime;
    [SerializeField, Range(0, 1)] float softBreakThreshold;
    [SerializeField] float brakeThreshold;
    [SerializeField] float animSpeed;
    [SerializeField] float RunSpeed;
    [SerializeField] float BoostSpeed;
    [SerializeField] float BoostCap;

    bool braking;
    float brakeTimer;

    void Move()
    {
        Vector3 moveVector = GetMoveVector(cameraTransform, groundInfo.normal, move);
        float currentSpeed = rb.velocity.magnitude;

        // Check for sliding
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                isSliding = false; // End sliding after duration
            }
            Decelerate(slideDeceleration); // Apply sliding deceleration
            animator.SetBool("IsSliding", true);
            return; // Skip further movement logic if sliding
        }
        else
        { 
            animator.SetBool("IsSliding", false);
        }

        if (currentSpeed == 0f)
        {
            isSliding = false;
        }

        float normalizedSpeed = currentSpeed / maxSpeed;
        animator.SetFloat("Speed", normalizedSpeed);

        // Debug log to check speed values
        Debug.Log($"Current Speed: {currentSpeed}, Normalized Speed: {normalizedSpeed}");

        bool wasBraking = braking;
        braking = groundInfo.ground;
        braking &= currentSpeed < rb.sleepThreshold;
        braking &= (braking && brakeTime > 0 && brakeTimer > 0)
            || Vector3.Dot(moveVector.normalized, PlayerPhysics.horizontalVelocity) < -brakeThreshold;

        if (braking)
            brakeTimer -= Time.deltaTime;

        if (braking && !wasBraking)
            brakeTimer = brakeTime;

        if (braking)
            Decelerate(brakeSpeed);
        else if (move.magnitude > 0)
        {
            if (Vector3.Dot(moveVector.normalized, PlayerPhysics.horizontalVelocity.normalized) >= (groundInfo.ground ? -softBreakThreshold : 0))
                Accelerate(acceleration, moveVector);
            else
                Decelerate(brakeSpeed);
        }
        else
            Decelerate(deceleration);

        if (isBoosting && currentSpeed < BoostCap)
        {
            Boost(moveVector);
            animator.SetBool("IsBoosting", true);
            animator.speed = BoostSpeed;

        }
        else if (currentSpeed < maxSpeed)
        {
            animator.SetBool("IsBoosting", false);
            animator.speed = animSpeed;
        }

        if (currentSpeed > 35 && !isBoosting)
            animator.speed = RunSpeed;

        if (braking)
        {
            animator.SetBool("IsBraking", true);
        }
        else
        {
            animator.SetBool("IsBraking", false);
        }
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration; // Reset slide timer
        // Optionally, you can add any additional effects like animation here
    }

    void Accelerate(float speed, Vector3 moveVector)
    {
        float maxRadData = Mathf.Lerp(minturnSpeed, maxturnSpeed, rb.velocity.magnitude / maxSpeed) * Mathf.PI * Time.deltaTime;
        float maxDistDelta = acceleration * Time.deltaTime;

        Vector3 velocity = Vector3.RotateTowards(PlayerPhysics.horizontalVelocity, moveVector * maxSpeed, maxRadData, maxDistDelta);
        velocity -= velocity * (Vector3.Angle(PlayerPhysics.horizontalVelocity, velocity) / 180 * turnDeceleration);

        rb.velocity = velocity + PlayerPhysics.verticalVelocity;
    }

    void Decelerate(float speed)
    {
        rb.velocity = Vector3.MoveTowards(PlayerPhysics.horizontalVelocity, Vector3.zero, speed * Time.deltaTime)
           + PlayerPhysics.verticalVelocity;
    }

    void Boost(Vector3 moveVector)
    {
        rb.AddForce(moveVector.normalized * boostForce, ForceMode.Acceleration);
    }

    Vector3 GetMoveVector(Transform relativeTo, Vector3 upNormal, Vector2 move)
    {
        Vector3 rightNormal = Vector3.Cross(upNormal, relativeTo.forward);
        Vector3 forwardNormal = Vector3.Cross(relativeTo.right, upNormal);

        Debug.DrawRay(rb.transform.position, rightNormal * 10, Color.red);
        Debug.DrawRay(rb.transform.position, forwardNormal * 10, Color.green);

        Vector3.OrthoNormalize(ref upNormal, ref forwardNormal, ref rightNormal);

        return (rightNormal * move.x) + (forwardNormal * move.y);
    }
}
