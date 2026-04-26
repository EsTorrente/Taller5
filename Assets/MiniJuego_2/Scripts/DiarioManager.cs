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

    [Header("Colores")]
    public Color colorCorrecto = Color.green;
    public Color colorMalPos = Color.yellow;
    public Color colorIncorrecto = Color.red;

    public int[] digitos = { 0, 0, 0 };
    public int[] codigoCorrecto = { 5, 6, 2 };

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

        texto1.color = Color.white;
        texto2.color = Color.white;
        texto3.color = Color.white;
    }

    public void Comprobar()
    {
        Debug.Log("Digitos ingresados: " + digitos[0] + " " + digitos[1] + " " + digitos[2]);

        TextMeshProUGUI[] textos = { texto1, texto2, texto3 };

        for (int i = 0; i < digitos.Length; i++)
        {
            if (digitos[i] == codigoCorrecto[i])
            {
                textos[i].color = colorCorrecto; // Verde
            }
            else if (System.Array.Exists(codigoCorrecto, x => x == digitos[i]))
            {
                textos[i].color = colorMalPos; // Amarillo
            }
            else
            {
                textos[i].color = colorIncorrecto; // Rojo
            }
        }

        if (digitos[0] == codigoCorrecto[0] &&
            digitos[1] == codigoCorrecto[1] &&
            digitos[2] == codigoCorrecto[2])
        {
            Debug.Log("CORRECTO");
            SceneManager.LoadScene("Corto_3");
        }
        else
        {
            Debug.Log("INCORRECTO");
        }
    }
}