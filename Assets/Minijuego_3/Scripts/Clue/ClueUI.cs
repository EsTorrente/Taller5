using UnityEngine;
using UnityEngine.UI;

public class ClueUI : MonoBehaviour
{
    [SerializeField] private Image markerImage;

    [Header("Marker Sprites")]
    [SerializeField] private Sprite relevantSprite;
    [SerializeField] private Sprite irrelevantSprite;
    [SerializeField] private Sprite noneSprite;

    private ClueData clueData;
    private ClueState currentState = ClueState.None;
    private PhotoManager photoManager;

    public void Init(ClueData newClue, PhotoManager manager) // metodo para meterle los datos apenas se crea
    {
        clueData = newClue;
        photoManager = manager;
        currentState = clueData.currentState;

        UpdateVisual(); // actualiza el dibujito
    }

    public void OnClick()
    {
        if (photoManager.IsCurrentPhotoSolved()) // pregunta si la foto ya se resolvio
            return; //si si pues no hace nada

        CycleState(); // cambia el estado al siguiente
        clueData.currentState = currentState;
        UpdateVisual();

        photoManager.CheckPhotoCompletion(); // checkea a ver si esta completada
    }

    void CycleState()
    {
        currentState = (ClueState)(((int)currentState + 1) % 3); // le suma 1 al estado y vuelve a cero si pasa de 2, gracias modulo
    }

    void UpdateVisual() //actualiza las visuales
    {
        markerImage.enabled = true;

        if (currentState == ClueState.None)
        {
            markerImage.sprite = noneSprite;
        }
        else if (currentState == ClueState.Relevant)
        {
            markerImage.sprite = relevantSprite;
        }
        else if (currentState == ClueState.Irrelevant) 
        {
            markerImage.sprite = irrelevantSprite;
        }
    }

    public bool IsCorrect() //mira a ver si esta correcta
    {
        if (currentState == ClueState.None)
            return false;

        return (currentState == ClueState.Relevant && clueData.isActuallyRelevant)
            || (currentState == ClueState.Irrelevant && !clueData.isActuallyRelevant);
    }

    public ClueState GetState() //pa q los otros scripts miren a ver si esta bien
    {
        return currentState;
    }
}