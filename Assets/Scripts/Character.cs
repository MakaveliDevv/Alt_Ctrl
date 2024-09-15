using System.Collections;
using UnityEngine;

public class Character : CharacterStats
{ 
    [SerializeField] private KeyboardMovement keyboardMovement;
    [SerializeField] private Rigidbody2D rb;

    private IMovementInputGetter movementInputGetter;
    
    void Awake() 
    {
        if (keyboardMovement == null) keyboardMovement = GetComponent<KeyboardMovement>();

        if(TryGetComponent<IMovementInputGetter>(out var _movement)) movementInputGetter = _movement;
        else movementInputGetter = null;
         
        rb = GetComponent<Rigidbody2D>();
    }

    void Start() 
    {
        currentHealth.SetValue(maxHealth.GetValue());
    }
    
    void Update() 
    {
        if(movementInputGetter != null) Move();

        if(Input.GetKeyDown(KeyCode.Space)) StartCoroutine(Jump());
    }

    protected void Move() 
    {
        Vector2 movement = new()
        {
            x = movementInputGetter.Horizontal,
            y = movementInputGetter.Vertical
        };

        movement *= movementSpeed.GetValue() * Time.deltaTime;
        transform.Translate(movement);

    }
    
    // public IEnumerator Jump() 
    // {
    //     float elapsedTime = 0f;
    //     float jumpDuration = 1f;

    //     while(elapsedTime < jumpDuration) 
    //     {
    //         Vector2 jump = new() 
    //         {
                
    //         };
    //         keyboardMovement.Vertical = 1f;
    //         elapsedTime += Time.deltaTime;
    //     }

    //     if(elapsedTime >= 1f) keyboardMovement.Vertical = 0f;

    //     yield break;
    // }

    public IEnumerator Jump() 
    {
        float elapsedTime = 0f;
        float jumpDuration = 1f;

        // Start jump
        if(keyboardMovement != null) keyboardMovement.SetJumping(true);

        while (elapsedTime < jumpDuration) 
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // End jump
        if(keyboardMovement != null) keyboardMovement.SetJumping(false);

        yield break;
    }
}
