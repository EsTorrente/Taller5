using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DiarioManager : MonoBehaviour
{
    [Header("Objetos")]
    public GameObject diarioCerrado;
    public GameObject panelCombinacion;
    public GameObject diarioLyra;

    [Header("Textos de las casillas")]
    public TextMeshProUGUI texto1;
    public TextMeshProUGUI texto2;
    public TextMeshProUGUI texto3;

    private int[] digitos = { 0, 0, 0 };
    private int[] codigoCorrecto = { 5, 8, 2 };

    void Start()
    {
        diarioCerrado.SetActive(false);
        panelCombinacion.SetActive(false);
    }

    public void AbrirDiario()
    {
        diarioCerrado.SetActive(true);
        panelCombinacion.SetActive(true);
        ActualizarTextos();
    }

    public void CerrarDiario()
    {
        diarioCerrado.SetActive(false);
        panelCombinacion.SetActive(false);
        diarioLyra.SetActive(true);
    }

    public void SubirDigito(int indice)
    {
        digitos[indice] = (digitos[indice] + 1) % 10;
        ActualizarTextos();
    }

    public void BajarDigito(int indice)
    {
        digitos[indice] = (digitos[indice] + 9) % 10;
        ActualizarTextos();
    }

    void ActualizarTextos()
    {
        texto1.text = digitos[0].ToString();
        texto2.text = digitos[1].ToString();
        texto3.text = digitos[2].ToString();
    }

    public void Comprobar()
    {
        Debug.Log("Digitos ingresados: " + digitos[0] + " " + digitos[1] + " " + digitos[2]);

        if (digitos[0] == codigoCorrecto[0] &&
            digitos[1] == codigoCorrecto[1] &&
            digitos[2] == codigoCorrecto[2])
        {
            Debug.Log("CORRECTO");
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            Debug.Log("INCORRECTO");
        }
    }
}