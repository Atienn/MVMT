using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Misc;

[RequireComponent(typeof(RunnerController))]
[RequireComponent(typeof(CharacterController))]
public class Runner : Entity
{
    #region Movement

    [SerializeField] RunnerController input;
    CharacterController charCtrl;

    [Header("General")]
    [SerializeField] float speed = 4;
    [SerializeField] float gravity = -3;

    [Header("Dash")]
    [SerializeField] float dashSpeed = 85;
   // [SerializeField] float jumpSpeed = 50;
    [SerializeField] float jumpForce = 55;
    [Header("Length")]
    [SerializeField] byte dashActive = 15;
    [SerializeField] byte dashRecovery = 21;
    // [SerializeField] byte dashJump = 30;

    //Enables strategy design pattern.
    VoidStrategy mvmtMode;
    VoidStrategy dashMode;

    bool dashInput;
    byte timer;

    Vector3 dirInput;
    Vector3 velocity;
    Vector3 targetRotation;
    Vector3 spawnLocation;

    const float turnSmoothTime = 0.075f;
    float turnSmoothVel;


    void Start()
    {
        //input = GetComponent<RunnerController>();
        charCtrl = GetComponent<CharacterController>();

        mvmtMode = GroundMvmt;
        dashMode = GroundDash;
        timer = dashRecovery;

        spawnLocation = transform.position;
    }

    void Update()
    {
        dirInput = input.GetMvmt();
        //Special is dash for Runner.
        if (input.GetMvmtSpecial()) { dashMode(); }
        if (input.GetReset()) { charCtrl.Move(spawnLocation - transform.position); }
    }

    void FixedUpdate() { mvmtMode(); }

    void SwitchMode(VoidStrategy mvmt, VoidStrategy dash)
    {
        mvmtMode = mvmt;
        dashMode = dash;
    }

    #region Movement Modes

    void GroundMvmt()
    {
        if (!charCtrl.isGrounded)
        {
            //Debug.Log("Ground -> Air");
            SwitchMode(AirMvmt, NoDash);
            AirMvmt(); return;
        }

        //Compute friction.
        velocity.x = Mathf.MoveTowards(velocity.x, 0f, 0.075f * Mathf.Abs(velocity.x) + 0.25f);
        velocity.z = Mathf.MoveTowards(velocity.z, 0f, 0.075f * Mathf.Abs(velocity.z) + 0.25f);

        //Check if grounded for this frame.
        //isGrounded = charCtrl.isGrounded;

        if (dirInput != Vector3.zero)
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

        //Move the character. The velocity will always be non-zero.
        charCtrl.Move(velocity * Time.fixedDeltaTime);
    }

    void AirMvmt()
    {
        if (charCtrl.isGrounded && velocity.y < 0)
        {
            //Lower vertical velocity to a neglegible amount. 
            //Cannot be 0 as the character controller's 'isGrounded' stops working properly otherwise.
            velocity.y = -0.1f;

            //Debug.Log("Air -> Ground");
            //If now grounded, switch to grounded mode.
            SwitchMode(GroundMvmt, GroundDash);
            GroundMvmt(); return;
        }

        //If not grounded, add gravity.
        velocity.y += gravity;

        //Move the character.
        charCtrl.Move(velocity * Time.fixedDeltaTime);
    }

    void DashMvmt()
    {
        //Compute dash cooldown if any.
        if (--timer == 0)
        {
            //Debug.Log("Dash -> Recovery");
            timer = dashRecovery;
            SwitchMode(DashRecoveryMvmt, NoDash);
            DashRecoveryMvmt(); return;
        }
        //
        else if (!charCtrl.isGrounded)
        {
            //Debug.Log("Dash -> F-Jump");
            //Jumping forwards
            velocity = velocity.normalized * jumpForce;
            velocity.y = jumpForce;
            SwitchMode(AirMvmt, NoDash);
            AirMvmt(); return;
        }
        //Move fast
        charCtrl.Move(velocity * Time.fixedDeltaTime);
    }

