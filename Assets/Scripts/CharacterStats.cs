using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] protected Stats maxHealth;
    [SerializeField] public Stats currentHealth;
    [SerializeField] protected Stats damage;
    [SerializeField] protected Stats armor;
    [SerializeField] protected Stats movementSpeed;

    public Stats maxEnergyPoints;
    public Stats currentEnergyPoints;

    public HealthBar healthBar;

    void Start() 
    {
        currentHealth.SetValue(maxHealth.GetValue());
        healthBar.SetMaxHealth((int)currentHealth.GetValue());

        currentEnergyPoints.SetValue(maxEnergyPoints.GetValue());
    }

    public void TakeDamage(float damage) 
    {
        currentHealth.SetValue(currentHealth.GetValue() - damage);
        healthBar.SetHealth((int)currentHealth.GetValue());
    }
}
