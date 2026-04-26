using UnityEngine;
using UnityEngine.EventSystems;

public class StickerDropZone : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (StickerManager.Instance != null)
        {
            StickerManager.Instance.PlaceSticker(transform);
        }
    }
}