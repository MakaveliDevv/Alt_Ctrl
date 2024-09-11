using UnityEngine;

[RequireComponent(typeof(Mover))]
public class KeyboardMovement : MonoBehaviour, IMovementInputGetter
{
    public float Horizontal { get; set; }
    public float Vertical { get; set; }
    public void Update() 
    {
        Horizontal = Input.GetAxisRaw("Horizontal");
        Vertical = Input.GetAxisRaw("Vertical");
    }
}
