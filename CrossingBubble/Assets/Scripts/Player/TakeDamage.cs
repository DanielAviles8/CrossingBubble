using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    private bool alreadyHit;
    private float playerLives = 3;
    private float hitsTaken = 0;

    public Vector3 CheckpointPosition;
    void Start()
    {
        alreadyHit = false;
    }

    void Update()
    {
        Debug.Log(hitsTaken);
        Debug.Log(alreadyHit);
    }
    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (alreadyHit) return;

        if (hit.gameObject.CompareTag("Enemy"))
        {
            hitsTaken += 1;
            alreadyHit = true; 
            StartCoroutine(ResetHitStatus()); 
        }
        if(hitsTaken >= 3)
        {
            RespawnPlayer();
        }
    }

    private IEnumerator ResetHitStatus()
    {
        yield return new WaitForSeconds(1f);
        alreadyHit = false;
    }
    public void RespawnPlayer()
    {
        gameObject.transform.position = CheckpointPosition;
        hitsTaken = 0;   
    }
}