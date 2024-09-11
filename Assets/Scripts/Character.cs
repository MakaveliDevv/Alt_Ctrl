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
        movementInputGetter = GetComponent<IMovementInputGetter>();
        char_combat = GetComponent<CharacterCombat>();

        currentHealth.SetValue(maxHealth.GetValue());
    }
    
    void Update() 
    {
        Move();
        char_combat.Attack();
        char_combat.SuperAttack();
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
