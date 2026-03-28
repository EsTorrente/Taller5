using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ComputerManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panel;
    public GameObject loginPanel;
    public GameObject desktopPanel;

    [Header("Login UI")]
    public TMP_InputField passwordInput;
    public TextMeshProUGUI errorText;

    [Header("Password")]
    public string correctPassword;

    void Start()
    {
        panel.SetActive(false);
        loginPanel.SetActive(true);
        desktopPanel.SetActive(false);

        errorText.text = "";
    }

    public void OnPasswordChanged()
    {
        errorText.text = "";
    }

    public void Show()
    {
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public void TryLogin()
    {
        string input = passwordInput.text;

        if (input == correctPassword)
        {
            UnlockComputer();
        }
        else
        {
            errorText.text = "Contraseña incorrecta";
        }
    }

    void UnlockComputer()
    {
        loginPanel.SetActive(false);
        desktopPanel.SetActive(true);
    }


    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}