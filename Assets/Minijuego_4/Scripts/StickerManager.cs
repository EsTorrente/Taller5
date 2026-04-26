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

        if (stickers.ContainsKey(type))
        {
            GameObject sticker = stickers[type];

            if (currentSticker == sticker)
            {
                ReturnStickerToDrawer(type);
                return;
            }

            currentSticker = sticker;
        }
        else
        {
            GameObject instance = Instantiate(prefab, canvasRect);
            stickers[type] = instance;
            currentSticker = instance;
        }

        currentSticker.transform.SetParent(canvasRect);
        currentSticker.transform.SetAsLastSibling();

        StickerVisual visual = currentSticker.GetComponent<StickerVisual>();
        if (visual != null)
            visual.SetFollowing(true);
    }

    public void PlaceSticker(Transform parent)
    {
        if (currentSticker == null) return;

        string type = GetStickerType(currentSticker);

        if (!IsValidPlacement(type, parent))
        {
            StartCoroutine(BlinkRed(currentSticker));
            return;
        }

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
            visual.SetFollowing(false);

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

    public bool AreAllStickersPlaced()
    {
        if (!stickers.ContainsKey("clock") || !stickers.ContainsKey("guilt"))
            return false;

        GameObject clock = stickers["clock"];
        GameObject guilt = stickers["guilt"];

        if (clock == null || guilt == null)
            return false;

        if (clock.transform.parent == canvasRect) return false;
        if (guilt.transform.parent == canvasRect) return false;

        return true;
    }

    string GetStickerType(GameObject sticker)
    {
        foreach (var kvp in stickers)
        {
            if (kvp.Value == sticker)
                return kvp.Key;
        }

        return "";
    }

    bool IsValidPlacement(string type, Transform target)
    {
        if (target == null) return false;

        Transform t = target;

        while (t != null)
        {
            if (type == "clock" && t.CompareTag("PostIt"))
                return true;

            if (type == "guilt" && (t.CompareTag("Photo") || t.CompareTag("GuiltZone")))
                return true;

            t = t.parent;
        }

        return false;
    }

    System.Collections.IEnumerator BlinkRed(GameObject sticker)
    {
        var img = sticker.GetComponent<UnityEngine.UI.Image>();
        if (img == null) yield break;

        Color original = img.color;

        img.color = Color.red;
        yield return new WaitForSeconds(0.15f);

        img.color = original;
    }

    void ReturnStickerToDrawer(string type)
    {
        if (!stickers.ContainsKey(type)) return;

        GameObject sticker = stickers[type];

        if (sticker != null)
            Destroy(sticker);

        stickers.Remove(type);

        currentSticker = null;
    }
}