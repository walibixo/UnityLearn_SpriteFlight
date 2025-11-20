using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float _minSize = 1f;
    [SerializeField] private float _maxSize = 3f;

    [SerializeField] private float _minSpeed = 200f;
    [SerializeField] private float _maxSpeed = 500f;

    [SerializeField] private float _maxSpinSpeed = 30f;

    [SerializeField] private GameObject _bounceEffectPrefab;

    private Rigidbody2D _rigidbody2D;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        var size = Random.Range(_minSize, _maxSize);
        transform.localScale = new Vector3(size, size, 1);

        var torque = Random.Range(-_maxSpinSpeed, _maxSpinSpeed);
        _rigidbody2D.AddTorque(torque);

        var impulse = Random.Range(_minSpeed, _maxSpeed);
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

    private float GetSpeed(GameObject gameObject)
    {
        if (gameObject == null) return 0f;
        if (!gameObject.TryGetComponent<Rigidbody2D>(out var rb)) return 0f;
        return rb.linearVelocity.magnitude;
    }
}
