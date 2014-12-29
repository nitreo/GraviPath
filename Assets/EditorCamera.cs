using UnityEngine;

public class EditorCamera : MonoBehaviour
{
    // Update is called once per frame
    private Camera m_CachedCamera;
    
    public float MinZoom;
    public float ZoomSpeed;
    public float ZoomFactor;
    public float CameraMoveSpeed;
    public float CameraMoveStep;
    private Vector3 _targetPosition;
    private float _targetSize;

    void Start()
    {
        m_CachedCamera = GetComponent<Camera>();
        _targetSize = m_CachedCamera.orthographicSize;
        _targetPosition = m_CachedCamera.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _targetSize += ZoomFactor;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _targetSize -= ZoomFactor;
        }
        if (Input.GetKey(KeyCode.W))
        {
            _targetPosition = new Vector3(_targetPosition.x,_targetPosition.y+CameraMoveStep*Time.deltaTime,_targetPosition.z);
        }
        if (Input.GetKey(KeyCode.S))
        {
            _targetPosition = new Vector3(_targetPosition.x, _targetPosition.y - CameraMoveStep * Time.deltaTime, _targetPosition.z);
        }
        if (Input.GetKey(KeyCode.A))
        {
            _targetPosition = new Vector3(_targetPosition.x - CameraMoveStep * Time.deltaTime, _targetPosition.y, _targetPosition.z);
        }
        if (Input.GetKey(KeyCode.D))
        {
            _targetPosition = new Vector3(_targetPosition.x + CameraMoveStep * Time.deltaTime, _targetPosition.y, _targetPosition.z);
        }
        m_CachedCamera.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, Mathf.Max(MinZoom,_targetSize), Time.deltaTime * ZoomSpeed);
        m_CachedCamera.transform.position = Vector3.Lerp(m_CachedCamera.transform.position, _targetPosition, Time.deltaTime * CameraMoveSpeed);

    }
}