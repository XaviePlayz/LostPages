using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOptionsManager : MonoBehaviour
{
    #region Singleton

    private static GameOptionsManager _instance;
    public static GameOptionsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameOptionsManager>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(GameOptionsManager).Name;
                    _instance = obj.AddComponent<GameOptionsManager>();
                }
            }
            return _instance;
        }
    }

    #endregion

    [Header("Scenes")]
    public GameObject mainMenu;
    public GameObject options;
    public GameObject areYouSureMenu;

    [Header("Link To My Portfolio")]
    public string url;

    [Header("Options")]
    public GameObject showGameOptions;
    public GameObject showVolumeOptions;

    [Header("Buttons")]
    public Image viewGameButton;
    public Image viewVolumeButton;
    public TextMeshProUGUI viewGameButtonText;
    public TextMeshProUGUI viewVolumeButtonText;
    public GameObject arrowMainMenuButton;

    [Header("Button Colors")]
    public Color currentViewButtonColor;
    public Color canBeViewedButtonColor;
    public Color canBeViewedTextButtonColor;

    [Header("Texts")]
    public TextMeshProUGUI exampleTextElement;
    public TextMeshProUGUI dialogueTextElement;
    public TextMeshProUGUI exampleTextSpeedElement;

    [Header("Text Size Controller")]
    public Slider textSizeSlider;
    private float dialogueTextSize;
    private float[] textSizeOptions = { 14, 16, 18, 20, 22 };
    private const string textSizeKey = "Text Size";

    [Header("Text Speed Controller")]
    public Slider textSpeedSlider;
    private float dialogueTextSpeed;
    public float typingSpeed;
    private float[] textSpeedOptions = { 0.07f, 0.06f, 0.05f, 0.04f, 0.03f };
    private const string textSpeedKey = "Text Speed";
    private void Start()
    {
        // Load the saved TEXT SIZE value
        float savedTextSize = PlayerPrefs.GetFloat(textSizeKey, 2f);
        textSizeSlider.value = savedTextSize;
        SetTextSize(savedTextSize);

        dialogueTextSize = savedTextSize;
        dialogueTextSize = textSizeSlider.value;


        // Load the saved TEXT SPEED value
        float savedTextSpeed = PlayerPrefs.GetFloat(textSpeedKey, 2f);
        textSpeedSlider.value = savedTextSpeed;
        SetTextSpeed(savedTextSpeed);

        dialogueTextSpeed = savedTextSpeed;
        dialogueTextSpeed = textSpeedSlider.value;

        // Load Game
        if (options != null)
        {
            options.SetActive(false);
        }

        if (showVolumeOptions != null)
        {
            showVolumeOptions.SetActive(false);
        }

        if (areYouSureMenu != null)
        {
            areYouSureMenu.SetActive(false);
        }

        viewGameButton.color = currentViewButtonColor;
        viewVolumeButton.color = canBeViewedButtonColor;

        // Attach a listener to the slider's OnValueChanged event
        textSizeSlider.onValueChanged.AddListener(OnTextSizeSliderValueChanged);
        textSpeedSlider.onValueChanged.AddListener(OnTextSpeedSliderValueChanged);
    }


    #region Title Screen Buttons
    public void StartNewGame()
    {
        AudioController.Instance.PlaySFX(3);
        HideMouse();

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

    public void SeeMore()
    {
        AudioController.Instance.PlaySFX(3);

        Application.OpenURL(url);
    }
    #endregion

    public void GoToMainMenu()
    {
        AudioController.Instance.PlaySFX(3);
        ShowMouse();

        // Load the MainMenu
        SceneManager.LoadScene(0);
    }

    public void AreYouSure()
    {
        OnHoverExitMainMenu();
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

    public void ShowGameOptions()
    {
        showGameOptions.SetActive(true);
        showVolumeOptions.SetActive(false);

        viewGameButton.color = currentViewButtonColor;
        viewGameButtonText.color = currentViewButtonColor;

        viewVolumeButton.color = canBeViewedButtonColor;
        viewVolumeButtonText.color = canBeViewedTextButtonColor;

        // Load the saved TEXT SIZE value
        float savedTextSize = PlayerPrefs.GetFloat(textSizeKey, 2f);
        textSizeSlider.value = savedTextSize;
        SetTextSize(savedTextSize);

        dialogueTextSize = savedTextSize;
        dialogueTextSize = textSizeSlider.value;


        // Load the saved TEXT SPEED value
        float savedTextSpeed = PlayerPrefs.GetFloat(textSpeedKey, 2f);
        textSpeedSlider.value = savedTextSpeed;
        SetTextSpeed(savedTextSpeed);

        dialogueTextSpeed = savedTextSpeed;
        dialogueTextSpeed = textSpeedSlider.value;

        TestTextSpeed();
    }

    public void OnTextSizeSliderValueChanged(float textSize)
    {
        // Set the Text Size for the Dialogue
        SetTextSize(textSize);
        dialogueTextSize = textSize;

        // Save the TEXT SIZE value
        PlayerPrefs.SetFloat(textSizeKey, textSize);
        PlayerPrefs.Save();
    }

    public void OnTextSpeedSliderValueChanged(float textSpeed)
    {
        SetTextSpeed(textSpeed);
        dialogueTextSpeed = textSpeed;

        // Save the TEXT SPEED value
        PlayerPrefs.SetFloat(textSpeedKey, textSpeed);
        PlayerPrefs.Save();
    }

    public void SetTextSize(float textSize)
    {
        // Get the slider value and convert it to an integer index
        int sliderValue = (int)textSizeSlider.value;

        // Update the text size based on the slider value
        if (dialogueTextElement != null)
        {
            dialogueTextElement.fontSize = textSizeOptions[sliderValue];
        }
        exampleTextElement.fontSize = textSizeOptions[sliderValue];
    }

    public void SetTextSpeed(float textSpeed)
    {
        // Get the slider value and convert it to an integer index
        int sliderValue = (int)textSpeedSlider.value;

        // Update the text size based on the slider value
        typingSpeed = textSpeedOptions[sliderValue];
    }

    public void HoverEnterGameOptionsButton()
    {
        if (showVolumeOptions.activeSelf && viewGameButton.GetComponent<Button>().interactable == true)
        {
            viewGameButtonText.color = currentViewButtonColor;
        }
    }
    public void HoverExitGameOptionsButton()
    {
        if (showVolumeOptions.activeSelf && viewGameButton.GetComponent<Button>().interactable == true)
        {
            viewGameButtonText.color = canBeViewedTextButtonColor;
        }
    }
    public void HoverEnterVolumeOptionsButton()
    {
        if (showGameOptions.activeSelf && viewVolumeButton.GetComponent<Button>().interactable == true)
        {
            viewVolumeButtonText.color = currentViewButtonColor;
        }
    }

    public void HoverExitVolumeOptionsButton()
    {
        if (showGameOptions.activeSelf && viewVolumeButton.GetComponent<Button>().interactable == true)
        {
            viewVolumeButtonText.color = canBeViewedTextButtonColor;
        }
    }
    public void TestTextSpeed()
    {
        StopAllCoroutines();
        StartCoroutine(TypeSentence("Every chapter we write is just another door we open into a new world"));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        exampleTextSpeedElement.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            exampleTextSpeedElement.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void OnHoverEnterMainMenu()
    {
        arrowMainMenuButton.SetActive(true);
    }

    public void OnHoverExitMainMenu()
    {
        arrowMainMenuButton.SetActive(false);
    }
    public void ShowMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void HideMouse()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }
}