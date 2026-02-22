using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Obstacle _obstaclePrefab;

    private readonly List<Obstacle> _activeObstacles = new();

    private PlayerController _playerController;
    private Camera _camera;

    void Awake()
    {
        _playerController = FindFirstObjectByType<PlayerController>();
        _camera = Camera.main;
    }

    private Vector2 GetScreenBottomLeft()
    {
        return _camera.ScreenToWorldPoint(new Vector2(0, 0));
    }

    private Vector2 GetScreenTopRight()
    {
        return _camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }

    void Update()
    {
        if (_activeObstacles.Count > 0) return;

        SpawnObstacles();
    }

    public void SpawnObstacles()
    {
        var playerLevel = Mathf.FloorToInt(_playerController.Level);

        SpawnObstaclesWithSize(4, playerLevel - 1);
        SpawnObstaclesWithSize(3, playerLevel);
        SpawnObstaclesWithSize(3, playerLevel + 1);
    }

    private void SpawnObstaclesWithSize(int count, int level)
    {
        Vector2 screenBottomLeft = GetScreenBottomLeft();
        Vector2 screenTopRight = GetScreenTopRight();

        for (int i = 0; i < count; i++)
        {
            var spawnPosition = new Vector3(
                Random.Range(screenBottomLeft.x, screenTopRight.x),
                Random.Range(screenBottomLeft.y, screenTopRight.y),
                0);

            var obstacle = Instantiate(_obstaclePrefab, spawnPosition, Quaternion.identity, transform);
            obstacle.Initialize(level);
            obstacle.OnDestroyed += RemoveObstacle;
            _activeObstacles.Add(obstacle);
        }
    }

    public void ClearObstacles()
    {
        foreach (var obstacle in _activeObstacles)
        {
            if (obstacle != null)
            {
                obstacle.Destroy();
            }
        }
        _activeObstacles.Clear();
    }

    public void RespawnObstacles()
    {
        ClearObstacles();
        SpawnObstacles();
    }

    private void RemoveObstacle(Obstacle obstacle)
    {
        obstacle.OnDestroyed -= RemoveObstacle;
        _activeObstacles.Remove(obstacle);
    }
}
