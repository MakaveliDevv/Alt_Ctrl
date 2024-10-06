using UnityEngine;
public class GameManager : MonoBehaviour 
{
    public Arduino arduino;

    void Awake() 
    {
        arduino = new Arduino();
        arduino.Start();
    }
}