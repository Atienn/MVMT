using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RunnerController : MonoBehaviour
{
    public abstract Vector3 GetMvmt();
    public abstract bool GetDash();

    public abstract bool GetReset();
}
