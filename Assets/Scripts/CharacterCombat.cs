using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using System.IO.Ports;

[RequireComponent(typeof(CharacterStats))]
public class CharacterCombat : MonoBehaviour
{
    private enum CharacterCombatState { IDLE, ATTACK, BLOCK, SUPERATTACK }
    [SerializeField] private CharacterCombatState combatState;
    [SerializeField] private GameObject weapon, shield, ability;
    [SerializeField] private Transform abilitySpawnPoint;
    [SerializeField] private bool isWeaponMoving, isShieldMoving;

    private CharacterStats stats;
    private CircleCollider2D circleCol;
    private Vector3 wpnStartPos, wpnEndPos, shieldStartPos, shieldEndPos;
    private Quaternion wpnStartRot, wpnEndRot;
    private bool isNormalActive, isSuperActive, isBlockActive, isQueued;

    private bool isBlockMaxedOut; 
    private float extraDecreaseTime = 3f; 

    private readonly SerialPort data_Stream = new("COM5", 9600);
    private string value;

    void Awake() 
    { 
        combatState = CharacterCombatState.IDLE; 
        stats = GetComponent<CharacterStats>();
        circleCol = shield.GetComponent<CircleCollider2D>();
    }

    void Start() 
    {
        // try
        // {
        //     data_Stream.Open();
        // }
        // catch (System.Exception e)
        // {
        //     Debug.LogError("Error opening serial port: " + e.Message);
        // }
        
        // Weapon & shield start position & rotation init
        wpnStartPos = new Vector3(.6f, .65f, 0f);
        wpnStartRot = new Quaternion(0f, 0f, 0.573576391f, 0.819152117f);

        shieldStartPos = new Vector3(.5f, -.5f, 0f);

        // weapon & shield end position & rotation init
        wpnEndPos = new Vector3(1.35f, .15f, 0f);
        wpnEndRot = new Quaternion(0f, 0f, 0.130526155f, 0.991444886f);

        shieldEndPos = new Vector3(0.5f, 0f, 0f);
        weapon.transform.SetLocalPositionAndRotation(wpnStartPos, wpnStartRot);
        shield.transform.SetLocalPositionAndRotation(shieldStartPos, shield.transform.localRotation);
    }