    void DashRecoveryMvmt()
    {
        if (!charCtrl.isGrounded)
        {
            //Debug.Log("Recovery -> Air");
            timer = 0;
            SwitchMode(AirMvmt, NoDash);
            AirMvmt(); return;
        }
        //Compute dash cooldown if any.
        else if (--timer == 0)
        {
            //Debug.Log("Recovery -> Ground");
            SwitchMode(GroundMvmt, GroundDash);
            GroundMvmt(); return;
        }

        //Compute friction.
        velocity.x = Mathf.MoveTowards(velocity.x, 0f, 0.08f * Mathf.Abs(velocity.x) + 0.1f);
        velocity.z = Mathf.MoveTowards(velocity.z, 0f, 0.08f * Mathf.Abs(velocity.z) + 0.1f);

        charCtrl.Move(velocity * Time.fixedDeltaTime);
    }

    void JumpMvmt()
    {
        charCtrl.Move(velocity * Time.fixedDeltaTime);

        if (charCtrl.isGrounded)
        {
            //Debug.Log("Jump -> Ground");

            //Lower vertical velocity to a neglegible amount. 
            //Cannot be 0 as the character controller's 'isGrounded' stops working properly otherwise.
            //velocity.y = -0.1f;
            SwitchMode(GroundMvmt, GroundDash);
            GroundMvmt(); return;
        }
        //Compute dash cooldown if any.
        else if (--timer == 0)
        {
            //Debug.Log("Jump -> Air");
            SwitchMode(AirMvmt, NoDash);
            AirMvmt(); return;
        }
    }

    #endregion

    #region Dash Modes

    void NoDash() {
        //Debug.Log("No Dash");
    }

    void GroundDash() 
    {
        //Debug.Log("Ground -> Dash");

        timer = dashActive;
        if(dirInput == Vector3.zero)
            velocity = targetRotation * dashSpeed;
        else
        {
            velocity = dirInput * dashSpeed;
        }
        //Keep the runner grounded.
        velocity.y = -0.1f;
        SwitchMode(DashMvmt, JumpDash);
    }

    void JumpDash() 
    {
        //Debug.Log("Dash -> B-Jump");
        velocity = -dirInput * jumpForce *.8f;
        velocity.y = jumpForce;
        SwitchMode(GroundMvmt, NoDash);
    }

    #endregion

    //abstract class Mode
    //{
    //    Strategy mvmt;
    //    Strategy dash;
    //}
    //class GroundMode : Mode { }
    //static class AirMode { }
    //static class DashMode { }
    //static class DashRecoveryMode { }


    //public interface IMovementMode
    //{
    //    void Move(RunnerMovement run);
    //    void Dash(RunnerMovement run);
    //}

    //public class Grounded : IMovementMode
    //{
    //    void IMovementMode.Move(RunnerMovement run) {

    //    }

    //    void IMovementMode.Dash(RunnerMovement run)
    //    {

    //    }
    //}

    //public class Air : IMovementMode
    //{
    //    public void Move(RunnerMovement run)
    //    {
    //        if (run.charCtrl.isGrounded)
    //        {
    //            //Lower vertical velocity to a neglegible amount. 
    //            //Cannot be 0 as the character controller's 'isGrounded' stops working properly otherwise.
    //            run.velocity.y = -0.1f;

    //            //If now grounded, switch to grounded movement.
    //            run.mvmt = run.GroundMvmt;
    //            //
    //            run.GroundMvmt();
    //            return;
    //        }

    //        //If not grounded, add gravity.
    //        run.velocity.y += run.gravity;

    //        //Move the character.
    //        run.charCtrl.Move(run.velocity * Time.fixedDeltaTime);
    //    }

    //    public void Dash(RunnerMovement run) { }
    //}


    #endregion

    #region Combat

    public override void OnHit(Attack attk)
    {
        this.velocity += attk.direction * attk.damage;
        SwitchMode(AirMvmt, NoDash);
    }

    #endregion
}