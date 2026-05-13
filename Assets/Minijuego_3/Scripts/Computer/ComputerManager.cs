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

    public void OnPasswordChanged() // se ejecuta cada vez que el jugador pone algo en el input field
    {
        errorText.text = ""; // borra el error para limpiar la pantalla
    }

    public void TryLogin() // se ejecuta al darle clic al boton de entrar o enter (creo la verdad no me acuerdo si enter si funciona así con el input fiel)
    {
        string input = passwordInput.text; 

        if (input == correctPassword) //compara con la clave correcta
        {
            UnlockComputer(); //si es igual, abre el pc
        }
        else
        {
            errorText.text = "Contraseña incorrecta";
        }
    }

    void UnlockComputer()
    {
        loginPanel.SetActive(false); // apaga el login
        desktopPanel.SetActive(true); // prende el escritorio
    }

    public void NextScene() //lo q llama el botoncito pa cambiar de escena
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // carga la escena que sigue en la lista
    }
}