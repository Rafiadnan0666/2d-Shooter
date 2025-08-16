using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public int Score { get; set; }
    public Text ScoreText;
    public PlayerScript Player;
    public Text HealthText;
    public Text GOText;

    public GameObject GOCanvas;
    public GameObject PauseCanvas;
    public GameObject MainCanvas;
    public GameObject MenuCanvas;
    private bool isPaused = false;

    private void Start()
    {
        Score = 0;
        GOCanvas.SetActive(false);
        PauseCanvas.SetActive(false);
        MainCanvas.SetActive(false);
        MenuCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    void Update()
    {
        ScoreText.text = "Score: " + Score.ToString();
        HealthText.text = "Health: " + Player.GetCurrentHealth().ToString();

        if (Player.GetCurrentHealth() <= 0)
        {
            GameOver();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over! Final Score: " + Score);
        GOText.text = "Game Over! Final Score: " + Score.ToString();
        Time.timeScale = 0f;
        GOCanvas.SetActive(true);
        MainCanvas.SetActive(false);
    }

    public void StartGame()
    {
        Score = 0;
        Player.ResetHealth(); 
        GOCanvas.SetActive(false);
        PauseCanvas.SetActive(false);
        MainCanvas.SetActive(true);
        MenuCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Score = 0;
        Player.ResetHealth(); 
        GOCanvas.SetActive(false);
        PauseCanvas.SetActive(false);
        MainCanvas.SetActive(true);
        MenuCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        PauseCanvas.SetActive(false);
        //MainCanvas.SetActive(true);
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            PauseCanvas.SetActive(true);
            MainCanvas.SetActive(false);
            MenuCanvas.SetActive(false);
        }
        else
        {
            Time.timeScale = 1f;
            PauseCanvas.SetActive(false);
            MainCanvas.SetActive(true);
            MenuCanvas.SetActive(false);
        }
    }
}