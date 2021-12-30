using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : RunnerController
{
    byte dashTimer;
    float angle; 
    Vector3 dir;

    public override Vector3 GetMvmt()
    {
        angle = Mathf.PerlinNoise(Time.unscaledTime, Time.unscaledTime * -1) * 2 * Mathf.PI;
        dir = new Vector3(
            Mathf.Cos(angle) + Mathf.Pow(-transform.position.x / 100, 3), 
            0f, 
            Mathf.Sin(angle) + Mathf.Pow(-transform.position.z / 100, 3));
        return dir.normalized;
    }

    public override bool GetDash()
    {
        if(dashTimer == 0)
        {
            if (0 == Random.Range(0, 600))
            {
                Debug.Log("dash");
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
