using UnityEngine;

public class ClueFactory : MonoBehaviour
{
    [Header("Configuración de Fábrica")]
    [SerializeField] private ClueUI cluePrefab;
    [SerializeField] private Transform clueContainer;

    public ClueUI CreateClue(ClueData data, PhotoManager manager) //pa crear las pistas
    {
        ClueUI newClue = Instantiate(cluePrefab, clueContainer); //lo instantica y lo mete dentro de lo q sea clue container

        newClue.Init(data, manager); // le inyecta los datos y le dice quien es el manager

        RectTransform rt = newClue.GetComponent<RectTransform>(); // agarra el control de posicion de la UI
        rt.anchoredPosition = data.position; // lo mueve a la posicion exacta
        rt.localScale = data.scale; // le cambia el tamaño

        return newClue; // devuelve la pista
    }
}