using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Spawner _spawner;

    private float _scoreMultiplier = 10f;

    [SerializeField] private UIDocument _uiDocument;

    private Label _scoreText;
    private Label _highScoreText;
    private Button _restartButton;

    private float _elapsedTime = 0f;
    private float _score = 0f;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        _scoreText = _uiDocument.rootVisualElement.Q<Label>("ScoreLabel");

        _highScoreText = _uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");
        _highScoreText.style.display = DisplayStyle.None;

        _restartButton = _uiDocument.rootVisualElement.Q<Button>("RestartButton");
        _restartButton.clicked += ReloadScene;
        _restartButton.style.display = DisplayStyle.None;

        _spawner.SpawnObstacles();
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
        _score = Mathf.FloorToInt(_elapsedTime * _scoreMultiplier);

        _scoreText.text = "SCORE: " + _score;
    }

    public void GameOver()
    {
        ShowHighScore();

        _restartButton.style.display = DisplayStyle.Flex;
    }

    private void ShowHighScore()
    {
        if (_score > PlayerPrefs.GetFloat("HighScore", 0))
        {
            PlayerPrefs.SetFloat("HighScore", _score);
            PlayerPrefs.Save();
        }

        _highScoreText.text = "HIGH SCORE: " + PlayerPrefs.GetFloat("HighScore", 0);
        _highScoreText.style.display = DisplayStyle.Flex;
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SlowDown(float durationInSeconds)
    {
        StartCoroutine(SlowDownEffect(durationInSeconds));
    }

    private IEnumerator SlowDownEffect(float durationInSeconds)
    {
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(durationInSeconds);
        Time.timeScale = 1f;
    }

}
