using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AudioVolumeController : MonoBehaviour
{
    #region Singleton

    private static AudioVolumeController _instance;
    public static AudioVolumeController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioVolumeController>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(AudioVolumeController).Name;
                    _instance = obj.AddComponent<AudioVolumeController>();
                }
            }
            return _instance;
        }
    }

    #endregion

    [Header("Music Volume")]
    public UnityEngine.UI.Slider musicVolumeSlider;
    private const string musicVolumeKey = "Music Volume";
    public float musicGameVolume = 0.5f;

    [Header("SFX Volume")]
    public UnityEngine.UI.Slider sfxVolumeSlider;
    private const string sfxVolumeKey = "SFX Volume";
    public float sfxGameVolume = 0.5f;

    [Header("Scenes")]
    public GameObject mainMenu;
    public GameObject options;
    public GameObject areYouSureMenu;
    public string url;

    private void Start()
    {
        // Load the saved volume value
        float savedMusicVolume = PlayerPrefs.GetFloat(musicVolumeKey, 0.05f);
        musicVolumeSlider.value = savedMusicVolume;
        SetMusicVolume(savedMusicVolume);

        musicGameVolume = savedMusicVolume;
        musicGameVolume = musicVolumeSlider.value;

        // Load the saved volume value
        float savedSFXVolume = PlayerPrefs.GetFloat(sfxVolumeKey, 0.05f);
        sfxVolumeSlider.value = savedSFXVolume;
        SetSFXVolume(savedSFXVolume);

        sfxGameVolume = savedSFXVolume;
        sfxVolumeSlider.value = sfxGameVolume;
        // Load Game
        if (options != null)
        {
            options.SetActive(false);
        }

        if (areYouSureMenu != null)
        {
            areYouSureMenu.SetActive(false);
        }

        // Attach a listener to the slider's OnValueChanged event
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    public void OpenSettings()
    {
        SetMusicVolume(musicGameVolume);

        // Load the saved volume value
        float savedMusicVolume = PlayerPrefs.GetFloat(musicVolumeKey, 1f);
        musicVolumeSlider.value = savedMusicVolume;
        SetMusicVolume(savedMusicVolume);

        float savedSFXVolume = PlayerPrefs.GetFloat(sfxVolumeKey, 1f);
        sfxVolumeSlider.value = savedSFXVolume;
        SetSFXVolume(savedSFXVolume);
    }

    public void ConfirmSettings()
    {
        SetMusicVolume(musicGameVolume);
        SetSFXVolume(sfxGameVolume);
    }

    public void OpenSettingsInMainMenu()
    {
        AnimatedBackground.Instance.OnHoverExit(1);
        AudioController.Instance.PlaySFX(3);

        mainMenu.SetActive(false);
        options.SetActive(true);

        // Load the saved volume value
        float savedMusicVolume = PlayerPrefs.GetFloat(musicVolumeKey, 1f);
        musicVolumeSlider.value = savedMusicVolume;
        SetMusicVolume(savedMusicVolume);

        float savedSFXVolume = PlayerPrefs.GetFloat(sfxVolumeKey, 1f);
        sfxVolumeSlider.value = savedSFXVolume;
        SetSFXVolume(savedSFXVolume);
    }

    public void ConfirmSettingsInMainMenu()
    {
        AudioController.Instance.PlaySFX(3);

        mainMenu.SetActive(true);
        options.SetActive(false);
        SetMusicVolume(musicGameVolume);
        SetSFXVolume(sfxGameVolume);
    }

    public void OnMusicVolumeChanged(float musicVolume)
    {
        // Set the volume for all audio sources
        SetMusicVolume(musicVolume);
        musicGameVolume = musicVolume;

        // Save the volume value
        PlayerPrefs.SetFloat(musicVolumeKey, musicVolume);
        PlayerPrefs.Save();
    }

    public void OnSFXVolumeChanged(float sfxVolume)
    {
        // Set the volume for all audio sources
        SetSFXVolume(sfxVolume);
        sfxGameVolume = sfxVolume;

        // Save the volume value
        PlayerPrefs.SetFloat(sfxVolumeKey, sfxVolume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float musicVolume)
    {
        // Find all game objects with the tag "Ball" in the scene
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Music");

        // Set the volume for all audio sources
        foreach (var gameObject in gameObjects)
        {
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource != null) // Check if the game object has an AudioSource component
            {
                audioSource.volume = musicVolume;
            }
        }
    }

    public void SetSFXVolume(float sfxVolume)
    {
        // Find all game objects with the tag "Ball" in the scene
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("SFX");

        // Set the volume for all audio sources
        foreach (var gameObject in gameObjects)
        {
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource != null) // Check if the game object has an AudioSource component
            {
                audioSource.volume = sfxVolume;
            }
        }
    }

    public void AreYouSure()
    {
        AudioController.Instance.PlaySFX(3);

        InventoryManager.Instance.settingsMenu.SetActive(false);
        areYouSureMenu.SetActive(true);

        InventoryManager.Instance.allowedToCloseInventory = false;
        InventoryManager.Instance.allowedToNavigate = false;
    }

    public void ReturnToGame()
    {
        AudioController.Instance.PlaySFX(3);

        InventoryManager.Instance.settingsMenu.SetActive(true);
        areYouSureMenu.SetActive(false);

        InventoryManager.Instance.allowedToCloseInventory = true;
        if (Tutorial.Instance.TutorialComplete)
        {
            InventoryManager.Instance.allowedToNavigate = true;
        }
    }

    public void GoToMainMenu()
    {
        AudioController.Instance.PlaySFX(3);
        InventoryManager.Instance.ShowMouse();

        // Load the MainMenu
        SceneManager.LoadScene(0);
    }

    public void StartNewGame()
    {
        AudioController.Instance.PlaySFX(3);
        InventoryManager.Instance.HideMouse();

        SceneManager.LoadScene(1);
    }

    public void SeeMore()
    {
        AudioController.Instance.PlaySFX(3);

        Application.OpenURL(url);
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