using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CasterController : MvmtController
{
    protected CasterEnemy self;
    public abstract AoEAttack GetAttack();

    void Start() { self = GetComponent<CasterEnemy>(); }
}