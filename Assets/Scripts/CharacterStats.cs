using Unity.VisualScripting;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] protected Stats maxHealth;
    [SerializeField] public Stats currentHealth;
    [SerializeField] protected Stats damage;
    [SerializeField] protected Stats armor;
    [SerializeField] protected Stats movementSpeed;

    [SerializeField] private Stats maxEnergy;
    [SerializeField] private Stats currentEnergy;

    [SerializeField] private Bar healthBar;
    [SerializeField] private Bar energyBar;
    public Bar shieldDurationBar;

    void Start() 
    {
        currentHealth.SetValue(maxHealth.GetValue());
        healthBar.SetMaxValue(currentHealth.GetValue());

        currentEnergy.SetValue(maxEnergy.GetValue());
    }

    public void TakeDamage(float damage) 
    {
        currentHealth.SetValue(currentHealth.GetValue() - damage);
        healthBar.SetValue(currentHealth.GetValue());
    }

    public void Energy(int value, bool increment) 
    {
        if(increment) 
        {
            currentEnergy.SetValue(currentEnergy.GetValue() + value);
            energyBar.SetValue(currentEnergy.GetValue());
        } 
        else 
        {
            currentEnergy.SetValue(currentEnergy.GetValue() - value);
            energyBar.SetValue(currentEnergy.GetValue());
        }
    } 
}
