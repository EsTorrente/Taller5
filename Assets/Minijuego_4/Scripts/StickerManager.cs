using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerManager : MonoBehaviour
{
    // evento que avisa a otros scripts cuando cambia el estado de los stickers
    public event Action<bool> OnStickerStateChanged;

    [Header("Sticker Prefabs")]
    [SerializeField] private GameObject clockStickerPrefab;
    [SerializeField] private GameObject guiltStickerPrefab;

    private Dictionary<StickerType, GameObject> stickers = new();
    private GameObject currentSticker;
    private RectTransform canvasRect;

    void Awake()
    {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    void Update()
    {
        // solo actualizo si hay un sticker siguiendo el mouse
        if (currentSticker != null && currentSticker.transform.parent == canvasRect)
        {
            RectTransform rt = currentSticker.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, null, out Vector2 pos);
            rt.anchoredPosition = pos;
        }
    }

    public void PickSticker(StickerType type)
    {
        if (currentSticker != null)
        {
            StickerType currentType = GetStickerType(currentSticker);

            if (currentType != type)
            {
                ReturnStickerToDrawer(currentType);
            }
        }

        GameObject prefab = type switch
        {
            StickerType.Clock => clockStickerPrefab,
            StickerType.Guilt => guiltStickerPrefab,
            _ => null
        };

        if (prefab == null) return;

        if (stickers.ContainsKey(type))
        {
            if (currentSticker == stickers[type])
            {
                ReturnStickerToDrawer(type);
                return;
            }
            currentSticker = stickers[type];
        }
        else
        {
            GameObject instance = Instantiate(prefab, canvasRect);
            stickers[type] = instance;
            currentSticker = instance;
        }

        currentSticker.transform.SetParent(canvasRect);
        currentSticker.transform.SetAsLastSibling();

        if (currentSticker.TryGetComponent(out StickerVisual visual))
            visual.SetFollowing(true);

        CheckCompletion();
    }

    public void PlaceSticker(Transform parent)
    {
        if (currentSticker == null) return;

        StickerType type = GetStickerType(currentSticker);

        // busco si el objetivo o sus padres implementan IStickerReceiver
        IStickerReceiver receiver = parent.GetComponentInParent<IStickerReceiver>();

        if (receiver == null || !receiver.CanAcceptSticker(type))
        {
            StartCoroutine(BlinkRed(currentSticker));
            return;
        }

        RectTransform rt = currentSticker.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent as RectTransform, Input.mousePosition, null, out Vector2 localPos);

        currentSticker.transform.SetParent(parent);
        rt.anchoredPosition = localPos;

        if (currentSticker.TryGetComponent(out StickerVisual visual))
            visual.SetFollowing(false);

        currentSticker = null;
        CheckCompletion(); // aviso a los observadores
    }

    public void RemoveStickersFromParent(Transform parent)
    {
        List<StickerType> toRemove = new List<StickerType>();

        foreach (var kvp in stickers)
        {
            if (kvp.Value != null && kvp.Value.transform.IsChildOf(parent))
            {
                Destroy(kvp.Value);
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var key in toRemove) stickers.Remove(key);

        currentSticker = null;
        CheckCompletion();
    }

    private void CheckCompletion()
    {
        // disparo el evento para que los botones sepan si activarse
        bool isComplete = AreAllStickersPlaced();
        OnStickerStateChanged?.Invoke(isComplete);
    }

    private bool AreAllStickersPlaced()
    {
        if (!stickers.ContainsKey(StickerType.Clock) ||
            !stickers.ContainsKey(StickerType.Guilt))
            return false;

        GameObject clock = stickers[StickerType.Clock];
        GameObject guilt = stickers[StickerType.Guilt];

        if (clock == null || guilt == null) return false;
        return clock.transform.parent != canvasRect && guilt.transform.parent != canvasRect;
    }

    private StickerType GetStickerType(GameObject sticker)
    {
        foreach (var kvp in stickers)
        {
            if (kvp.Value == sticker) return kvp.Key;
        }

        throw new Exception("Sticker no encontrado");
    }

    private IEnumerator BlinkRed(GameObject sticker)
    {
        if (!sticker.TryGetComponent(out UnityEngine.UI.Image img)) yield break;

        Color colorDeReposo = Color.white;

        img.color = Color.red;
        yield return new WaitForSeconds(0.15f);

        if (img != null)
        {
            img.color = colorDeReposo;
        }
    }

    private void ReturnStickerToDrawer(StickerType type)
    {
        if (stickers.TryGetValue(type, out GameObject sticker))
        {
            Destroy(sticker);
            stickers.Remove(type);
        }
        currentSticker = null;
        CheckCompletion();
    }

    public void PickClockSticker()
    {
        PickSticker(StickerType.Clock);
    }

    public void PickGuiltSticker()
    {
        PickSticker(StickerType.Guilt);
    }
}