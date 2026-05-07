using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ComputerManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject desktopPanel;

    [Header("Login UI")]
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("Password")]
    [SerializeField] private string correctPassword;

    void Start()
    {
        loginPanel.SetActive(false);
        desktopPanel.SetActive(false);
        errorText.text = "";
    }

    public void OnPasswordChanged()
    {
        errorText.text = "";
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