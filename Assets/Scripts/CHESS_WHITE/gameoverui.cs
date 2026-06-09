using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject youWonPanel;

    [Header("Game Over UI")]
    public TMP_Text gameOverText;

    [Header("You Won UI")]
    public TMP_Text winnerText;
    public Image rickRollImage;

    [Header("Rick Roll Audio")]
    public AudioSource audioSource;
    public AudioClip rickRollSong;

    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu";

    void Awake()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (youWonPanel != null)
            youWonPanel.SetActive(false);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
            audioSource.ignoreListenerPause = true;
    }

    public void ShowGameOver()
    {
        Time.timeScale = 0f;

        if (gameOverText != null)
            gameOverText.text = "LOSER :)";

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void ShowYouWon()
    {
        Time.timeScale = 0f;

        if (winnerText != null)
            winnerText.text = GetWinnerMessage();

        if (youWonPanel != null)
            youWonPanel.SetActive(true);

        StopAllSceneAudio();

        if (audioSource != null && rickRollSong != null)
        {
            audioSource.ignoreListenerPause = true;
            audioSource.clip = rickRollSong;
            audioSource.loop = true;
            audioSource.spatialBlend = 0f;
            audioSource.Play();
        }
    }

    string GetWinnerMessage()
    {
        if (GameManager.Instance == null)
            return "CONGRATS!\nYOU'VE BEEN PROMOTED TO QUEEN.\nAND YOU GOT RICK ROLLED.";

        string winnerName;

        if (GameManager.Instance.witchRelics > GameManager.Instance.nunRelics)
            winnerName = "WITCH";
        else if (GameManager.Instance.nunRelics > GameManager.Instance.witchRelics)
            winnerName = "NUN";
        else
            winnerName = "BOTH PLAYERS";

        return "CONGRATS, " + winnerName + "!\n" +
               "YOU'VE BEEN PROMOTED TO QUEEN.\n" +
               "AND YOU GOT RICK ROLLED.";
    }

    void StopAllSceneAudio()
    {
        AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource source in sources)
        {
            if (source != audioSource)
                source.Stop();
        }
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;

        if (audioSource != null)
            audioSource.Stop();

        if (GameManager.Instance != null)
            GameManager.Instance.ResetGame();

        SceneManager.LoadScene(mainMenuSceneName);
    }
}