using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody rb;

    public float movHor = 1f; 
    public float speed = 3f;

    public LayerMask groundLayer;

    public float frontGrndRayDist = 0.25f;
    public float floorCheckY = 0.52f;
    public float frontCheckDist = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No se encontró un Rigidbody en el enemigo.");
        }
    }

    void Update()
    {
        Vector3 floorCheckPosition = new Vector3(transform.position.x, transform.position.y - floorCheckY, transform.position.z);
        bool isGroundFloor = Physics.Raycast(floorCheckPosition, Vector3.down, frontGrndRayDist, groundLayer);

        if (!isGroundFloor)
        {
            movHor *= -1; 
        }

        Vector3 frontCheckPosition = new Vector3(transform.position.x + movHor * frontCheckDist, transform.position.y, transform.position.z);
        if (Physics.Raycast(frontCheckPosition, Vector3.right * movHor, frontCheckDist, groundLayer))
        {
            movHor *= -1; 
        }

        Vector3 velocity = new Vector3(movHor * speed, rb.velocity.y, rb.velocity.z);
        rb.velocity = velocity;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            new Vector3(transform.position.x, transform.position.y - floorCheckY, transform.position.z),
            new Vector3(transform.position.x, transform.position.y - floorCheckY - frontGrndRayDist, transform.position.z)
        );

        Gizmos.DrawLine(
            new Vector3(transform.position.x + movHor * frontCheckDist, transform.position.y, transform.position.z),
            new Vector3(transform.position.x + movHor * (frontCheckDist + frontGrndRayDist), transform.position.y, transform.position.z)
        );
    }
}
