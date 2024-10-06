using UnityEngine;
using System.IO.Ports;

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
    public GameManager gameManager;

    void Start() 
    {        
        currentHealth.SetValue(maxHealth.GetValue());
        healthBar.SetMaxValue(currentHealth.GetValue());
        currentEnergy.SetValue(maxEnergy.GetValue());

        SendHealthToArduino(); 
    }

    public void TakeDamage(float damage) 
    {
        currentHealth.SetValue(currentHealth.GetValue() - damage);
        healthBar.SetValue(currentHealth.GetValue());

        Debug.Log("Took damage: " + damage + ", Current Health: " + currentHealth.GetValue());
        SendHealthToArduino(); // Send updated health value
    }

    private void SendHealthToArduino() 
    {
        if (gameManager.arduino.data_Stream != null && gameManager.arduino.data_Stream.IsOpen) 
        {
            int healthPercent = (int)((currentHealth.GetValue() / maxHealth.GetValue()) * 100);
            gameManager.arduino.data_Stream.WriteLine($"HP:{healthPercent}"); // Send health percentage to Arduino
            Debug.Log("Sent Health to Arduino: HP:" + healthPercent);
        }
        else
        {
            Debug.LogError("Serial port is not open or is null.");
        }
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

    void OnApplicationQuit()
    {
        if (gameManager.arduino.data_Stream != null && gameManager.arduino.data_Stream.IsOpen)
        {
            gameManager.arduino.data_Stream.Close();
            Debug.Log("Serial port closed.");
        }
    }
}