    // Move this to the character class
    void Update() 
    {
        // Check for serial data from Arduino
        if (data_Stream.IsOpen && data_Stream.BytesToRead > 0)
        {
            value = data_Stream.ReadLine().Trim();
            Debug.Log("Received value: " + value); // Debug message

            // Force sensor logic
            if (int.TryParse(value, out int sensorValue) && sensorValue >= 200) 
            {
                if (!isNormalActive && !isBlockActive)
                {
                    combatState = CharacterCombatState.SUPERATTACK;
                    Debug.Log("Super Attack triggered by force sensor!");
                }
            }
            else if (value.Contains("ROTATED:")) // Check for rotation message
            {
                Debug.Log("Rotation detected, setting combat state to NORMALATTACK.");
                if (!isNormalActive && !isBlockActive)
                {
                    combatState = CharacterCombatState.ATTACK;
                    Debug.Log("Normal Attack triggered by rotation!");
                }
            }
        }
    
                // // Extract rotation and acceleration values
                // string[] parts = value.Split(',');
                // if (parts.Length == 2)
                // {
                //     string rotatedPart = parts[0].Split(':')[1].Trim();
                //     // string accelPart = parts[1].Split(':')[1].Trim();

                //     bool hasRotated = rotatedPart == "1";
                //     // bool hasAccelerated = accelPart == "1";

                //     // Check if both conditions are met for normal attack
                //     if (hasRotated && !isSuperActive && !isBlockActive)
                //     {
                //         combatState = CharacterCombatState.ATTACK;
                //         Debug.Log("Normal Attack triggered by MPU-board!");
                //     }
                // }
     

            // MPU logic for NORMALATTACK
            // else if (value == "NORMALATTACK")
            // {
            //     if (!isNormalActive && !isBlockActive && !isSuperActive)
            //     {
            //         combatState = CharacterCombatState.ATTACK;
            //         Debug.Log("Combat state set to NORMALATTACK"); // Debug message
            //     }
            // }

            // else
            // {
            //     // Check for MPU rotation and acceleration data
            //     string[] sensorData = value.Split(',');
            //     if (sensorData.Length == 2 
            //         && float.TryParse(sensorData[0], out float rotationValue) 
            //         && float.TryParse(sensorData[1], out float accelerationValue))
            //     {
            //         float rotationThreshold = 90f; // Example threshold for rotation in degrees
            //         float accelerationThreshold = .5f; // Example threshold for acceleration in g's

            //         if (rotationValue >= rotationThreshold && accelerationValue >= accelerationThreshold)
            //         {
            //             if (!isNormalActive && !isBlockActive)
            //             {
            //                 combatState = CharacterCombatState.ATTACK;
            //             }
            //         }
            //     }
            // }
        
        // // Check for serial data from Arduino
        // if (data_Stream.IsOpen && data_Stream.BytesToRead > 0)
        // {
        //     value = data_Stream.ReadLine().Trim();
        //     Debug.Log("Received value: " + value); // Debug message

        //     // Force sensor
        //     if (int.TryParse(value, out int sensorValue) && sensorValue >= 200) 
        //     {
        //         if (!isNormalActive && !isBlockActive)
        //         {
        //             combatState = CharacterCombatState.SUPERATTACK;
        //         }
        //     }
        // }

        switch (combatState)
        {
            case CharacterCombatState.ATTACK:
                Attack();

            break;

            case CharacterCombatState.BLOCK:
                Block(true);

            break;

            case CharacterCombatState.SUPERATTACK:
                SuperAttack();

            break; 
        }

        // if(!isSuperActive && !isBlockActive && Input.GetKeyDown(KeyCode.Q)) combatState = CharacterCombatState.ATTACK;
        
        // else if(!isNormalActive && !isBlockActive && Input.GetKeyDown(KeyCode.R)) combatState = CharacterCombatState.SUPERATTACK;
        
        if(!isNormalActive && !isSuperActive && Input.GetKeyDown(KeyCode.LeftShift))
        {
            combatState = CharacterCombatState.BLOCK;
            // StartCoroutine(BlockDuration(3f));
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            Block(false);
            combatState = CharacterCombatState.IDLE;
        }
    }

    private IEnumerator BlockDurationIncrease(float duration)
    {
        float elapsedTime = stats.shieldDurationBar.slider.value; 
        stats.shieldDurationBar.SetMaxValue(duration);

        while (elapsedTime < duration && isBlockActive)
        {
            elapsedTime += Time.deltaTime;
            stats.shieldDurationBar.SetValue(elapsedTime);

            yield return null;
        }
        
        if (elapsedTime >= duration)
        {
            isBlockMaxedOut = true; 
            StartCoroutine(BlockDurationDecrease(duration)); 
            Block(false); 
        }
        else
        {
            StartCoroutine(BlockDurationDecrease(elapsedTime));
        }
    
        combatState = CharacterCombatState.IDLE;
    }

    private IEnumerator BlockDurationDecrease(float startValue)
    {
        float elapsedTime = startValue;
        float decreaseDuration = startValue; 

        if (isBlockMaxedOut)
        {
            decreaseDuration += extraDecreaseTime;
        }

        while (elapsedTime > 0f)
        {
            elapsedTime -= Time.deltaTime * (startValue / decreaseDuration);
            stats.shieldDurationBar.SetValue(elapsedTime);

            yield return null;
        }

        // Once fully decreased, hide the bar
        // stats.shieldDurationBar.gameObject.SetActive(false);
        isBlockMaxedOut = false; 
    }

    public void Attack() 
    {
        isNormalActive = true;
        StartCoroutine(MoveObject(weapon, wpnStartPos, wpnEndPos, wpnStartRot, wpnEndRot, isWeaponMoving, 2f));
    }

