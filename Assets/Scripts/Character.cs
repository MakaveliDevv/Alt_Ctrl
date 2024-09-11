using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Some methods meant for the character's behavior 
/// </summary>

public class Character : CharacterStats
{
    private IMovementInputGetter movementInputGetter;
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
        Vector2 movement = new Vector2 
        {
            x = movementInputGetter.Horizontal,
            y = movementInputGetter.Vertical
        };

        movement *= movementSpeed.GetValue() * Time.deltaTime;
        transform.Translate(movement);
    }
}
