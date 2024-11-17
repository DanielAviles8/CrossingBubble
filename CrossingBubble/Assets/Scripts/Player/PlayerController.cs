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

    private void OnDestroy()
    {
        _inputActions.Player.Jump.performed -= JumpPlayer;
        _inputActions.Player.Dash.performed -= DashPlayer;
        _inputActions.Player.GrabWall.performed -= GrabWall;
    }

    void Start()
    {
        Prepare();
    }

    void Update()
    {
        
        Debug.Log(cooldownTimeRemaining);
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
        /*switch (state)
        {
            case PlayerStates.Moving:
                MovePlayer();
                break;
            case PlayerStates.Dashing:
                PerformDash();
                if (cooldownTimeRemaining > 0)
                {
                    cooldownTimeRemaining -= Time.deltaTime;
                }
                break;
            case PlayerStates.Jumping:
                PerformJump();
                break;
        }*/
    }

    private void Prepare()
    {
        _characterController = GetComponent<CharacterController>();
        _inputActions = inputActionsHolder._GameInputActions;
        _inputActions.Player.Jump.performed += JumpPlayer;
        _inputActions.Player.Dash.performed += DashPlayer;
        _inputActions.Player.GrabWall.performed += GrabWall;
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

        var dir = new Vector3(_inputVector.x, 0, _inputVector.y);
        Vector3 move = transform.TransformDirection(dir) * _moveSpeed;

        if (_characterController.isGrounded)
        {
            if (verticalSpeed < 0)
            {
                verticalSpeed = -0.5f;
            }
        }
        else
        {
            verticalSpeed -= _gravity * Time.deltaTime;
        }

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
        if (!isClimbing) return; // Solo permitir escalada si está agarrado a una pared

        float verticalInput = _inputVector.y;

        // Movimiento vertical para escalar
        Vector3 climbMovement = new Vector3(0, verticalInput * climbSpeed, 0);
        _characterController.Move(climbMovement * Time.deltaTime);

        // Si no hay input vertical, el jugador se queda en su lugar
        if (Mathf.Abs(verticalInput) < 0.1f)
        {
            verticalSpeed = 0; // Detener cualquier movimiento vertical
        }

        Debug.Log($"Escalando la pared: {verticalInput}");
    }
}
