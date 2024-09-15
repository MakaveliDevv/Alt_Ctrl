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
        float elapsedTime = 0f;
        stats.shieldDurationBar.gameObject.SetActive(true);
        stats.shieldDurationBar.SetMaxValue(duration);

        while (elapsedTime < duration && isBlockActive)  // Block is still active
        {
            elapsedTime += Time.deltaTime;
            stats.shieldDurationBar.SetValue(elapsedTime);

            yield return null;
        }

        if (!isBlockActive)  // If the shield was dropped manually, stop increasing the bar
        {
            yield break;
        }

        // Once the duration has run out, disable the shield and bar
        Block(false);
        combatState = CharacterCombatState.IDLE;

        Debug.Log("Blocking stopped...");
    }

   private IEnumerator BlockDurationDecrease(float duration)
    {
        float elapsedTime = stats.shieldDurationBar.slider.value; 
        float decreaseDuration = elapsedTime; 

        while (elapsedTime > 0f)
        {
            elapsedTime -= Time.deltaTime;
            float normalizedValue = Mathf.Clamp01(elapsedTime / decreaseDuration); // Normalize to 0-1 range
            stats.shieldDurationBar.SetValue(normalizedValue * duration); // Gradually decrease the value

            yield return null; 
        }

        // Once fully decreased, hide the bar
        stats.shieldDurationBar.gameObject.SetActive(false);
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
        if(active) 
        {
            TransformLerp(shield, shieldEndPos, 2f);
            circleCol.enabled = true;
            isBlockActive = true;

            StartCoroutine(BlockDurationIncrease(3f));   
        }
        else 
        {
            TransformLerp(shield, shieldStartPos, 2f);
            circleCol.enabled = false;
            isBlockActive = false;

            StopAllCoroutines();
            StartCoroutine(BlockDurationDecrease(3f));  
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
        HandleAttackStates(isShieldMoving, ref isBlockActive);

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
