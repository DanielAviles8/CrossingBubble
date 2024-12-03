using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private InputActionsHolder inputActionsHolder;

    [SerializeField] private Transform _targetTransform;
    [SerializeField] private float _cameraSize;
    [SerializeField] private float _lerpSpeed = 1f;
    [SerializeField] private float _targetDistance = -3f;

    [SerializeField] private float _changePosY = -5f;
    private bool lookingDown;

    [SerializeField] private Vector3 _targetPosition;

    private GameInputActions _inputActions;
    
    void Start()
    {
        Prepare();
    }
    private void OnDestroy()
    {
        //_inputActions.Player.MoveCameraDown.performed -= MoveCameraDown;
        _inputActions.Player.MoveCameraDown.canceled -= StopCameraMove;
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log("Posicion objetivo Y "+_targetPosition.y);
        Debug.Log("Posicion actual Y " + _targetTransform.position.y);
        var axis = _inputActions.Player.MoveCameraDown.ReadValue<float>();
        var dis = new Vector3(0f, 0f, _targetDistance) + _targetPosition;

        lookingDown = axis > .80f;
        Debug.Log("Bool mirando abajo " + lookingDown);
        if (lookingDown)
        {
            var yDelta = Mathf.Abs(_targetPosition.y - _targetTransform.position.y);
            
            if (yDelta <= _changePosY * -1f)
            {
                _targetPosition += new Vector3 (0, -5, 0);
            }
        }
        else
        {
            _targetPosition = _targetTransform.position;
        }

        transform.position = Vector3.Lerp(this.transform.position, dis, _lerpSpeed * Time.deltaTime);
    }
    private void Prepare()
    {
        _inputActions = inputActionsHolder._GameInputActions;
        //_inputActions.Player.MoveCameraDown.performed += MoveCameraDown;
        _inputActions.Player.MoveCameraDown.canceled += StopCameraMove;
    }
    private void MoveCameraDown(InputAction.CallbackContext ctx)
    {
        StartCoroutine(CorCameraDown());
    }
    private void StopCameraMove(InputAction.CallbackContext ctx)
    {
        Debug.Log("Canceled");
        StopCoroutine(nameof (CorCameraDown));
        lookingDown = false;
    }
    private IEnumerator CorCameraDown()
    {
        yield return new WaitForSeconds(1f);
        lookingDown = true;
    }
}
