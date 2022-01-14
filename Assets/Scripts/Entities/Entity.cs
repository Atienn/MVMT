using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Entity : MonoBehaviour
{
    public abstract void OnHit(Attack attk);
}

public abstract class CasterEntity : Entity
{
    public abstract void AttackStart();
    public abstract void AttackEnd();
}