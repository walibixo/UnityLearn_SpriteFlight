using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Obstacle _obstaclePrefab;
    private Vector2 _screenBottomLeft;
    private Vector2 _screenTopRight;

    private readonly List<Obstacle> _activeObstacles = new();

    void Start()
    {
        var camera = Camera.main;
        _screenBottomLeft = camera.ScreenToWorldPoint(new Vector2(0, 0));
        _screenTopRight = camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }

    public void SpawnObstacles()
    {
        SpawnObstaclesWithSize(4, 0);
        SpawnObstaclesWithSize(3, 1);
        SpawnObstaclesWithSize(3, 2);
    }

    private void SpawnObstaclesWithSize(int count, int level)
    {
        for (int i = 0; i < count; i++)
        {
            var spawnPosition = new Vector3(
                Random.Range(_screenBottomLeft.x, _screenTopRight.x),
                Random.Range(_screenBottomLeft.y, _screenTopRight.y),
                0);

            var obstacle = Instantiate(_obstaclePrefab, spawnPosition, Quaternion.identity, transform);
            obstacle.Initialize(level);
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

    public void RemoveObstacle(Obstacle obstacle)
    {
        if (_activeObstacles.Contains(obstacle))
        {
            _activeObstacles.Remove(obstacle);
        }
    }
}
