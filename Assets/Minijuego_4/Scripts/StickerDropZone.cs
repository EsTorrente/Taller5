using UnityEngine;
using UnityEngine.EventSystems;

public class StickerDropZone : MonoBehaviour, IStickerReceiver, IPointerClickHandler
{
    [SerializeField] private bool acceptsClock = true;
    [SerializeField] private bool acceptsGuilt = false;

    private StickerManager stickerManager;

    void Awake()
    {
        stickerManager = Object.FindFirstObjectByType<StickerManager>();
    }

    public bool CanAcceptSticker(StickerType type)
    {
        return type switch
        {
            StickerType.Clock => acceptsClock,
            StickerType.Guilt => acceptsGuilt,
            _ => false
        };
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (stickerManager != null)
        {
            stickerManager.PlaceSticker(this.transform);
        }
    }
}