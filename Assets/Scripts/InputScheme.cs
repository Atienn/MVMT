using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScheme
{
    [SerializeField]
    public enum Binds
    {
        Forwards = KeyCode.W,
        Backwards = KeyCode.S,
        Left = KeyCode.A,
        Right = KeyCode.B,
        Dash = KeyCode.Space,
    };
}
