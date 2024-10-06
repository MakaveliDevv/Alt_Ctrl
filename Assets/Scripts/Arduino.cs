// using System.IO.Ports;

// public class Arduino 
// {
//     private const string portName = "COM5"; 
//     private const int baudRate = 9600; 

//     public SerialPort data_Stream = new(portName, baudRate);
//     public string value;

//     public void Start() 
//     {
//         // Open the Arduino serial port
//         try
//         {
//             data_Stream.Open();
//             UnityEngine.Debug.Log("Serial port open");
//         }
//         catch (System.Exception e)
//         {
//             UnityEngine.Debug.LogError("Error opening serial port: " + e.Message);
//         }
//     }

//     public void OnApplicationQuit() 
//     {
//         // Close the serial port when quitting the application
//         if (data_Stream.IsOpen)
//         {
//             data_Stream.Close();
//         }
//     }
// }