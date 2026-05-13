using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    //========================================================
    // REFERENCIAS
    //========================================================

    private RectTransform rectTransform;
    private Canvas canvas;

    //========================================================
    // CONFIGURACIÓN VISUAL
    //========================================================

    [Header("Rotación tipo colgante")]
    [SerializeField] private float rotationAmplitude = 10f;
    [SerializeField] private float rotationSpeed = 5f;

    //========================================================
    // ESTADO
    //========================================================

    // indica si el objeto está siendo arrastrado
    private bool isDragging = false;

    // offset aleatorio para que distintos objetos no roten exactamente igual (ni se nota, pero yo sé que está ahí y me hace feliz)
    private float timeOffset;

    private StickerManager stickerManager;

    //========================================================
    // INICIALIZACIÓN
    //========================================================

    // permite inyectar dependencias LUEGO de instanciar el objeto
    public void Initialize(StickerManager manager)
    {
        stickerManager = manager;
    }

    //========================================================
    // UNITY METHODS
    //========================================================

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        timeOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        ApplyDraggingRotation();
    }

    //========================================================
    // EFECTO VISUAL
    //========================================================

    // genera una rotación de seno mientras se arrastra para dar sensación de objeto colgando
    private void ApplyDraggingRotation()
    {
        if (!isDragging)
            return;

        float angle =
            Mathf.Sin(Time.time * rotationSpeed + timeOffset)
            * rotationAmplitude;

        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    //========================================================
    // DRAG
    //========================================================

    // comienza el arrastre
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    // mueve el objeto en coordenadas UI
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }

    // termina el arrastre
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        // restauro la rotación
        rectTransform.rotation = Quaternion.identity;
    }
}