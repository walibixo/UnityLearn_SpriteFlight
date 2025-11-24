using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private float _minSize = 0.1f;
    private float _maxSize = 3f;

    private float _minSpeed = 50f;
    private float _maxSpeed = 100f;

    private float _maxSpinSpeed = 30f;

    [SerializeField] private GameObject _bounceEffectPrefab;

    private Rigidbody2D _rigidbody2D;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        float size = Mathf.Round(Random.Range(_minSize, _maxSize) * 2f) / 2f;
        transform.localScale = new Vector3(size, size, 1);

        var torque = Random.Range(-_maxSpinSpeed, _maxSpinSpeed);
        _rigidbody2D.AddTorque(torque);

        var impulse = Random.Range(_minSpeed, _maxSpeed) * (1 / size) ;
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
        Destroy(gameObject);
    }

    private float GetSpeed(GameObject gameObject)
    {
        if (gameObject == null) return 0f;
        if (!gameObject.TryGetComponent<Rigidbody2D>(out var rb)) return 0f;
        return rb.linearVelocity.magnitude;
    }
}
