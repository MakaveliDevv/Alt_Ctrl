using UnityEngine;

/// <summary>
/// This class is meant to initialize the stats of the characters and bring changes to the stats of the character
/// </summary>
public class CharacterStats : MonoBehaviour
{
    [SerializeField] protected Stats maxHealth;
    [SerializeField] public Stats currentHealth;
    [SerializeField] protected Stats damage;
    [SerializeField] protected Stats armor;
    [SerializeField] protected Stats movementSpeed;

    public void TakeDamage(float damage) 
    {
        currentHealth.SetValue(currentHealth.GetValue() - damage);
    }
}
