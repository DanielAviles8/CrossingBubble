using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody rb;

    public float movHor = 1f; // Dirección inicial (1 = derecha, -1 = izquierda)
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
        // Verificar si hay suelo delante para evitar caer
        Vector3 floorCheckPosition = new Vector3(transform.position.x, transform.position.y - floorCheckY, transform.position.z);
        bool isGroundFloor = Physics.Raycast(floorCheckPosition, Vector3.down, frontGrndRayDist, groundLayer);

        if (!isGroundFloor)
        {
            movHor *= -1; // Cambiar de dirección si no hay suelo
        }

        // Verificar si hay una pared delante
        Vector3 frontCheckPosition = new Vector3(transform.position.x + movHor * frontCheckDist, transform.position.y, transform.position.z);
        if (Physics.Raycast(frontCheckPosition, Vector3.right * movHor, frontCheckDist, groundLayer))
        {
            movHor *= -1; // Cambiar de dirección si hay una pared
        }

        // Actualizar velocidad
        Vector3 velocity = new Vector3(movHor * speed, rb.velocity.y, rb.velocity.z);
        rb.velocity = velocity;
    }

    void OnDrawGizmos()
    {
        // Gizmos para visualizar los rayos
        Gizmos.color = Color.red;

        // Rayo para detectar suelo
        Gizmos.DrawLine(
            new Vector3(transform.position.x, transform.position.y - floorCheckY, transform.position.z),
            new Vector3(transform.position.x, transform.position.y - floorCheckY - frontGrndRayDist, transform.position.z)
        );

        // Rayo para detectar pared
        Gizmos.DrawLine(
            new Vector3(transform.position.x + movHor * frontCheckDist, transform.position.y, transform.position.z),
            new Vector3(transform.position.x + movHor * (frontCheckDist + frontGrndRayDist), transform.position.y, transform.position.z)
        );
    }
}
