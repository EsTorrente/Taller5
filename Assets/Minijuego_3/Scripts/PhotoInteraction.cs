using UnityEngine;

public class PhotoInteraction : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El Transform del objeto que contiene la foto.")]
    [SerializeField] private Transform photoContainer; 

    [Header("Configuración de Zoom y Arrastre")]
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 1f;
    [SerializeField] private float maxZoom = 4f;
    [SerializeField] private float dragSpeed = 1f;
    [SerializeField] private float baseWidth = 1920f;
    [SerializeField] private float baseHeight = 1080f;

    private Vector3 lastMousePosition;
    public bool IsInteractionEnabled { get; set; } = true;

    void Update()
    {
        if (!IsInteractionEnabled) return;

        HandleZoom();
        HandleDrag();
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll != 0f)
        {
            Vector3 newScale = photoContainer.localScale + Vector3.one * scroll * zoomSpeed;
            newScale.x = Mathf.Clamp(newScale.x, minZoom, maxZoom);
            newScale.y = Mathf.Clamp(newScale.y, minZoom, maxZoom);
            newScale.z = Mathf.Clamp(newScale.z, minZoom, maxZoom); 

            photoContainer.localScale = newScale;

            if (newScale.x <= minZoom)
            {
                photoContainer.localPosition = new Vector3(0f, 0f, photoContainer.localPosition.z);
            }
            else
            {
                ApplyDragLimits();
            }
        }
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            photoContainer.localPosition += delta * dragSpeed;
            ApplyDragLimits();

            lastMousePosition = Input.mousePosition;
        }
    }

    private void ApplyDragLimits()
    {
        float currentScale = photoContainer.localScale.x;
        if (currentScale < 0.5f)
        {
            photoContainer.localPosition = new Vector3(0f, 0f, photoContainer.localPosition.z);
            return;
        }
        
        float maxLimitX = (baseWidth * (currentScale - 1f)) / 2f;
        float maxLimitY = (baseHeight * (currentScale - 1f)) / 2f;

        Vector3 currentPos = photoContainer.localPosition;
        currentPos.x = Mathf.Clamp(currentPos.x, -maxLimitX, maxLimitX);
        currentPos.y = Mathf.Clamp(currentPos.y, -maxLimitY, maxLimitY);

        photoContainer.localPosition = currentPos;
    }
}