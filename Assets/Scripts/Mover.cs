using UnityEngine;

public class Mover : MonoBehaviour
{
    private IMovementInputGetter movementInputGetter;
    public float speed = 5f;

    private void Awake()
    {
        movementInputGetter = GetComponent<IMovementInputGetter>();
    }

    void Update() 
    {
        Vector2 movement = new Vector2()
        {
            x = movementInputGetter.Horizontal,
            y = movementInputGetter.Vertical
        }.normalized;

        movement *= speed * Time.deltaTime;

        transform.Translate(movement);
    }
}
