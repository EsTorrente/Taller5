using UnityEngine;
using UnityEngine.EventSystems;

public class StickerDropZone : MonoBehaviour, IStickerReceiver, IPointerClickHandler
{
    //========================================================
    // CONFIGURACIÓN
    //========================================================

    // define qué tipos de stickers puede aceptar la zona
    [SerializeField] private bool acceptsClock = true;
    [SerializeField] private bool acceptsGuilt = false;

    // referencia al manager principal de stickers
    private StickerManager stickerManager;

    //========================================================
    // UNITY METHODS
    //========================================================

    private void Awake()
    {
        // busco automáticamente el manager en escena
        stickerManager = Object.FindFirstObjectByType<StickerManager>();
    }

    //========================================================
    // VALIDACIÓN
    //========================================================

    // la interfaz permite que StickerManager no necesite saber específicamente qué tipo de objeto recibió el click :P
    public bool CanAcceptSticker(StickerType type)
    {
        return type switch
        {
            StickerType.Clock => acceptsClock,
            StickerType.Guilt => acceptsGuilt,
            _ => false
        };
    }

    //========================================================
    // INPUT
    //========================================================

    // intenta colocar el sticker actual sobre esta zona
    public void OnPointerClick(PointerEventData eventData)
    {
        if (stickerManager != null)
        {
            stickerManager.PlaceSticker(transform);
        }
    }
}