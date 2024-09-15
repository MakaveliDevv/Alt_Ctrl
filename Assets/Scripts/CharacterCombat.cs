using System.Collections;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class CharacterCombat : MonoBehaviour
{
    private enum CharacterCombatState { IDLE, ATTACK, BLOCK, SUPERATTACK }
    [SerializeField] private CharacterCombatState combatState;

    [SerializeField] private GameObject weapon, shield, superAttackAbility;
    [SerializeField] private Transform superAttackPoint;
    
    // public bool ableToDoDamage;
    private Vector3 wpnStartPos, wpnEndPos, shieldStartPos, shieldEndPos;
    private Quaternion wpnStartRot, wpnEndRot, shieldStartRot, shieldEndRot;
    private readonly bool isWeaponMoving, isShieldMoving;
    private bool isQueued, isNoramlActive, isSuperActive, isBlockActive;

    void Awake() { combatState = CharacterCombatState.IDLE; }

    void Start() 
    {
        // Weapon & shield start position & rotation init
        wpnStartPos = new Vector3(.6f, .65f, 0f);
        wpnStartRot = new Quaternion(0f, 0f, 0.573576391f, 0.819152117f);

        shieldStartPos = new();
        shieldStartRot = new(); 

        // weapon & shield end position & rotation init
        wpnEndPos = new Vector3(1.35f, .15f, 0f);
        wpnEndRot = new Quaternion(0f, 0f, 0.130526155f, 0.991444886f);

        shieldEndPos = new();
        shieldEndRot = new();

        weapon.transform.SetLocalPositionAndRotation(wpnStartPos, wpnStartRot);
        // shield.transform.SetLocalPositionAndRotation(shieldStartPos, shieldStartRot);
    }

    void Update() 
    {
        switch (combatState)
        {
            case CharacterCombatState.ATTACK:
                Attack();

            break;

            case CharacterCombatState.BLOCK:

            break;

            case CharacterCombatState.SUPERATTACK:
                SuperAttack();

            break; 
        }

        if(!isSuperActive && Input.GetKeyDown(KeyCode.Q)) combatState = CharacterCombatState.ATTACK;
        
        else if(!isNoramlActive && Input.GetKeyDown(KeyCode.R)) combatState = CharacterCombatState.SUPERATTACK;
        
        else if(Input.GetKeyDown(KeyCode.E)) combatState = CharacterCombatState.BLOCK;
    }

    public void Attack() 
    {
        isNoramlActive = true;
        
        // Start animation // Control the animation from another script

        // if(isWeaponMoving) isQueued = true;
        // else StartCoroutine(MoveObject(weapon, wpnStartPos, wpnEndPos, wpnStartRot, wpnEndRot, isWeaponMoving, 2f));
        StartCoroutine(MoveObject(weapon, wpnStartPos, wpnEndPos, wpnStartRot, wpnEndRot, isWeaponMoving, 2f));
    }

    public void SuperAttack() 
    {
    
        isSuperActive = true;

        // Start animation // Control the animation from another script
        
        // Shoot fireball
        // if(isWeaponMoving) isQueued = true;
        // else StartCoroutine(MoveObject(weapon, wpnStartPos, wpnEndPos, wpnStartRot, wpnEndRot, isWeaponMoving, 2f));
        StartCoroutine(MoveObject(weapon, wpnStartPos, wpnEndPos, wpnStartRot, wpnEndRot, isWeaponMoving, 2f));
    
        // StartCoroutine(MoveWeapon()); 
        
        // Check if there is a colision between the this object and the other object
        
        // If there is collision
        
        // Get the opponen's stats script and apply damage

    }

    private void Block() 
    {
 
        isBlockActive = true;
        
        // Start animation

        // Block
        if(isShieldMoving) isQueued = true;
        else StartCoroutine(MoveObject(shield, shieldStartPos, shieldEndPos, shieldStartRot, shieldEndRot, isShieldMoving, 1.5f));
        

        // Start animation // Control the animation from another script
        
        // Block

        // Check if there is a colision between the this object and the other object
        
        // If blocked
        
        // Decrease armor
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

        if(@object.transform.localPosition == objectStartPos && @object.transform.localRotation == objectStartRot) 
        {
            // Move to destination
            TransformLerp(@object, objectEndPos, lerpDuration);
            RotateLerp(@object, objectEndRot, lerpDuration);
        }

        yield return new WaitForSeconds(.5f);
        
        if(isSuperActive) 
        {
            GameObject superAbility = Instantiate(superAttackAbility, superAttackPoint.position, quaternion.identity);
            superAbility.transform.SetParent(transform);
            isSuperActive = false;
        }

        if(@object.transform.localPosition == objectEndPos && @object.transform.localRotation == objectEndRot) 
        {
            // Move back to start position
            TransformLerp(@object, objectStartPos, lerpDuration);
            RotateLerp(@object, objectStartRot, lerpDuration);
        } 

        isMoving = false;
        combatState = CharacterCombatState.IDLE;

    }
}
        // Queue attack
        // if(isQueued) 
        // {
        //     isQueued = false;
        //     StartCoroutine(MoveObject(@object, objectStartPos, objectEndPos, objectStartRot, objectEndRot, isMoving, lerpDuration));
        //     combatState = CharacterCombatState.IDLE;
        // }

        // yield break;
