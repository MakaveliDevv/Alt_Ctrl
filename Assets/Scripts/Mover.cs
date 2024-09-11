using UnityEngine;

public class Mover : MonoBehaviour
{
    private IMovementInputGetter movementInputGetter;
    // private CharacterStats stats;
    private float speed = 5f;

    private void Awake()
    {
        movementInputGetter = GetComponent<IMovementInputGetter>();
        // stats = GetComponent<CharacterStats>();
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
