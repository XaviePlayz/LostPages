using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioVolumeController : MonoBehaviour
{
    [Header("Volume")]
    public Slider volumeSlider;
    private const string VolumeKey = "Volume";

    [Header("Scenes")]
    public GameObject pauseMenu;
    public GameObject mainMenu, options;
    float gameVolume;

    private void Start()
    {
        // Load Game
        options.SetActive(false);

        // Load the saved volume value
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.05f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);

        // Attach a listener to the slider's OnValueChanged event
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    public void OpenSettings()
    {
        pauseMenu.SetActive(false);
        options.SetActive(true);
        SetVolume(gameVolume);

        // Load the saved volume value
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
    }

    public void ConfirmSettings()
    {
        pauseMenu.SetActive(true);
        options.SetActive(false);
        SetVolume(gameVolume);
    }

    public void OpenSettingsInMainMenu()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
        SetVolume(gameVolume);

        // Load the saved volume value
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
    }

    public void ConfirmSettingsInMainMenu()
    {
        mainMenu.SetActive(true);
        options.SetActive(false);
        SetVolume(gameVolume);
    }

    public void OnVolumeChanged(float volume)
    {
        // Set the volume for all audio sources
        SetVolume(volume);
        gameVolume = volume;

        // Save the volume value
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
    }

    public void SetVolume(float volume)
    {
        // Find all instances of AudioSource in the scene
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        // Set the volume for all audio sources
        foreach (var audioSource in audioSources)
        {
            audioSource.volume = volume;
        }
    }

    public void GoToMainMenu()
    {
        // Load the MainMenu
        SceneManager.LoadScene(0);
    }
    public void RetryButton()
    {
        SceneManager.LoadScene(1);
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}