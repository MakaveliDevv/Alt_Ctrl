using System;
using UnityEngine;

public class Character : CharacterStats
{ 
    private IMovementInputGetter movementInputGetter;
    private KeyboardMovement keyboardMovement;
    private CharacterCombat char_combat;

    void Awake() 
    {
        if(TryGetComponent<IMovementInputGetter>(out var _movement))
            movementInputGetter = _movement;

        else
            movementInputGetter = null;


        if(TryGetComponent<CharacterCombat>(out var _combat))   
            char_combat = _combat;
        
        else 
            char_combat = null;
         

        keyboardMovement = GetComponent<KeyboardMovement>();
    }

    void Start() 
    {
        currentHealth.SetValue(maxHealth.GetValue());
    }
    
    void Update() 
    {
        if(movementInputGetter != null)
            Move();

        if(char_combat != null) 
        {
            char_combat.Attack();
            char_combat.SuperAttack();
        }
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
    
    public void Jump() 
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            float elapsedTime = 0f;
            float jumpDuration = 1f;
            
            while(elapsedTime < jumpDuration) 
            {
                keyboardMovement.Vertical = 1f;
                elapsedTime += Time.deltaTime;
            }
        }
    }
}
