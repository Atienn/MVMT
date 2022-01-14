using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasterAI : CasterController
{
    [SerializeField] GameObject target;
    [SerializeField] TriggerArea[] attkTriggers;
    Vector3 dirInput;

    sbyte jumpTimer;
    float angle;

    public override Vector3 GetMvmt()
    {
        return Vector3.zero;
        //angle = Mathf.PerlinNoise(Time.unscaledTime, Time.unscaledTime * -1) * 2 * Mathf.PI;
        //dirInput = new Vector3(
        //    Mathf.Cos(angle) + Mathf.Pow(-transform.position.x / 100, 3),
        //    0f,
        //    Mathf.Sin(angle) + Mathf.Pow(-transform.position.z / 100, 3));

        //return dirInput.normalized;
    }

    public override bool GetMvmtSpecial()
    {
        return false;
        //if (jumpTimer == 0)
        //{
        //    //1 in 600 chance to use dash (tried once per frame).
        //    if (0 == Random.Range(0, 600))
        //    {
        //        jumpTimer = 45;
        //        return true;
        //    }
        //    return false;
        //}

        //else jumpTimer--;
        //return false;
    }

    public override AoEAttack GetAttack()
    {
        for(byte i = 0; i < attkTriggers.Length; i++)
        {
            foreach(Collider trigger in attkTriggers[i].triggers)
            {
                if (trigger.bounds.Contains(target.transform.position)) { return self.attks[i]; }
            }
        }

        //for (byte i = 0; i < self.attks.Length; i++)
        //{
        //    if (self.attks[i].isInArea(target.transform.position)) { return self.attks[i]; }
        //}
        return null;
    }
}

//Really dumb. Required to be able to view and assign values in inspector.
[System.Serializable] 
public class TriggerArea 
{
    public Collider[] triggers;
}