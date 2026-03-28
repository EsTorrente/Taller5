using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiarioManager : MonoBehaviour
{
    [Header("Objetos")]
    public GameObject diarioCerrado;
    public GameObject panelCombinacion;

    [Header("Textos de las casillas")]
    public TextMeshProUGUI texto1;
    public TextMeshProUGUI texto2;
    public TextMeshProUGUI texto3;

    private int[] digitos = { 0, 0, 0 };
    private int[] codigoCorrecto = { 0, 0, 0 };

    void Start()
    {
        diarioCerrado.SetActive(false);
        panelCombinacion.SetActive(false);
    }

    // Abre el diario desde DiarioLyra
    public void AbrirDiario()
    {
        diarioCerrado.SetActive(true);
        panelCombinacion.SetActive(true);
        ActualizarTextos();
    }

    // Botones arriba
    public void SubirDigito(int indice)
    {
        digitos[indice] = (digitos[indice] + 1) % 10;
        ActualizarTextos();
    }

    // Botones abajo
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
        if (digitos[0] == codigoCorrecto[0] &&
            digitos[1] == codigoCorrecto[1] &&
            digitos[2] == codigoCorrecto[2])
        {
            Debug.Log("CORRECTO - fin del juego");
            Application.Quit();

            // Esto es para probar en el editor:
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else
        {
            Debug.Log("INCORRECTO");
        }
    }
}
