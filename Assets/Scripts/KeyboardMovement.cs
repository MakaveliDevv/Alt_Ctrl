using UnityEngine;

[RequireComponent(typeof(Player))]
public class KeyboardMovement : MonoBehaviour, IMovementInputGetter
{
    public float Horizontal { get; set; }
    public float Vertical { get; set; }

    public void Update() 
    {
        Horizontal = Input.GetAxisRaw("Horizontal");
        Vertical = 0f;
    }
}
