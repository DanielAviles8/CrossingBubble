using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    private bool alreadyHit;
    private float playerLives = 3;
    private float hitsTaken = 0;

    public Vector3 CheckpointPosition; 
    private bool isRespawning = false;  

    void Start()
    {
        alreadyHit = false;
    }

    void Update()
    {
        Debug.Log(hitsTaken); 
        Debug.Log(gameObject.transform.position);
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isRespawning || alreadyHit) return;

        if (hit.gameObject.CompareTag("Enemy"))
        {
            hitsTaken += 1;
            alreadyHit = true;
            StartCoroutine(ResetHitStatus()); 
        }

        if (hitsTaken >= 3)
        {
            StartCoroutine(RespawnPlayer());  
        }
    }

    private IEnumerator ResetHitStatus()
    {
        yield return new WaitForSeconds(0.5f); 
        alreadyHit = false;
    }

    private IEnumerator RespawnPlayer()
    {
        if (CheckpointPosition == Vector3.zero)
        {
            Debug.LogError("CheckpointPosition no está configurado correctamente.");
            yield break; 
        }

        isRespawning = true;  
        gameObject.transform.position = CheckpointPosition;  
        Debug.Log("Respawn");
        hitsTaken = 0; 

        yield return new WaitForSeconds(1f);
        isRespawning = false; 
    }
}
