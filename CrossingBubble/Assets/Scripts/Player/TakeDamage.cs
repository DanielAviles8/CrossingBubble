using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TakeDamage : MonoBehaviour
{
    private CharacterController _characterController;

    private bool alreadyHit;
    private float playerLives = 3;
    private float hitsTaken = 0;

    public Transform firstCheckpoint;
    public Vector3 CheckpointPosition;
    public Vector3 LastCheckpoint;
    private bool isRespawning = false;

    public Transform Check1;
    public Transform Check2;

    void Start()
    {
        CheckpointPosition = firstCheckpoint.position;
        _characterController = GetComponent<CharacterController>();
        alreadyHit = false;
    }

    void Update()
    {
        Debug.Log(CheckpointPosition.ToString());
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

        if (hit.gameObject.CompareTag("Checkpoint"))
        {
            LastCheckpoint = hit.gameObject.transform.position;
            hit.gameObject.GetComponent<BoxCollider>().enabled = false;
            UpdateCheckpoint();
        }
        if (hit.gameObject.CompareTag("Check1"))
        {
            _characterController.enabled = false;
            gameObject.transform.position = Check1.position;
            _characterController.enabled = true;
        }
        if (hit.gameObject.CompareTag("EndGame"))
        {
            SceneManager.LoadScene("Credits");
        }
    }
    public void UpdateCheckpoint()
    {
        CheckpointPosition = LastCheckpoint;
    }
    private IEnumerator ResetHitStatus()
    {
        yield return new WaitForSeconds(0.5f); 
        alreadyHit = false;
    }

    private IEnumerator RespawnPlayer()
    {
        isRespawning = true;
        _characterController.enabled = false;
        gameObject.transform.position = CheckpointPosition;  
        Debug.Log("Respawn");
        hitsTaken = 0; 

        yield return new WaitForSeconds(1f);
        isRespawning = false; 
        _characterController.enabled = true;
    }
}