    public void SuperAttack() 
    {
        isSuperActive = true;
        StartCoroutine(MoveObject(weapon, wpnStartPos, wpnEndPos, wpnStartRot, wpnEndRot, isWeaponMoving, 3f));
    }
    
    private void Block(bool active) 
    {                
        if(active && !isBlockMaxedOut) 
        {
            TransformLerp(shield, shieldEndPos, 2f);
            circleCol.enabled = true;
            isBlockActive = true;

            StopAllCoroutines(); 
            StartCoroutine(BlockDurationIncrease(3f));   
        }
        else 
        {
            TransformLerp(shield, shieldStartPos, 2f);
            circleCol.enabled = false;
            isBlockActive = false;

            StopAllCoroutines();

            float currentValue = stats.shieldDurationBar.slider.value;
            StartCoroutine(BlockDurationDecrease(currentValue));  
        }
    }
    
    private IEnumerator MoveObject(
        GameObject @object, 
        Vector3 objectStartPos, 
        Vector3 objectEndPos, 
        Quaternion objectStartRot, 
        Quaternion objectEndRot, 
        bool isMoving,
        float lerpDuration
    ) 
    {
        if(isMoving) yield break;

        yield return new WaitForEndOfFrame();

        if(ReturnObjectPosAndRot(@object, objectStartPos, objectStartRot)) 
        {
            // Move to destination
            TransformLerp(@object, objectEndPos, lerpDuration);

            if(@object != shield) RotateLerp(@object, objectEndRot, lerpDuration);

            isMoving = true; 
            isWeaponMoving = isMoving;  
            
            if(@object == shield) isShieldMoving = isMoving;
        }

        yield return new WaitForSeconds(.5f);

        HandleAttackStates(isWeaponMoving, ref isSuperActive, ability, abilitySpawnPoint);
        HandleAttackStates(isWeaponMoving, ref isNormalActive);

        if(ReturnObjectPosAndRot(@object, objectEndPos, objectEndRot)) 
        {
            // Move back to start position
            TransformLerp(@object, objectStartPos, lerpDuration);
            if(@object != shield) RotateLerp(@object, objectStartRot, lerpDuration);

            isMoving = false;
            isWeaponMoving = isMoving;  

            if(@object == shield) isShieldMoving = isMoving;
        }

        combatState = CharacterCombatState.IDLE;
    }


    private void TransformLerp(GameObject @object, Vector2 destination, float duration) 
    {
        float elapsedTime = 0f;
        while(elapsedTime < duration) 
        {
            float t = elapsedTime / duration;
            @object.transform.localPosition = Vector2.Lerp(@object.transform.localPosition, destination, t);
            elapsedTime += Time.deltaTime;
        }
    }

    private void RotateLerp(GameObject @object, Quaternion destination, float duration) 
    {  
        float elapsedTime = 0f;
        while(elapsedTime < duration) 
        {
            float t = elapsedTime / duration;
            @object.transform.localRotation = Quaternion.Lerp(@object.transform.localRotation, destination, t);
            elapsedTime += Time.deltaTime;
        }
    }

    private void HandleAttackStates(bool isObjectMoving, ref bool abilityActiveState, GameObject abilityPrefab = null, Transform abilitySpawnPoint = null) 
    {
        if(isObjectMoving && abilityActiveState) 
        {
            if(abilityPrefab != null && abilitySpawnPoint != null) 
            {
                GameObject ability = Instantiate(abilityPrefab, abilitySpawnPoint.position, quaternion.identity);
                ability.transform.SetParent(transform);
            }
        }

        abilityActiveState = false;
    }

    private bool ReturnObjectPosAndRot(GameObject @object, Vector3 v, Quaternion r)  
    {
        if(@object.transform.localPosition == v && @object.transform.localRotation == r) 
        {
            return true;
        }

        return false;
    }
}
