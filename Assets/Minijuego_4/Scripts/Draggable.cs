using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    [Header("Escala")]
    [SerializeField] private float scaleMultiplier = 1.15f;
    private Vector3 originalScale;

    [Header("Rotación tipo colgante")]
    [SerializeField] private float rotationAmplitude = 10f;
    [SerializeField] private float rotationSpeed = 5f;

    private bool isDragging = false;
    private float timeOffset;

    private StickerManager stickerManager;

    public void Initialize(StickerManager manager)
    {
        stickerManager = manager;
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rectTransform.localScale;

        timeOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        if (isDragging)
        {
            float angle = Mathf.Sin(Time.time * rotationSpeed + timeOffset) * rotationAmplitude;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        rectTransform.localScale = originalScale * scaleMultiplier;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        rectTransform.localScale = originalScale * scaleMultiplier;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        rectTransform.localScale = originalScale;
        rectTransform.rotation = Quaternion.identity;
    }

}