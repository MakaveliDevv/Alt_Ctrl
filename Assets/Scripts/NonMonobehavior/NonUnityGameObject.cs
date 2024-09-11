// using System;
// using UnityEngine;

// public class NonUnityGameObject
// {
//     protected GameObject myInstance;
//     [SerializeField] protected string prefabName;

//     public NonUnityGameObject(string _prefabName = "", Vector2 _pos = new())
//     {
//         this.prefabName = _prefabName;

//         try 
//         {
//             var myObject = Resources.Load<GameObject>(prefabName);
//             myInstance = GameObject.Instantiate(myObject);

//             myInstance.transform.position = _pos;

//         }
//         catch(Exception)
//         {
//             Debug.Log($"Can't instantiate {prefabName} right now");
//         }
//     }

//     public void CustomUpdate(float deltaTime) 
//     { 
//         myInstance.transform.position = myInstance.transform.position + Vector3.right * deltaTime;
//     }
// }
