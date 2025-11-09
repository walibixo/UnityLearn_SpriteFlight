using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float _minSize = 1f;
    [SerializeField] private float _maxSize = 3f;

    [SerializeField] private float _minSpeed = 500f;
    [SerializeField] private float _maxSpeed = 2000f;

    [SerializeField] private float _maxSpinSpeed = 30f;

    private Rigidbody2D _rigidbody2D;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        var size = Random.Range(_minSize, _maxSize);
        transform.localScale = new Vector3(size, size, 1);

        var torque = Random.Range(-_maxSpinSpeed, _maxSpinSpeed);
        _rigidbody2D.AddTorque(torque);

        var impulse = Random.Range(_minSpeed, _maxSpeed) / size;
        var direction = Random.insideUnitCircle.normalized;
        _rigidbody2D.AddForce(direction * impulse);
    }

    void Update()
    {

    }
}
