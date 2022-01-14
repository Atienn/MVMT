using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RunnerTPSPlayer : RunnerController
{
    [SerializeField] Transform cam;
    Vector3 dirInput;

    public override Vector3 GetMvmt()
    {
        dirInput =
            cam.forward * Input.GetAxisRaw("Vertical") +
            cam.right * Input.GetAxisRaw("Horizontal");

        //Project the directioal input vector onto horizontal plane.
        dirInput.y = 0f;
        //When normalized, this vector represents the desired direction to move in.
        return dirInput.normalized;
    }
    public override bool GetMvmtSpecial()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public override bool GetReset()
    {
        return Input.GetKeyDown(KeyCode.R);
    }
}