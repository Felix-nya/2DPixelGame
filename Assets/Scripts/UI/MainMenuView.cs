using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button howToPlayButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private Button closeHowToPlayButton;

    private void Start()
    {
        playButton.onClick.AddListener(() => SceneManager.LoadScene(SceneNames.Game));
        howToPlayButton.onClick.AddListener(() => howToPlayPanel.SetActive(true));
        closeHowToPlayButton.onClick.AddListener(() => howToPlayPanel.SetActive(false));
        quitButton.onClick.AddListener(Quit);

        howToPlayPanel.SetActive(false);
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // в редакторе останавливаем Play
#else
        Application.Quit();
#endif
    }
}