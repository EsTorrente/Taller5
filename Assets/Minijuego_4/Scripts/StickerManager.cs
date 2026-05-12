using System;
using System.Collections.Generic;
using UnityEngine;

public class StickerManager : MonoBehaviour
{
    //========================================================
    // EVENTOS
    //========================================================

    // evento que avisa a otros scripts cuando cambia el estado de colocación de los stickers
    public event Action<bool> OnStickerStateChanged;

    //========================================================
    // PREFABS
    //========================================================

    [Header("Sticker Prefabs")]
    [SerializeField] private GameObject clockStickerPrefab;
    [SerializeField] private GameObject guiltStickerPrefab;

    //========================================================
    // REFERENCIAS Y ESTADO
    //========================================================

    // diccionario que guarda todos los stickers existentes
    //
    // key   = tipo de sticker (Clock / Guilt), está en el enum
    // value = instancia del sticker en la escena
    private Dictionary<StickerType, GameObject> stickers = new();

    // sticker que actualmente está siendo sostenido y siguiendo el mouse
    private GameObject currentSticker;

    // tipo del sticker actualmente agarrado
    private StickerType? currentStickerType;

    // referencia al canvas principal
    // se usa para mover stickers en UI
    private RectTransform canvasRect;

    // indica si actualmente hay un sticker siguiendo el cursor
    private bool isHoldingSticker;

    //========================================================
    // UNITY METHODS
    //========================================================

    private void Awake()
    {
        // guardo referencia al canvas automáticamente
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    private void Update()
    {
        // solo sigo el mouse si realmente hay un sticker agarrado
        if (isHoldingSticker)
        {
            FollowMouse();
        }
    }

    //========================================================
    // MOVIMIENTO DEL STICKER
    //========================================================

    // mueve el sticker actual junto al cursor
    private void FollowMouse()
    {
        if (currentSticker == null)
            return;

        RectTransform rt = currentSticker.GetComponent<RectTransform>();

        // convierto la posición del mouse a coordenadas locales del canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            null,
            out Vector2 pos
        );

        // muevo el sticker
        rt.anchoredPosition = pos;
    }

    //========================================================
    // AGARRAR STICKERS
    //========================================================

    // selecciona un sticker desde el cajón
    private void PickSticker(StickerType type)
    {
        //----------------------------------------------------
        // SI YA ESTOY SOSTENIENDO ESTE MISMO STICKER
        //----------------------------------------------------

        // significa que el usuario volvió a clickear el mismo botón para cancelar la selección
        if (currentSticker != null && currentStickerType == type)
        {
            RemoveSticker(type);
            return;
        }

        //----------------------------------------------------
        // SI ESTOY SOSTENIENDO OTRO STICKER
        //----------------------------------------------------

        // destruyo el sticker anterior antes de agarrar el nuevo
        if (currentSticker != null && currentStickerType.HasValue)
        {
            RemoveSticker(currentStickerType.Value);
        }

        //----------------------------------------------------
        // SI EL STICKER TODAVÍA NO EXISTE
        //----------------------------------------------------

        // lo creo y lo agrego al dictionary
        if (!stickers.ContainsKey(type))
        {
            GameObject prefab = GetPrefab(type);

            if (prefab == null)
                return;

            GameObject instance = Instantiate(prefab, canvasRect);

            stickers[type] = instance;
        }

        //----------------------------------------------------
        // AGARRAR EL STICKER
        //----------------------------------------------------

        // saco el sticker correspondiente del dictionary (el GameObject)
        currentSticker = stickers[type];

        // guardo también su tipo (el enum)
        currentStickerType = type;

        //----------------------------------------------------
        // ACTIVAR SEGUIMIENTO DEL MOUSE
        //----------------------------------------------------

        // lo paso al canvas para que siga el cursor
        currentSticker.transform.SetParent(canvasRect);

        // lo pongo al frente visualmente
        currentSticker.transform.SetAsLastSibling();

        // activo feedback visual
        if (currentSticker.TryGetComponent(out StickerVisual visual))
        {
            visual.SetFollowing(true);
        }

        isHoldingSticker = true;

        //----------------------------------------------------
        // ACTUALIZAR ESTADO
        //----------------------------------------------------

        // se llama aquí porque debe desactivar el botón al volver a agarrar un sticker que ya estaba puesto, hasta que se vuelva a pegar
        CheckCompletion();
    }

    //========================================================
    // COLOCAR STICKERS
    //========================================================

