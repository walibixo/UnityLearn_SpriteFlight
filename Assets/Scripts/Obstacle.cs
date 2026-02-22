using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Obstacle : MonoBehaviour
{
    private readonly float _minSpeed = 50f;
    private readonly float _maxSpeed = 100f;

    private readonly float _maxSpinSpeed = 30f;

    [SerializeField] private GameObject _bounceEffectPrefab;

    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private PlayerController _playerController;

    public int Level { get; private set; }

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

    private void OnPlayerLevelChanged(int newPlayerLevel)
    {
        if (newPlayerLevel > Level)
        {
            _spriteRenderer.color = Color.red;
        }
    }

    public void Initialize(int level)
    {
        Level = level;
        OnPlayerLevelChanged(Mathf.FloorToInt(_playerController.Level));

        var size = level + 0.5f;
        transform.localScale = new Vector3(size, size, 1);

        var torque = Random.Range(-_maxSpinSpeed, _maxSpinSpeed);
        _rigidbody2D.AddTorque(torque);

        var impulse = Random.Range(_minSpeed, _maxSpeed) * (1f / size);
        var direction = Random.insideUnitCircle.normalized;
        _rigidbody2D.AddForce(direction * impulse);
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
}
