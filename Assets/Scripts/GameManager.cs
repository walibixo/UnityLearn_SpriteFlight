using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float _scoreMultiplier = 10f;

    [SerializeField] private UIDocument _uiDocument;
    private Label _scoreText;

    private float _elapsedTime = 0f;
    private float _score = 0f;

    void Start()
    {
        _scoreText = _uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
        _score = Mathf.FloorToInt(_elapsedTime * _scoreMultiplier);

        _scoreText.text = "SCORE: " + _score;
    }
}
