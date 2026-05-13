using UnityEngine;

public class PostItSpawner : MonoBehaviour
{
    //========================================================
    // REFERENCIAS
    //========================================================

    [SerializeField] private GameObject postItPrefab;

    // área del tablero donde se instancian los post-its
    [SerializeField] private Transform boardArea;

    // referencias necesarias para inicializar scripts dinámicos
    [SerializeField] private StickerManager stickerManager;
    [SerializeField] private ThreadManager threadManager;

    //========================================================
    // CREACIÓN DE POST-ITS
    //========================================================

    // crea un nuevo post-it dentro del tablero
    public void SpawnPostIt()
    {
        //----------------------------------------------------
        // INSTANCIAR
        //----------------------------------------------------

        GameObject obj = Instantiate(postItPrefab, boardArea);

        RectTransform rt = obj.GetComponent<RectTransform>();

        //----------------------------------------------------
        // POSICIÓN ALEATORIA
        //----------------------------------------------------

        rt.anchoredPosition = new Vector2(
            Random.Range(-100f, 100f),
            Random.Range(-100f, 100f)
        );

        //----------------------------------------------------
        // INYECCIÓN DE DEPENDENCIAS
        //----------------------------------------------------

        // inyecto referencias necesarias a scripts creados dinámicamente

        if (obj.TryGetComponent(out Draggable draggable))
        {
            draggable.Initialize(stickerManager);
        }

        if (obj.TryGetComponent(out ThreadPoint threadPoint))
        {
            threadPoint.Initialize(threadManager);
        }
    }
}