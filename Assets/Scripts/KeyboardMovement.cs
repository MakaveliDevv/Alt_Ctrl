using System;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class KeyboardMovement : MonoBehaviour, IMovementInputGetter
{
    private bool isJumping;
    public float Horizontal { get; set; }
    public float Vertical { get; set; }

    public void Update() 
    {
        Horizontal = Input.GetAxisRaw("Horizontal");

        if(!isJumping) Vertical = 0f;
    }

    public void SetJumping(bool jumping)
    {
        isJumping = jumping;
        if (jumping) Vertical = 1f;
        else Vertical = 0f;
    }
}
