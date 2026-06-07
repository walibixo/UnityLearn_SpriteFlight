using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Obstacle : MonoBehaviour
{
    private readonly float _minSpeed = 50f;
    private readonly float _maxSpeed = 100f;

    private readonly float _maxSpinSpeed = 30f;

    private readonly float _repulsionRadius = 4f;
    private readonly float _repulsionForce = 2f;

    [SerializeField] private GameObject _bounceEffectPrefab;

    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private PlayerController _playerController;

    public int Level { get; private set; }

    private float Size => Level + 1f;

    public event System.Action<Obstacle> OnDestroyed;

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerController = FindFirstObjectByType<PlayerController>();
    }

    void OnEnable()
    {
        _playerController.OnPlayerLevelChanged += OnPlayerLevelChanged;
    }

    void OnDisable()
    {
        _playerController.OnPlayerLevelChanged -= OnPlayerLevelChanged;
    }

    public void Initialize(int level)
    {
        Level = level;
        OnPlayerLevelChanged(Mathf.FloorToInt(_playerController.Level));

        transform.localScale = new Vector3(Size, Size, 1);

        var torque = Random.Range(-_maxSpinSpeed, _maxSpinSpeed);
        _rigidbody2D.AddTorque(torque);

        var impulse = ScaleToCurrentSize(Random.Range(_minSpeed, _maxSpeed));
        var direction = Random.insideUnitCircle.normalized;
        _rigidbody2D.AddForce(direction * impulse);
    }

    void FixedUpdate()
    {
        _rigidbody2D.linearVelocity = Vector3.ClampMagnitude(_rigidbody2D.linearVelocity, _maxSpeed);
    }

    void Update()
    {
        FleePlayer();
    }

    private void FleePlayer()
    {
        if (_playerController == null) return;

        int playerLevel = Mathf.FloorToInt(_playerController.Level);

        // Only apply repulsion if player level is higher than obstacle level
        if (playerLevel <= Level) return;

        float playerDistance = Vector2.Distance(transform.position, _playerController.transform.position);

        var minRadius = ScaleToCurrentSize(0.1f);
        var maxRadius = ScaleToCurrentSize(_repulsionRadius);
        var repulsionForce = ScaleToCurrentSize(_repulsionForce);

        // If within repulsion radius, push away from player
        if (maxRadius > playerDistance && playerDistance > minRadius)
        {
            Vector2 directionAwayFromPlayer = (transform.position - _playerController.transform.position).normalized;
            float repulsionStrength = repulsionForce * (1f - playerDistance / maxRadius);
            _rigidbody2D.AddForce(directionAwayFromPlayer * repulsionStrength);
        }
    }

    private void OnPlayerLevelChanged(int newPlayerLevel)
    {
        if (newPlayerLevel > Level)
        {
            _spriteRenderer.color = Color.red;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (GetSpeed(gameObject) < GetSpeed(collision.gameObject))
        {
            return;
        }

        Vector2 contactPoint = collision.GetContact(0).point;
        GameObject bounceEffect = Instantiate(_bounceEffectPrefab, contactPoint, Quaternion.identity);
        bounceEffect.transform.localScale = Vector3.one * (GetSpeed(gameObject) / 2f);

        // Destroy the effect after 1 second
        Destroy(bounceEffect, 1f);
    }

    public void Destroy()
    {
        OnDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    private float GetSpeed(GameObject gameObject)
    {
        if (gameObject == null) return 0f;
        if (!gameObject.TryGetComponent<Rigidbody2D>(out var rb)) return 0f;
        return rb.linearVelocity.magnitude;
    }

    private float ScaleToCurrentSize(float baseValue)
    {
        return baseValue * Size;
    }
}
