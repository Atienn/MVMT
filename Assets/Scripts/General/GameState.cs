using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class GameState : MonoBehaviour
{
    public static Entity[] entities;
    void Start()
    {
        entities = FindObjectsOfType<Entity>(true);
    }
}
