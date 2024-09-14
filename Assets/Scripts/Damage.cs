using UnityEngine;

public class Damage : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider) 
    {
        if(collider.CompareTag("Player")) 
        {
            Debug.Log("Hitt the player");

            var player = collider.gameObject;

            if(player.TryGetComponent<CharacterStats>(out var stats)) 
            {
                // if(combat.ableToDoDamage) 
                // {
                    stats.TakeDamage(20f);
                    Debug.Log("Damage done...");
                // }
            }
        }
    } 
}
