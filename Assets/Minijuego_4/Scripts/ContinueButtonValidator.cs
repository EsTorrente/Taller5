using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ContinueButtonValidator : MonoBehaviour
{
    private Button button;
    [SerializeField] private StickerManager stickerManager;

    void Awake()
    {
        button = GetComponent<Button>();
        button.interactable = false; // estado inicial por defecto
    }

    void OnEnable()
    {
        // lo suscribo al evento
        if (stickerManager != null)
            stickerManager.OnStickerStateChanged += UpdateButtonState;
    }

    void OnDisable()
    {
        // lo desuscribo para evitar fugas de memoria
        if (stickerManager != null)
            stickerManager.OnStickerStateChanged -= UpdateButtonState;
    }

    private void UpdateButtonState(bool canContinue)
    {
        button.interactable = canContinue;
    }
}