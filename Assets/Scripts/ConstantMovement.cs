using UnityEngine;

[RequireComponent(typeof(Mover))]
public class ConstantMovement : MonoBehaviour, IMovementInputGetter
{
    public float Horizontal => 1f;

    public float Vertical => 0f;
}
