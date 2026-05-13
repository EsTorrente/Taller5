using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DiarioManager : MonoBehaviour
{
    // En el inspector aparecen 3 casillas para asignar los siguientes objetos 
    [Header("Objetos")]
    public GameObject diarioCerrado;
    public GameObject panelCombinacion;
    public GameObject diarioLyra;

    // En el inspector aparecen 3 casillas para asignar los textos que se usaran en la contrase�a
    [Header("Textos de las casillas")]
    public TextMeshProUGUI texto1;
    public TextMeshProUGUI texto2;
    public TextMeshProUGUI texto3;

    // En el inspector aparecen 3 casillas para asignar los siguientes colores
    [Header("Colores")]
    public Color colorCorrecto = Color.green;
    public Color colorMalPos = Color.yellow;
    public Color colorIncorrecto = Color.red;

    // En el inspector aparecera el array en ceros que corresponde a los numeros del usuario al iniciar el juego
    public int[] digitos = { 0, 0, 0 };
    // En el inspector aparecera el array con los numeros de la contrase�a correcta
    public int[] codigoCorrecto = { 5, 6, 2 };

    void Start()
    {
        // Cuando le das play se apagan los objetos que asiganste a estas variables
        diarioCerrado.SetActive(false);
        panelCombinacion.SetActive(false);
    }

    public void AbrirDiario()
    {
        // Cuando le das click al objeto que tiene asignado este codigo, prende los objetos que antes tenias apagados
        diarioCerrado.SetActive(true);
        panelCombinacion.SetActive(true);
        ActualizarTextos();
    }

    public void CerrarDiario()
    {
        // Cuando le das click al objeto que tiene asignado este codigo, apaga los objetos que antes tenias prendidos.
        diarioCerrado.SetActive(false);
        panelCombinacion.SetActive(false);

        diarioLyra.SetActive(true);

    }

    public void SubirDigito(int indice)
    {
        // El objeto que tiene asigando este codigo debe decirme a que numero voy a modificar con el INDICE del array
        // Una vez haya un indice asignado, al darle click aumenta +1 el valor
        digitos[indice] = (digitos[indice] + 1) % 10;
        ActualizarTextos();
    }

    public void BajarDigito(int indice)
    {
        // El objeto que tiene asigando este codigo debe decirme a que numero voy a modificar con el INDICE del array
        // Una vez haya un indice asignado, al darle click disminuye -1 el valor >> (-1 para c# es +9)
        digitos[indice] = (digitos[indice] + 9) % 10;
        ActualizarTextos();
    }

    void ActualizarTextos()
    {
        //Usa las nuevas modificaciones hechas en los arrays de los numeros del usuario y las transforma a strings
        texto1.text = digitos[0].ToString();
        texto2.text = digitos[1].ToString();
        texto3.text = digitos[2].ToString();

        //El color base siempre sera blanco
        texto1.color = Color.white;
        texto2.color = Color.white;
        texto3.color = Color.white;
    }

    public void Comprobar()
    {
        Debug.Log("Digitos ingresados: " + digitos[0] + " " + digitos[1] + " " + digitos[2]);

        //Al darle click al boton que ejecuta este codigo, se crea un array NUEVO con los ultimos valores en strings asignados a los textos
        TextMeshProUGUI[] textos = { texto1, texto2, texto3 };

        //Se inicia un for que se repite 3 veces, analizando cada numero del nuevo array para dar feedback visual del codigo del usuario
        for (int i = 0; i < digitos.Length; i++)
        {
            if (digitos[i] == codigoCorrecto[i])
            {
                //Numero y posicion correctos
                textos[i].color = colorCorrecto; // Verde
            }
            else if (System.Array.Exists(codigoCorrecto, x => x == digitos[i]))
            {
                //Numero correcto y mala posicion
                textos[i].color = colorMalPos; // Amarillo
            }
            else
            {
                //Ninguna de las anteriores >> todo malo
                textos[i].color = colorIncorrecto; // Rojo
            }
        }

        //Se inicia un for que COMPRUEBA EN CONJUNTO todo el codigo del usuario con el correcto 
        if (digitos[0] == codigoCorrecto[0] &&
            digitos[1] == codigoCorrecto[1] &&
            digitos[2] == codigoCorrecto[2])
        {
            //Si es correcto, pasa de escena
            Debug.Log("CORRECTO");
            SceneManager.LoadScene("Corto_3");
        }
        else
        {
            //Si hay un solo digito malo, no pasa nada, solo marca los colores
            Debug.Log("INCORRECTO");
        }
    }
}