using UnityEngine;

public class ClueFactory : MonoBehaviour
{
    [Header("Configuración de Fábrica")]
    [SerializeField] private ClueUI cluePrefab;
    [SerializeField] private Transform clueContainer; // El objeto padre de la UI donde van las pistas

    public ClueUI CreateClue(ClueData data, PhotoManager manager)
    {
        // Instanciamos el Prefab en el contenedor
        ClueUI newClue = Instantiate(cluePrefab, clueContainer);

        // Lo inicializamos con sus datos y la referencia al manager (Inyección)
        newClue.Init(data, manager);

        // Ajustamos su posición y escala según los datos
        RectTransform rt = newClue.GetComponent<RectTransform>();
        rt.anchoredPosition = data.position;
        rt.localScale = data.scale;

        return newClue;
    }
}