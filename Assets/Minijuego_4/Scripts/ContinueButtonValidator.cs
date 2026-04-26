using UnityEngine;
using UnityEngine.UI;

public class ContinueButtonValidator : MonoBehaviour
{
    [SerializeField] private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        if (StickerManager.Instance == null) return;

        bool canContinue = StickerManager.Instance.AreAllStickersPlaced();

        button.interactable = canContinue;
    }
}