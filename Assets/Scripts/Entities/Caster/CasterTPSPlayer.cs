using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasterTPSPlayer : CasterController
{
    [SerializeField] Transform cam;
    [SerializeField] AoEAttack defaultAttk;
    Vector3 dirInput;

    public override Vector3 GetMvmt()
    {
        dirInput =
            cam.forward * Input.GetAxisRaw("Vertical") +
            cam.right * Input.GetAxisRaw("Horizontal");

        //Project the directioal input vector onto horizontal plane.
        dirInput.y = 0f;
        //When normalized, this vector represents the desired direction to move in.
        dirInput.Normalize();

        return dirInput;
    }

    public override bool GetMvmtSpecial()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public override AoEAttack GetAttack()
    {
        if (Input.GetKey(KeyCode.Mouse0))       { return ResolveAttack(0); }
        else if (Input.GetKey(KeyCode.Mouse1))  { return ResolveAttack(1); }
        else if (Input.GetKey(KeyCode.E))       { return ResolveAttack(2); }
        else if (Input.GetKey(KeyCode.Q))       { return ResolveAttack(3); }
        else if (Input.GetKey(KeyCode.Z))       { return ResolveAttack(4); }
        else if (Input.GetKey(KeyCode.C))       { return ResolveAttack(5); }
        else { return null; }
    }

    private AoEAttack ResolveAttack(byte i)
    {
        if (i < self.attks.Length)
            return self.attks[i];
        else
            return self.defaultAttk;
    }

    private AoEAttack ResoelveAttack(byte i)
    {
        if (i < self.attks.Length)
            return self.attks[i];
        else
            return defaultAttk;
    }
}
