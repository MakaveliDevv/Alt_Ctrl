using UnityEngine;

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
