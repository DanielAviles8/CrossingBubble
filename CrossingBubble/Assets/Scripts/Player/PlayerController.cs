using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{

    [SerializeField] private InputActionsHolder inputActionsHolder;

    [SerializeField] private float _moveSpeed = 15f;
    [SerializeField] private float _gravity = 9.8f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float climbSpeed = 5f; 
    [SerializeField] private float wallDetectionDistance = 0.5f; 
    [SerializeField] private float grappleRange = 10f; 
    [SerializeField] private float grappleSpeed = 10f;
    [SerializeField] private LineRenderer grappleLine; // Referencia al LineRenderer
    [SerializeField] private Material grappleMaterial; // Material del grapple
    [SerializeField] private float grappleLineWidth = 0.1f; // Ancho de la línea


    private GameInputActions _inputActions;
    private CharacterController _characterController;

    public float jumpForce = 15.0f;
    private float verticalSpeed;
    private Vector2 _inputVector;
    private Vector2 dashDirection;
    private bool isDashing = false;
    private bool isTouchingWall = false;
    private bool isClimbing = false; 
    private float dashTimeRemaining = 0f;
    private float cooldownTimeRemaining = 0f;

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask anchorPointLayer; 


    private CharacterController controller;
    private Vector3 grappleTarget;
    private bool isGrappling = false;
    private Transform lastAnchorPoint;
    private float grappleTimeRemaining; // Tiempo restante para el grapple
    [SerializeField] private float grappleMaxDuration = 2f; // Duración máxima del grapple


    private void OnDestroy()
    {
        _inputActions.Player.Jump.performed -= JumpPlayer;
        _inputActions.Player.Dash.performed -= DashPlayer;
        _inputActions.Player.GrabWall.performed -= GrabWall;
        _inputActions.Player.Grapple.performed -= TryGrapple;
    }

    void Start()
    {
        Prepare();
        // Configurar el LineRenderer si no se ha hecho manualmente
        if (grappleLine == null)
        {
            GameObject lineObject = new GameObject("GrappleLine");
            grappleLine = lineObject.AddComponent<LineRenderer>();
            grappleLine.material = grappleMaterial; // Asignar material
            grappleLine.startWidth = grappleLineWidth;
            grappleLine.endWidth = grappleLineWidth;
            grappleLine.positionCount = 2; // La línea tendrá dos puntos
            grappleLine.enabled = false;  // Inicialmente deshabilitado
        }
    }

    void Update()
    {
        if(isClimbing == true)
        {
            _inputVector = _inputActions.Player.ClimbWall.ReadValue<Vector2>();
        }
        else
        {
            _inputVector = _inputActions.Player.Move.ReadValue<Vector2>();
        }

        if (isDashing)
        {
            PerformDash();
        }
        else
        {
            MovePlayer();
            ClimbWall();
            if (cooldownTimeRemaining > 0)
            {
                cooldownTimeRemaining -= Time.deltaTime;
            }
        }
        if (isGrappling)
        {
            MoveTowardsGrappleTarget();
        }
    }

    private void Prepare()
    {
        _characterController = GetComponent<CharacterController>();
        _inputActions = inputActionsHolder._GameInputActions;
        _inputActions.Player.Jump.performed += JumpPlayer;
        _inputActions.Player.Dash.performed += DashPlayer;
        _inputActions.Player.GrabWall.performed += GrabWall;
        _inputActions.Player.Grapple.performed += TryGrapple;
    }

    private void MovePlayer()
    {
        if (isClimbing)
        {
            Vector3 climbMovement = new Vector3(0, _inputVector.y * climbSpeed, 0);
            _characterController.Move(climbMovement * Time.deltaTime);

            if (_inputVector.y == 0 && !_inputActions.Player.GrabWall.ReadValue<float>().Equals(1))
            {
                ReleaseWall();
            }

            return;
        }

        // Dirección del movimiento basada en la entrada
        Vector3 dir = new Vector3(_inputVector.x, 0, _inputVector.y);
        Vector3 move = transform.TransformDirection(dir) * _moveSpeed;

        if (_characterController.isGrounded)
        {
            if (verticalSpeed < 0)
            {
                verticalSpeed = -0.5f;
            }

            // Permite volver al último punto de anclaje
            if (lastAnchorPoint != null)
            {
                Debug.Log("Jugador grounded. Puedes volver a usar el último punto de anclaje.");
                lastAnchorPoint = null; // Esto elimina la restricción
            }
        }
        else
        {
            verticalSpeed -= _gravity * Time.deltaTime;
        }

        // Combina el movimiento en los ejes XZ con la velocidad vertical
        move.y = verticalSpeed;
        _characterController.Move(move * Time.deltaTime);
    }
    private void JumpPlayer(InputAction.CallbackContext ctx)
    {
        if (isClimbing)
        {
            ReleaseWall(); 
            verticalSpeed = jumpForce; 
        }
        else if (_characterController.isGrounded)
        {
            verticalSpeed = jumpForce;
        }
    }
    private void DashPlayer(InputAction.CallbackContext ctx)
    {
        if (isClimbing) return;
        if (cooldownTimeRemaining > 0) return;

        dashDirection = _inputVector.normalized;
        if (dashDirection == Vector2.zero) return;

        isDashing = true;
        dashTimeRemaining = dashDistance / dashSpeed;
        cooldownTimeRemaining = dashCooldown;
    }

    private void PerformDash()
    {
        if (dashTimeRemaining > 0)
        {
            Vector3 movement = new Vector3(dashDirection.x, 0, dashDirection.y) * dashSpeed * Time.deltaTime;
            _characterController.Move(movement);
            dashTimeRemaining -= Time.deltaTime;
        }
        else
        {
            isDashing = false;
        }
    }

    private void GrabWall(InputAction.CallbackContext ctx)
    {
        Vector3 direction = _inputVector.x > 0 ? transform.right : -transform.right;

        Vector3 rayOrigin = transform.position;

        if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, wallDetectionDistance, wallLayer))
        {
            isTouchingWall = true;
            isClimbing = true;
            verticalSpeed = 0; 
        }
        else
        {
            isTouchingWall = false;
            isClimbing = false;
            Debug.Log("No se detectó pared");
        }

        Debug.DrawRay(rayOrigin, direction * wallDetectionDistance, Color.red, 1.0f);
    }

    private void ReleaseWall()
    {
        isClimbing = false; 
        isTouchingWall = false;
        Debug.Log("Liberado de la pared");
    }

    private void ClimbWall()
    {
        if (!isClimbing) return; 

        float verticalInput = _inputVector.y;

        Vector3 climbMovement = new Vector3(0, verticalInput * climbSpeed, 0);
        _characterController.Move(climbMovement * Time.deltaTime);

        if (Mathf.Abs(verticalInput) < 0.1f)
        {
            verticalSpeed = 0; 
        }

        Debug.Log($"Escalando la pared: {verticalInput}");
    }

    void TryGrapple(InputAction.CallbackContext ctx)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, grappleRange, anchorPointLayer);
        if (hitColliders.Length > 0)
        {
            Transform closestPoint = hitColliders[0].transform;
            float closestDistance = Vector3.Distance(transform.position, closestPoint.position);

            foreach (Collider2D hit in hitColliders)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestPoint = hit.transform;
                    closestDistance = distance;
                }
            }

            if (lastAnchorPoint != closestPoint || _characterController.isGrounded)
            {
                grappleTarget = closestPoint.position;
                isGrappling = true;
                grappleTimeRemaining = grappleMaxDuration;
                lastAnchorPoint = closestPoint;

                // Habilita la línea visual
                grappleLine.enabled = true;
                UpdateGrappleLine(transform.position, grappleTarget);
            }
            else
            {
                Debug.Log("No puedes usar el gancho en el mismo punto sin estar grounded.");
            }
        }
        else
        {
            Debug.Log("No hay puntos de anclaje en el rango.");
        }
    }

    void MoveTowardsGrappleTarget()
    {
        if (grappleTimeRemaining <= 0)
        {
            EndGrapple();
            return;
        }

        grappleTimeRemaining -= Time.deltaTime;

        Vector3 direction = (grappleTarget - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, grappleTarget);

        if (distance > 0.1f)
        {
            Vector3 move = direction * grappleSpeed * Time.deltaTime;
            _characterController.Move(move);

            // Actualiza la línea visual
            UpdateGrappleLine(transform.position, grappleTarget);
        }
        else
        {
            EndGrapple();
        }
    }
    void EndGrapple()
    {
        isGrappling = false;
        grappleLine.enabled = false; // Deshabilita la línea
        Debug.Log("Grapple finalizado.");
    }

    void UpdateGrappleLine(Vector3 start, Vector3 end)
    {
        grappleLine.SetPosition(0, start); // Punto inicial (jugador)
        grappleLine.SetPosition(1, end);   // Punto final (anclaje)
    }
    void OnDrawGizmos()
    {
        // Dibuja el rango del gancho para facilitar el diseño
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, grappleRange);
    }
}
