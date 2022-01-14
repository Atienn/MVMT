using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerAI : RunnerController
{
    byte dashTimer;
    float angle; 
    Vector3 dirInput;

    public override Vector3 GetMvmt()
    {
        angle = Mathf.PerlinNoise(Time.unscaledTime, Time.unscaledTime * -1) * 2 * Mathf.PI;
        dirInput = new Vector3(
            Mathf.Cos(angle) + Mathf.Pow(-transform.position.x / 100, 3), 
            0f, 
            Mathf.Sin(angle) + Mathf.Pow(-transform.position.z / 100, 3));

        return dirInput.normalized;
    }

    public override bool GetMvmtSpecial()
    {
        if(dashTimer == 0)
        {
            //1 in 600 chance to use dash (tried once per frame).
            if (0 == Random.Range(0, 600))
            {
                dashTimer = 7;
                return true;
            }
            return false;
        }
        
        else dashTimer--;
        return false;
    }

    public override bool GetReset()
    {
        return transform.position.y < -25f;
    }
}