    // intenta colocar el sticker actual sobre un objeto
    public void PlaceSticker(Transform parent)
    {
        //----------------------------------------------------
        // VALIDACIÓN
        //----------------------------------------------------

        if (currentSticker == null || !currentStickerType.HasValue)
            return;

        //----------------------------------------------------
        // BUSCAR SI EL OBJETO ACEPTA STICKERS
        //----------------------------------------------------

        // busco la interfaz en el objeto o cualquiera de sus padres
        IStickerReceiver receiver = parent.GetComponentInParent<IStickerReceiver>();

        //----------------------------------------------------
        // SI EL OBJETO NO ACEPTA ESTE STICKER
        //----------------------------------------------------

        if (receiver == null || !receiver.CanAcceptSticker(currentStickerType.Value))
        {
            // hago feedback visual
            if (currentSticker.TryGetComponent(out StickerVisual visual))
            {
                visual.BlinkRed();
            }

            return;
        }

        //----------------------------------------------------
        // COLOCAR EL STICKER
        //----------------------------------------------------

        RectTransform rt = currentSticker.GetComponent<RectTransform>();

        // convierto la posición del mouse al espacio local del objeto
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent as RectTransform,
            Input.mousePosition,
            null,
            out Vector2 localPos
        );

        // hago al sticker hijo del objeto
        currentSticker.transform.SetParent(parent);

        // lo posiciono donde se hizo click
        rt.anchoredPosition = localPos;

        //----------------------------------------------------
        // DESACTIVAR FOLLOW
        //----------------------------------------------------

        if (currentSticker.TryGetComponent(out StickerVisual placedVisual))
        {
            placedVisual.SetFollowing(false);
        }

        //----------------------------------------------------
        // LIMPIAR ESTADO
        //----------------------------------------------------

        // ya no hay sticker en el mouse
        currentSticker = null;
        currentStickerType = null;

        isHoldingSticker = false;

        //----------------------------------------------------
        // ACTUALIZAR ESTADO
        //----------------------------------------------------

        CheckCompletion();
    }

    //========================================================
    // ELIMINAR STICKERS
    //========================================================

    // elimina todos los stickers hijos de un objeto
    public void RemoveStickersFromParent(Transform parent)
    {
        List<StickerType> toRemove = new();

        //----------------------------------------------------
        // BUSCAR STICKERS HIJOS
        //----------------------------------------------------

        foreach (var kvp in stickers)
        {
            GameObject sticker = kvp.Value;

            if (sticker != null && sticker.transform.IsChildOf(parent))
            {
                // guardo el tipo para eliminarlo después
                toRemove.Add(kvp.Key);
            }
        }

        //----------------------------------------------------
        // ELIMINAR STICKERS
        //----------------------------------------------------

        foreach (StickerType type in toRemove)
        {
            RemoveSticker(type);
        }
    }

    // elimina un sticker, limpia referencias y actualiza estado
    private void RemoveSticker(StickerType type)
    {
        //----------------------------------------------------
        // BUSCAR STICKER
        //----------------------------------------------------

        if (!stickers.TryGetValue(type, out GameObject sticker))
            return;

        //----------------------------------------------------
        // SI ERA EL STICKER ACTUAL
        //----------------------------------------------------

        if (sticker == currentSticker)
        {
            currentSticker = null;
            currentStickerType = null;

            isHoldingSticker = false;
        }

        //----------------------------------------------------
        // DESTRUIR Y ELIMINAR REFERENCIA
        //----------------------------------------------------

        Destroy(sticker);

        stickers.Remove(type);

        //----------------------------------------------------
        // ACTUALIZAR ESTADO
        //----------------------------------------------------

        CheckCompletion();
    }

    //========================================================
    // COMPLETADO
    //========================================================

    // revisa si ambos stickers ya fueron colocados
    private void CheckCompletion()
    {
        bool isComplete = AreAllStickersPlaced();

        // notifico a los scripts suscritos al evento
        OnStickerStateChanged?.Invoke(isComplete);
    }

    // comprueba si ambos stickers existen y ya fueron colocados fuera del canvas
    private bool AreAllStickersPlaced()
    {
        //----------------------------------------------------
        // SI FALTA ALGÚN STICKER
        //----------------------------------------------------

        if (!stickers.ContainsKey(StickerType.Clock) ||
            !stickers.ContainsKey(StickerType.Guilt))
            return false;

        //----------------------------------------------------
        // TRAER REFERENCIAS
        //----------------------------------------------------

        GameObject clock = stickers[StickerType.Clock];
        GameObject guilt = stickers[StickerType.Guilt];

        //----------------------------------------------------
        // VALIDACIÓN EXTRA porsiaca
        //----------------------------------------------------

        if (clock == null || guilt == null)
            return false;

        //----------------------------------------------------
        // REVISAR SI YA FUERON COLOCADOS
        //----------------------------------------------------

        // si siguen siendo hijos del canvas, todavía están en el mouse
        return clock.transform.parent != canvasRect &&
               guilt.transform.parent != canvasRect;
    }

    //========================================================
    // UTILIDADES
    //========================================================

    // devuelve el prefab correspondiente al tipo
    private GameObject GetPrefab(StickerType type)
    {
        return type switch
        {
            StickerType.Clock => clockStickerPrefab,
            StickerType.Guilt => guiltStickerPrefab,
            _ => null
        };
    }

    //========================================================
    // WRAPPERS UI
    //========================================================

    // wrapper para botón del sticker clock
    public void PickClockSticker()
    {
        PickSticker(StickerType.Clock);
    }

    // wrapper para botón del sticker guilt
    public void PickGuiltSticker()
    {
        PickSticker(StickerType.Guilt);
    }
}