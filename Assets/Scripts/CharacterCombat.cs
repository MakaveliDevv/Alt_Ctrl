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
    
    private Vector3 wpnStartPos, wpnEndPos;
    private Quaternion wpnStartRot, wpnEndRot;
    private bool isNormalActive, isSuperActive, isBlockActive, shieldRelease, isQueued;

    void Awake() 
    { 
        combatState = CharacterCombatState.IDLE; 
    }

    void Start() 
    {
        // Weapon & shield start position & rotation init
        wpnStartPos = new Vector3(.6f, .65f, 0f);
        wpnStartRot = new Quaternion(0f, 0f, 0.573576391f, 0.819152117f);

        // weapon & shield end position & rotation init
        wpnEndPos = new Vector3(1.35f, .15f, 0f);
        wpnEndRot = new Quaternion(0f, 0f, 0.130526155f, 0.991444886f);

        weapon.transform.SetLocalPositionAndRotation(wpnStartPos, wpnStartRot);
        // shield.transform.SetLocalPositionAndRotation(shieldStartPos, shieldStartRot);
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
                Block();

            break;

            case CharacterCombatState.SUPERATTACK:
                SuperAttack();

            break; 
        }

        if(!isSuperActive && Input.GetKeyDown(KeyCode.Q)) combatState = CharacterCombatState.ATTACK;
        
        else if(!isNormalActive && Input.GetKeyDown(KeyCode.R)) combatState = CharacterCombatState.SUPERATTACK;
        
        if(!isNormalActive && !isSuperActive && Input.GetKeyDown(KeyCode.LeftShift)) combatState = CharacterCombatState.BLOCK;

        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            shieldRelease = true;
            combatState = CharacterCombatState.IDLE;
        } 

        if(shieldRelease)
        {
            shield.SetActive(false);
            isBlockActive = false;
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

    private void Block() 
    {
        StartCoroutine(ShieldActivation());

        // StartCoroutine(MoveObject(shield, shieldStartPos, shieldEndPos, wpnStartRot, wpnEndRot, isWeaponMoving, isSuperActive, 3f));
    }

    private IEnumerator ShieldActivation() 
    {                
        shield.SetActive(true);
        isBlockActive = true;
        
        yield return new WaitForSeconds(.5f);

        shield.SetActive(false);
        isBlockActive = false;

        yield break;
    }

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
            RotateLerp(@object, objectEndRot, lerpDuration);

            isMoving = true;
            // if(@object == weapon) 
            isWeaponMoving = isMoving;  
            
            // else if(@object == shield) isShieldMoving = isMoving;
        }

        yield return new WaitForSeconds(.5f);

        HandleAttackStates(isWeaponMoving, ref isAttackActive, ref isSuperActive, ability, abilitySpawnPoint);
        HandleAttackStates(isWeaponMoving, ref isAttackActive, ref isNormalActive);

        if(ReturnObjectPosAndRot(@object, objectEndPos, objectEndRot)) 
        {
            // Move back to start position
            TransformLerp(@object, objectStartPos, lerpDuration);
            RotateLerp(@object, objectStartRot, lerpDuration);

            isMoving = false;
            // if(@object == weapon) 
            isWeaponMoving = isMoving;  

            // else if(@object == shield) isShieldMoving = isMoving;
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
