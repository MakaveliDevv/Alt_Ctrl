using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class CharacterCombat : MonoBehaviour
{
    public bool ableToDoDamage;
    [SerializeField] private GameObject weapon, superAttackAbility;
    [SerializeField] private Transform superAttackPoint;
    private Vector3 weaponStart_Pos, weaponEnd_Pos;
    private Quaternion weaponStart_Rot, weaponEnd_Rot;
    private bool isWeaponMoving, isQueued, superAbilityActivated;

    void Start() 
    {
        // Weapon init
        weaponStart_Pos = new Vector3(.6f, .65f, 0f);
        weaponStart_Rot = new Quaternion(0f, 0f, 0.573576391f, 0.819152117f); 

        weapon.transform.localPosition = weaponStart_Pos;
        weapon.transform.localRotation = weaponStart_Rot;

        weaponEnd_Pos = new Vector3(1.35f, .15f, 0f);
        weaponEnd_Rot = new Quaternion(0f, 0f, 0.130526155f, 0.991444886f);
    }
    
    public void Attack() 
    {
      
        if(Input.GetKeyDown(KeyCode.Q)) 
        {
            Debug.Log("Key input detected!");
            // Start animation // Control the animation from another script

            // Swing sword
            if(isWeaponMoving) 
            {
                isQueued = true;
            } 
            else 
            {
                StartCoroutine(MoveWeapon());
            } 

            // // Check if there is a colision between the this object and the other object
            // var weaponCol = weapon.GetComponentInChildren<CircleCollider2D>();

            // Collider2D hitObject = Physics2D.OverlapCircle(weaponCol.transform.position, weaponCol.radius, LayerMask.GetMask("Player"));
            
            // // If there is collision
            // if(hitObject.gameObject.CompareTag("Player")) 
            // {
            //     Debug.Log("Hitt the player");
            //     var player = hitObject.gameObject;

            //     // Get the opponen's stats script and apply damage
                
            //     if(player.TryGetComponent<CharacterStats>(out var stats)) 
            //     {
            //         stats.TakeDamage(20f);
            //         Debug.Log("Damage done...");
            //     }
            // }
            // else if(hitObject) { Debug.Log("Hitt an object but couldn't detect the Player"); }  
        }
    }

    public void SuperAttack() 
    {
        if(Input.GetKeyDown(KeyCode.R)) 
        {
            superAbilityActivated = true;
            // Start animation // Control the animation from another script
            
            // Shoot fireball
            if(isWeaponMoving) 
            {
                isQueued = true;
            } 
            else 
            {
                StartCoroutine(MoveWeapon());
            } 
            
            // Check if there is a colision between the this object and the other object
            
            // If there is collision
            
            // Get the opponen's stats script and apply damage
        }

    }

    private void Block() 
    {
        // Start animation // Control the animation from another script
        // Swing sword
        // Check if there is a colision between the this object and the other object
        // If there is collision
        // Get the opponen's stats script and apply damage

    }

     private void TransformObject(GameObject _object, Vector2 _destination, float _elapsedTime, float _duration) 
    {
        while(_elapsedTime < _duration) 
        {
            float t = _elapsedTime / _duration;
            _object.transform.localPosition = Vector2.Lerp(_object.transform.localPosition, _destination, t);
            _elapsedTime += Time.deltaTime;
        }
    }

    private void RotateObject(GameObject _object, Quaternion _destination, float _elapsedTime, float _duration) 
    {  
        while(_elapsedTime < _duration) 
        {
            float t = _elapsedTime / _duration;
            _object.transform.localRotation = Quaternion.Lerp(_object.transform.localRotation, _destination, t);
            _elapsedTime += Time.deltaTime;
        }
    }

    private IEnumerator MoveWeapon() 
    {
        if (isWeaponMoving) yield break; 

        isWeaponMoving = true;
        Debug.Log("Coroutine started...");

        yield return new WaitForSeconds(.5f);

        if(weapon.transform.localPosition == weaponStart_Pos) 
        {
            // Transform to destination
            TransformObject(weapon, weaponEnd_Pos, 0f, 2f);

            // Rotate to destination
            RotateObject(weapon, weaponEnd_Rot, 0f, 2f);


            ableToDoDamage = true;
        }

        ableToDoDamage = false;

        if(!superAbilityActivated) 
            yield return new WaitForSeconds(.5f);

        else 
        {
            yield return new WaitForSeconds(.1f);

            Instantiate(superAttackAbility, superAttackPoint.position, quaternion.identity);
            
            yield return new WaitForSeconds(.5f);
        }
        
        if(weapon.transform.localPosition == weaponEnd_Pos) 
        {
            // Transform back
            TransformObject(weapon, weaponStart_Pos, 0f, 1f);

            // Rotate back
            RotateObject(weapon, weaponStart_Rot, 0f, 1f);
        }        

        isWeaponMoving = false;

        // Queue attack
        if(isQueued) 
        {
            isQueued = false;
            StartCoroutine(MoveWeapon());
        }
    }
}
