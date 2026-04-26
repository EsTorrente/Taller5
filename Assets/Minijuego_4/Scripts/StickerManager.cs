using UnityEngine;
using System.Collections.Generic;

public class StickerManager : MonoBehaviour
{
    public static StickerManager Instance;

    [Header("Sticker Prefabs")]
    public GameObject clockStickerPrefab;
    public GameObject guiltStickerPrefab;

    private Dictionary<string, GameObject> stickers = new Dictionary<string, GameObject>();
    private GameObject currentSticker;

    private RectTransform canvasRect;

    void Awake()
    {
        Instance = this;
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    void Update()
    {
        if (currentSticker != null && currentSticker.transform.parent == canvasRect)
        {
            RectTransform rt = currentSticker.GetComponent<RectTransform>();

            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Input.mousePosition,
                null,
                out pos
            );

            rt.anchoredPosition = pos;
        }
    }

    public void PickSticker(string type)
    {
        GameObject prefab = null;

        if (type == "clock") prefab = clockStickerPrefab;
        if (type == "guilt") prefab = guiltStickerPrefab;

        if (prefab == null) return;

        if (!stickers.ContainsKey(type))
        {
            GameObject instance = Instantiate(prefab, canvasRect);
            stickers[type] = instance;
        }

        currentSticker = stickers[type];

        StickerVisual visual = currentSticker.GetComponent<StickerVisual>();
        if (visual != null)
        {
            visual.SetFollowing(true);
        }


        currentSticker.transform.SetParent(canvasRect);
        currentSticker.transform.SetAsLastSibling(); 
    }

    public void PlaceSticker(Transform parent)
    {
        if (currentSticker == null) return;

        RectTransform rt = currentSticker.GetComponent<RectTransform>();
        RectTransform parentRect = parent as RectTransform;

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            Input.mousePosition,
            null,
            out localPos
        );

        currentSticker.transform.SetParent(parent);
        rt.anchoredPosition = localPos;

        StickerVisual visual = currentSticker.GetComponent<StickerVisual>();
        if (visual != null)
        {
            visual.SetFollowing(false);
        }

        currentSticker = null;
    }

    public void RemoveStickersFromParent(Transform parent)
    {
        List<string> toRemove = new List<string>();

        foreach (var kvp in stickers)
        {
            GameObject sticker = kvp.Value;

            if (sticker != null && sticker.transform.IsChildOf(parent))
            {
                Destroy(sticker);
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var key in toRemove)
        {
            stickers.Remove(key);
        }

        currentSticker = null;
    }
}