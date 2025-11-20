using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Obstacle _obstaclePrefab;

    void Start()
    {
        var camera = Camera.main;
        var screenBottomLeft = camera.ScreenToWorldPoint(new Vector2(0, 0));
        var screenTopRight = camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        for (int i = 0; i < 10; i++)
        {
            var spawnPosition = new Vector3(
                Random.Range(screenBottomLeft.x, screenTopRight.x),
                Random.Range(screenBottomLeft.y, screenTopRight.y),
                0);

            Instantiate(_obstaclePrefab, spawnPosition, Quaternion.identity, transform);
        }
    }
}
