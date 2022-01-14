using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    public Vector3 direction;
    public float damage;

    public Attack(Vector3 direction, float damage)
    {
        this.direction = direction;
        this.damage = damage;
    }
}
