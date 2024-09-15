using System.Collections;
using Unity.Mathematics;
using UnityEngine;

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

    void Awake() 
    { 
        combatState = CharacterCombatState.IDLE; 
        stats = GetComponent<CharacterStats>();
        circleCol = shield.GetComponent<CircleCollider2D>();
    }

    void Start() 
    {
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

        if(!isSuperActive && !isBlockActive && Input.GetKeyDown(KeyCode.Q)) combatState = CharacterCombatState.ATTACK;
        
        else if(!isNormalActive && !isBlockActive && Input.GetKeyDown(KeyCode.R)) combatState = CharacterCombatState.SUPERATTACK;
        
        if(!isNormalActive && !isSuperActive && Input.GetKeyDown(KeyCode.LeftShift)) combatState = CharacterCombatState.BLOCK;

        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            Block(false);
            combatState = CharacterCombatState.IDLE;
        } 
    }

    public void Attack() 
    {
        isNormalActive = true;
        StartCoroutine(MoveObject(weapon, wpnStartPos, wpnEndPos, wpnStartRot, wpnEndRot, isWeaponMoving, isNormalActive, 2f));
    }

    public void SuperAttack() 
    {
        isSuperActive = true;
        StartCoroutine(MoveObject(weapon, wpnStartPos, wpnEndPos, wpnStartRot, wpnEndRot, isWeaponMoving, isSuperActive, 3f));
    }

    private void Block(bool active) 
    {                
        if(active) 
        {
            TransformLerp(shield, shieldEndPos, 2f);
            circleCol.enabled = true;
            isBlockActive = true;
        }
        else 
        {
            TransformLerp(shield, shieldStartPos, 2f);
            circleCol.enabled = false;
            isBlockActive = false;
        }
    }

    // private IEnumerator EnergyPoints() 
    // {
    //     // Decrease value
    //     if(stats.currentEnergyPoints.GetValue() > 0 && stats.currentEnergyPoints.GetValue() <= stats.maxEnergyPoints.GetValue()) 
    //     {
    //         // Decrease points
    //     }
    //     else if(stats.currentEnergyPoints.GetValue() <= 0 && stats.currentEnergyPoints.GetValue() < stats.maxEnergyPoints.GetValue()) 
    //     {
    //         // Increase
    //     }

    //     yield break;
    // }

    // private IEnumerator EnergyPoints() 
    // {
    //     // While loop to keep checking energy conditions
    //     while (true)
    //     {
    //         // Decrease energy points if the player is actively doing something that costs energy (placeholder condition)
    //         if (stats.currentEnergyPoints.GetValue() > 0 && stats.currentEnergyPoints.GetValue() <= stats.maxEnergyPoints.GetValue()) 
    //         {
    //             // Placeholder condition to decrease energy points (e.g., player is sprinting or using an ability)
    //             if (/* Condition for decreasing energy, e.g., isSprinting or isUsingAbility */) 
    //             {
    //                 stats.currentEnergyPoints.SetValue(stats.currentEnergyPoints.GetValue() - 1);
    //                 Debug.Log("Energy decreased: " + stats.currentEnergyPoints.GetValue());
    //             }
    //         }
            
    //         // Increase energy points when they are below the max but not zero
    //         else if (stats.currentEnergyPoints.GetValue() <= 0 || stats.currentEnergyPoints.GetValue() < stats.maxEnergyPoints.GetValue()) 
    //         {
    //             // Placeholder condition to increase energy points (e.g., player is resting)
    //             if (/* Condition for increasing energy, e.g., isResting */) 
    //             {
    //                 stats.currentEnergyPoints.SetValue(stats.currentEnergyPoints.GetValue() + 1);
    //                 Debug.Log("Energy increased: " + stats.currentEnergyPoints.GetValue());
    //             }
    //         }

    //         // Yield for a short duration before the next energy update
    //         yield return new WaitForSeconds(1f);
    //     }
    // }

    
    private IEnumerator MoveObject(
        GameObject @object, 
        Vector3 objectStartPos, 
        Vector3 objectEndPos, 
        Quaternion objectStartRot, 
        Quaternion objectEndRot, 
        bool isMoving,
        bool isAttackActive, 
        float lerpDuration
    ) 
    {
        if(isMoving) yield break;

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

        HandleAttackStates(isWeaponMoving, ref isAttackActive, ref isSuperActive, ability, abilitySpawnPoint);
        HandleAttackStates(isWeaponMoving, ref isAttackActive, ref isNormalActive);
        HandleAttackStates(isShieldMoving, ref isAttackActive, ref isBlockActive);

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

    private void HandleAttackStates(bool isObjectMoving, ref bool isAttackActive, ref bool abilityActiveState, GameObject abilityPrefab = null, Transform abilitySpawnPoint = null) 
    {
        if(isObjectMoving && isAttackActive == abilityActiveState) 
        {
            if(abilityPrefab != null && abilitySpawnPoint != null) 
            {
                GameObject ability = Instantiate(abilityPrefab, abilitySpawnPoint.position, quaternion.identity);
                ability.transform.SetParent(transform);
            }
        }

        isAttackActive = false;
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

    // void OnDrawGizmos() 
    // {
    //     Gizmos.color = Color.green;
    //     if(cirlceCol != null) 
    //     {
    //         Vector3 colliderCenter = shieldSpawnPoint.position + (Vector3)cirlceCol.offset;
    //         Gizmos.DrawWireSphere(colliderCenter, cirlceCol.radius);
    //     }
    // }
}

// if(isWeaponMoving && isAttackActive == isSuperActive) 
// {
//     GameObject superAbility = Instantiate(ability, abilitySpawnPoint.position, quaternion.identity);
//     superAbility.transform.SetParent(transform);

//     isAttackActive = false;
//     isSuperActive = isAttackActive;
// }
// else if(isWeaponMoving && isAttackActive == isNormalActive)
// {
//     isAttackActive = false;
//     isNormalActive = isAttackActive;
// }
// else if(isShieldMoving && isAttackActive == isBlockActive) 
// {
//     isAttackActive = false;
//     isBlockActive = isAttackActive;
// }

// Queue attack
// if(isQueued) 
// {
//     isQueued = false;
//     StartCoroutine(MoveObject(@object, objectStartPos, objectEndPos, objectStartRot, objectEndRot, isMoving, lerpDuration));
//     combatState = CharacterCombatState.IDLE;
// }

// yield break;
