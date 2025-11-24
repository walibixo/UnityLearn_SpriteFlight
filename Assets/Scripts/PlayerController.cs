using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float _thrustForce = 3f;
    private float _maxSpeed = 6f;

    [SerializeField] private GameObject _boosterFlame;
    [SerializeField] private GameObject _explosionEffect;

    private Rigidbody2D _rigidbody2D;
    private TrailRenderer _trailRenderer;

    void Start()
    {
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
        if (!collision.gameObject.TryGetComponent<Obstacle>(out var obstacle))
        {
            return;
        }

        var obstacleSize = GetSize(collision.gameObject);
        var playerSize = GetSize(this.gameObject);

        if (playerSize == obstacleSize)
        {
            return;
        }

        if (playerSize > obstacleSize)
        {
            obstacle.Destroy();
            Grow();

            return;
        }

        Instantiate(_explosionEffect, transform.position, transform.rotation);
        GameManager.Instance.GameOver();

        Destroy(gameObject);
    }

    private float GetSize(GameObject gameObject)
    {
        return gameObject.transform.localScale.x;
    }

    private void Grow()
    {
        gameObject.transform.localScale += Vector3.one * (0.5f);
        if (_trailRenderer != null)
        {
            _trailRenderer.widthMultiplier += 0.5f;
        }

        GameManager.Instance.SlowDown(1f);
    }
}
