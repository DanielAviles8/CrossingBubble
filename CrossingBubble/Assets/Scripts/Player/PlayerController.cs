using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionsHolder inputActionsHolder;

    [SerializeField] private float _moveSpeed = 15f;
    [SerializeField] private float _gravity = 9.8f;  

    private GameInputActions _inputActions;
    private CharacterController _characterController;

    public float jumpForce = 15.0f;

    private float verticalSpeed;  
    private Vector2 _inputVector;

    private void OnDestroy()
    {
        _inputActions.Player.Jump.performed -= JumpPlayer;
    }

    void Start()
    {
        Prepare();
    }

    void Update()
    {
        _inputVector = _inputActions.Player.Move.ReadValue<Vector2>();
        MovePlayer();
    }

    private void Prepare()
    {
        _characterController = GetComponent<CharacterController>();
        _inputActions = inputActionsHolder._GameInputActions;
        _inputActions.Player.Jump.performed += JumpPlayer;
    }

    private void MovePlayer()
    {
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
        if (_characterController.isGrounded)
        {
            verticalSpeed = jumpForce;
        }
    }
}
