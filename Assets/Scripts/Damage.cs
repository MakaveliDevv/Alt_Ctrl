using UnityEngine;

public class Damage : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider) 
    {
        if(collider.CompareTag("Player")) if(collider.gameObject.TryGetComponent<CharacterStats>(out var stats)) stats.TakeDamage(20f);
    } 
}
