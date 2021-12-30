using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Linked Components")]
    [SerializeField] CharacterController charCtrl;
    [SerializeField] Transform camPos;

    [Header("General")]
    [SerializeField] float speed = 4;
    [SerializeField] float gravity = -3;

    [Header("Dash")]
    [SerializeField] float dashForce = 40;
    [SerializeField] float jumpForce = 40;
    [Tooltip("Counted in frames")]
    [SerializeField] byte dashCooldown = 30;


    bool isGrounded;
    bool isDashing;
    byte dashTimer;
    
    Vector3 velocity;
    Vector3 dirInput;
    Vector3 targetRotation;

    const float turnSmoothTime = 0.075f;
    float turnSmoothVel;


    void Start()
    {
        dashTimer = dashCooldown;
        isDashing = true;
    }


    void Update()
    {
        if (isGrounded)
        {
            if(!isDashing)
            {
                dirInput =
                camPos.forward * Input.GetAxisRaw("Vertical") +
                camPos.right * Input.GetAxisRaw("Horizontal");

                //Project the directioal input vector onto horizontal plane.
                dirInput.y = 0f;
                //When normalized, this vector represents the desired direction to move in.
                dirInput.Normalize();

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //isGrounded = false;
                    //velocity.y += jumpForce;
                    isDashing = true;
                    dashTimer = dashCooldown;

                    if (dirInput == Vector3.zero)
                    {
                        dirInput = Vector3.zero;
                        this.velocity = targetRotation * -40 * speed;
                    }
                    else
                    {
                        this.velocity = dirInput * dashForce * speed;
                        dirInput = Vector3.zero;
                    }
                }
            }
            else if (dashTimer > dashCooldown/2 && Input.GetKeyDown(KeyCode.Space))
            {
                this.velocity = -targetRotation * dashForce + Vector3.up * jumpForce; 
                Debug.Log("Dash Jump");
            }
        }
    }

    void FixedUpdate()
    {
        //Compute friction.
        velocity.x = Mathf.MoveTowards(velocity.x, 0f, 0.075f * Mathf.Abs(velocity.x) + 0.25f);
        velocity.z = Mathf.MoveTowards(velocity.z, 0f, 0.075f * Mathf.Abs(velocity.z) + 0.25f);

        //Check if grounded for this frame.
        isGrounded = charCtrl.isGrounded;

        //Compute dash cooldown if any.
        if (dashTimer > 0)
            if(--dashTimer == 0) isDashing = false;

        if(dirInput != Vector3.zero)
        {
            targetRotation = dirInput;
            //Wouldn't matter if it was executed each frame, but this saves an operation.
            velocity += dirInput * speed;
        }

        //All of this is just for the character rotation to be smooth. It is purely for asthetics.
        transform.rotation =
            Quaternion.Euler(
                0f,
                Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    Mathf.Atan2(targetRotation.x, targetRotation.z) * Mathf.Rad2Deg,
                    ref turnSmoothVel,
                    turnSmoothTime),
                0f);

        //If not grounded, add gravity.
        if (!isGrounded)
            velocity.y += gravity;
        //If grounded with negative velocity, lower it to a neglegible amount. 
        //Cannot be 0 as the character controller's 'isGrounded' stops working properly otherwise.
        else if(velocity.y < -0.1f)
            velocity.y = -0.1f;

        //Move the character. The velocity will always be non-zero.
        charCtrl.Move(velocity * Time.fixedDeltaTime);
    }
}
