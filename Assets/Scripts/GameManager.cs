using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float _scoreMultiplier = 10f;

    [SerializeField] private UIDocument _uiDocument;
    private Label _scoreText;
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

        _restartButton = _uiDocument.rootVisualElement.Q<Button>("RestartButton");
        _restartButton.clicked += ReloadScene;
        _restartButton.style.display = DisplayStyle.None;
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
        _score = Mathf.FloorToInt(_elapsedTime * _scoreMultiplier);

        _scoreText.text = "SCORE: " + _score;
    }

    public void GameOver()
    {
        Debug.Log("Game Over! Final Score: " + _score);
        _restartButton.style.display = DisplayStyle.Flex;
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
