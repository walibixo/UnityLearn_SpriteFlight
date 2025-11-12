using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _thrustForce = 2f;
    [SerializeField] private float _maxSpeed = 5f;

    [SerializeField] private GameObject _boosterFlame;
    [SerializeField] private GameObject _explosionEffect;

    private Rigidbody2D _rigidbody2D;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
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
            _rigidbody2D.AddForce(direction * _thrustForce);

            if (_rigidbody2D.linearVelocity.magnitude > _maxSpeed)
            {
                _rigidbody2D.linearVelocity = _rigidbody2D.linearVelocity.normalized * _maxSpeed;
            }
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
        Instantiate(_explosionEffect, transform.position, transform.rotation);
        //Destroy(gameObject);
    }
}
