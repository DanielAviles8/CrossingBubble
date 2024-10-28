using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionsHolder inputActionsHolder;
    
    private GameInputActions _inputActions;
    private CharacterController _characterController;

    private Vector2 _inputVector;
    private void OnDestroy()
    {
        _inputActions.Player.Jump.performed -= JumpPlayer;
    }
    // Start is called before the first frame update
    void Start()
    {
        Prepare();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        _inputVector = _inputActions.Player.Move.ReadValue<Vector2>();
    }

    private void Prepare()
    {
        _characterController = GetComponent<CharacterController>();
        _inputActions = inputActionsHolder._GameInputActions;
        _inputActions.Player.Jump.performed += JumpPlayer;
    }
    
    private void MovePlayer()
    {

    }

    private void JumpPlayer(InputAction.CallbackContext ctx)
    {

    }
}
