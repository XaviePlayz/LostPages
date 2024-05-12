using UnityEngine;
using UnityEngine.UI;

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
    public Slider musicVolumeSlider;
    private const string musicVolumeKey = "Music Volume";
    public float musicGameVolume = 0.5f;

    [Header("SFX Volume")]
    public Slider sfxVolumeSlider;
    private const string sfxVolumeKey = "SFX Volume";
    public float sfxGameVolume = 0.5f;

    [Header("Muffled Volume")]
    public AudioLowPassFilter lowPassFilter;
    public float normalFrequency = 22000f;
    public float muffledCutoffFrequency = 350f;

    private void Start()
    {
        // Load the saved MUSIC value
        float savedMusicVolume = PlayerPrefs.GetFloat(musicVolumeKey, 0.5f);
        musicVolumeSlider.value = savedMusicVolume;
        SetMusicVolume(savedMusicVolume);

        musicGameVolume = savedMusicVolume;
        musicGameVolume = musicVolumeSlider.value;

        // Load the saved SFX value
        float savedSFXVolume = PlayerPrefs.GetFloat(sfxVolumeKey, 0.5f);
        sfxVolumeSlider.value = savedSFXVolume;
        SetSFXVolume(savedSFXVolume);

        sfxGameVolume = savedSFXVolume;
        sfxVolumeSlider.value = sfxGameVolume;

        normalFrequency = 22000f;
        muffledCutoffFrequency = 350f;

        // Attach a listener to the slider's OnValueChanged event
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    public void OpenSettings()
    {
        SetMusicVolume(musicGameVolume);
        GameOptionsManager.Instance.ShowGameOptions();
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

        GameOptionsManager.Instance.mainMenu.SetActive(false);
        GameOptionsManager.Instance.options.SetActive(true);
    }

    public void ConfirmSettingsInMainMenu()
    {
        GameOptionsManager.Instance.OnHoverExitMainMenu();
        AudioController.Instance.PlaySFX(3);

        GameOptionsManager.Instance.mainMenu.SetActive(true);
        GameOptionsManager.Instance.options.SetActive(false);
    }

    public void ShowVolumeOptions()
    {
        GameOptionsManager.Instance.showGameOptions.SetActive(false);
        GameOptionsManager.Instance.showVolumeOptions.SetActive(true);

        GameOptionsManager.Instance.viewGameButton.color = GameOptionsManager.Instance.canBeViewedButtonColor;
        GameOptionsManager.Instance.viewGameButtonText.color = GameOptionsManager.Instance.canBeViewedTextButtonColor;

        GameOptionsManager.Instance.viewVolumeButton.color = GameOptionsManager.Instance.currentViewButtonColor;
        GameOptionsManager.Instance.viewVolumeButtonText.color = GameOptionsManager.Instance.currentViewButtonColor;

        // Load the saved MUSIC value
        float savedMusicVolume = PlayerPrefs.GetFloat(musicVolumeKey, 0.5f);
        musicVolumeSlider.value = savedMusicVolume;
        SetMusicVolume(savedMusicVolume);

        musicGameVolume = savedMusicVolume;
        musicGameVolume = musicVolumeSlider.value;

        // Load the saved SFX value
        float savedSFXVolume = PlayerPrefs.GetFloat(sfxVolumeKey, 0.5f);
        sfxVolumeSlider.value = savedSFXVolume;
        SetSFXVolume(savedSFXVolume);

        sfxGameVolume = savedSFXVolume;
        sfxVolumeSlider.value = sfxGameVolume;
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
}