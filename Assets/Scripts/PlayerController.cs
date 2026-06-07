using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TrailRenderer))]
public class PlayerController : MonoBehaviour
{
    private readonly float _thrustForce = 3f;
    private readonly float _maxSpeed = 6f;

    private readonly float _growthFactor = 0.5f;

    [SerializeField] private GameObject _boosterFlame;
    [SerializeField] private GameObject _explosionEffect;

    private Rigidbody2D _rigidbody2D;
    private TrailRenderer _trailRenderer;

    public float Level { get; private set; }

    public event System.Action<int> OnPlayerLevelChanged;

    void Start()
    {
        Level = 1f;

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            // Calculate mouse direction
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            Vector2 direction = (mousePos - transform.position).normalized;

            // Move player in direction of mouse
            transform.up = direction;

            _rigidbody2D.AddForce(direction * ScaleToCurrentLevel(_thrustForce));

            _rigidbody2D.linearVelocity = Vector3.ClampMagnitude(_rigidbody2D.linearVelocity, ScaleToCurrentLevel(_maxSpeed));
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _boosterFlame.SetActive(true);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _boosterFlame.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Obstacle>(out var obstacle))
        {
            OnObstacleCollision(obstacle);
        }
    }

    private void OnObstacleCollision(Obstacle obstacle)
    {
        var obstacleLevel = obstacle.Level;
        var playerLevel = Mathf.FloorToInt(Level);

        if (playerLevel == obstacleLevel)
        {
            return;
        }
        else if (playerLevel > obstacleLevel)
        {
            obstacle.Destroy();

            var growth = _growthFactor / (playerLevel - obstacleLevel);

            Grow(growth);

            return;
        }
        else
        {
            Instantiate(_explosionEffect, transform.position, transform.rotation);
            GameManager.Instance.GameOver();

            Destroy(gameObject);
        }
    }

    private void Grow(float growth)
    {
        LevelUp(growth);

        gameObject.transform.localScale += Vector3.one * (growth);
        if (_trailRenderer != null)
        {
            _trailRenderer.widthMultiplier += growth;
        }
    }

    private void LevelUp(float growth)
    {
        int previousLevel = Mathf.FloorToInt(Level);
        Level += growth;
        int newLevel = Mathf.FloorToInt(Level);

        if (newLevel <= previousLevel) return;

        OnPlayerLevelChanged?.Invoke(newLevel);

        GameManager.Instance.SlowDown(1f);
    }

    private float ScaleToCurrentLevel(float baseValue)
    {
        int level = Mathf.FloorToInt(Level);
        return baseValue * level;
    }
}
