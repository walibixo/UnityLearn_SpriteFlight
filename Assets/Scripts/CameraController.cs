using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private float _initialOrthographicSize;
    private float _zoomMultiplier = 2f;
    private float _zoomSpeed = 2f;

    private Camera _camera;
    private PlayerController _playerController;

    private float _targetOrthographicSize;

    private Transform _topWall;
    private Transform _bottomWall;
    private Transform _leftWall;
    private Transform _rightWall;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        _initialOrthographicSize = _camera.orthographicSize;
        _targetOrthographicSize = _initialOrthographicSize;

        _playerController = FindFirstObjectByType<PlayerController>();

        CreateBoundaryWalls();
    }

    void Start()
    {
        UpdateBoundaries();
    }

    void Update()
    {
        if (Mathf.Abs(_camera.orthographicSize - _targetOrthographicSize) > 0.01f)
        {
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetOrthographicSize, Time.deltaTime * _zoomSpeed);
            UpdateBoundaries();
        }
    }

    private void CreateBoundaryWalls()
    {
        _topWall = CreateWall("Top Wall");
        _bottomWall = CreateWall("Bottom Wall");
        _leftWall = CreateWall("Left Wall");
        _rightWall = CreateWall("Right Wall");
    }

    private Transform CreateWall(string wallName)
    {
        GameObject wall = new(wallName)
        {
            layer = LayerMask.NameToLayer("Default")
        };

        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;

        return wall.transform;
    }

    void OnEnable()
    {
        _playerController.OnPlayerLevelChanged += OnPlayerLevelChanged;
    }

    void OnDisable()
    {
        _playerController.OnPlayerLevelChanged -= OnPlayerLevelChanged;
    }

    private void OnPlayerLevelChanged(int newPlayerLevel)
    {
        _targetOrthographicSize = _initialOrthographicSize + (newPlayerLevel - 1) * _zoomMultiplier;
    }

    private void UpdateBoundaries()
    {
        float cameraHeight = _camera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * _camera.aspect;

        float halfWidth = cameraWidth / 2f;
        float halfHeight = cameraHeight / 2f;

        // Position and scale walls at the edges of the camera view
        _topWall.position = new Vector3(0, halfHeight, 0);
        _topWall.localScale = new Vector3(cameraWidth, 1, 1);

        _bottomWall.position = new Vector3(0, -halfHeight, 0);
        _bottomWall.localScale = new Vector3(cameraWidth, 1, 1);

        _leftWall.position = new Vector3(-halfWidth, 0, 0);
        _leftWall.localScale = new Vector3(1, cameraHeight, 1);

        _rightWall.position = new Vector3(halfWidth, 0, 0);
        _rightWall.localScale = new Vector3(1, cameraHeight, 1);
    }
}
