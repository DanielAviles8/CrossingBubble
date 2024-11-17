using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private float _cameraSize;
    [SerializeField] private float _lerpSpeed = 1f;
    [SerializeField] private float _targetDistance = -3f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var dis = new Vector3(0f, 0f, _targetDistance) + _targetTransform.position;
        transform.position = Vector3.Lerp(this.transform.position, dis, _lerpSpeed * Time.deltaTime);
    }
}
