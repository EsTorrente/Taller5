using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    [Header("Escala")]
    public float scaleMultiplier = 1.15f;
    private Vector3 originalScale;

    [Header("Rotación tipo colgante")]
    public float rotationAmplitude = 10f;
    public float rotationSpeed = 5f;

    private bool isDragging = false;
    private float timeOffset;

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
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        rectTransform.localScale = originalScale;
        rectTransform.rotation = Quaternion.identity;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (StickerManager.Instance != null)
        {
            StickerManager.Instance.PlaceSticker(transform);
        }
    }
}