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

    private SerialPort data_Stream = new("COM5", 9600);

    void Start() 
    {
        try
        {
            if (!data_Stream.IsOpen)
            {
                data_Stream.Open();
                Debug.Log("Serial port opened successfully.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error opening serial port: " + e.Message);
        }
        
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

    // private void SendHealthToArduino() 
    // {
    //     if (data_Stream.IsOpen) 
    //     {
    //         int healthPercent = (int)(currentHealth.GetValue() / maxHealth.GetValue() * 100);
    //         data_Stream.WriteLine($"HP:{healthPercent}"); // Send health percentage to Arduino
    //         Debug.Log("Sent Health to Arduino: " + healthPercent);
    //     }
    // }

    private void SendHealthToArduino() 
    {
        if (data_Stream != null && data_Stream.IsOpen) 
        {
            int healthPercent = (int)((currentHealth.GetValue() / maxHealth.GetValue()) * 100);
            data_Stream.WriteLine($"HP:{healthPercent}"); // Send health percentage to Arduino
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
        if (data_Stream != null && data_Stream.IsOpen)
        {
            data_Stream.Close();
            Debug.Log("Serial port closed.");
        }
    }
}
