using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Misc;

public class CasterEnemy : CasterEntity
{
    #region General
    [Header("General")]

    [SerializeField] CasterController input;
    [SerializeField] CharacterController charCtrl;
    
    bool canAct;

    #endregion

    #region  Combat
    [Header("Combat")]

    public AoEAttack[] attks;
    public AoEAttack defaultAttk;
    AoEAttack currentAttk;

    float hitPoints;

    #endregion

    #region Movement
    [Header("Movement")]

    [SerializeField] float speed = 3;
    [SerializeField] float gravity = -3;
    [SerializeField] float jumpForce = 40;

    Vector3 dirInput;
    Vector3 velocity;
    Vector3 targetRotation;

    const float turnSmoothTime = 0.05f;
    float turnSmoothVel;

    VoidStrategy mvmtMode;
    VoidStrategy jumpMode;

    #endregion

    void Start()
    {
        if (attks == null) { 
            attks = new AoEAttack[0];
        }
        mvmtMode = GroundMvmt;
        jumpMode = GroundJump;

        canAct = true;
    }

    void Update()
    {
        dirInput = input.GetMvmt();
        //Special is jump for caster.
        if (input.GetMvmtSpecial()) { jumpMode(); }

        if (canAct)
        {


            currentAttk = input.GetAttack();
            if(currentAttk != null) { StartCoroutine(currentAttk.Trigger()); }
        }
    }

    void FixedUpdate() { mvmtMode(); }

    #region Movement Modes

    void GroundMvmt()
    {
        if (!charCtrl.isGrounded)
        {
            SwitchMode(AirMvmt, NoJump);
            AirMvmt(); return;
        }

        //Compute friction.
        velocity.x = Mathf.MoveTowards(velocity.x, 0f, 0.075f * Mathf.Abs(velocity.x) + 0.25f);
        velocity.z = Mathf.MoveTowards(velocity.z, 0f, 0.075f * Mathf.Abs(velocity.z) + 0.25f);

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
        if (charCtrl.isGrounded)
        {
            //Lower vertical velocity to a neglegible amount. 
            //Cannot be 0 as the character controller's 'isGrounded' stops working properly otherwise.
            velocity.y = -0.1f;

            //If now grounded, switch to grounded mode.
            SwitchMode(GroundMvmt, GroundJump);
            GroundMvmt(); return;
        }

        //If not grounded, add gravity.
        velocity.y += gravity;

        //Move the character.
        charCtrl.Move(velocity * Time.fixedDeltaTime);
    }


    #endregion

    #region Jump Modes

    void NoJump()
    {
        Debug.Log("No Dash");
    }

    void GroundJump()
    {
        velocity.y = jumpForce;
        SwitchMode(GroundMvmt, NoJump);
    }

    #endregion

    void SwitchMode(VoidStrategy mvmt, VoidStrategy jump)
    {
        mvmtMode = mvmt;
        jumpMode = jump;
    }

    public override void OnHit(Attack attk) { hitPoints -= attk.damage; }
    public override void AttackStart() { canAct = false; }
    public override void AttackEnd() { canAct = true; }
}