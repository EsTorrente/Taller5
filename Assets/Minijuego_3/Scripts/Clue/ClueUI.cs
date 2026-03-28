using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClueUI : MonoBehaviour
{
    [Header("Data")]
    public ClueData clueData;

    [Header("UI References")]
    public Image baseImage;
    public Image markerImage;

    [Header("Marker Sprites")]
    public Sprite relevantSprite;
    public Sprite irrelevantSprite;

    private ClueState currentState = ClueState.None;
    
    private PhotoManager photoManager;

    void Start()
    {
        photoManager = FindObjectOfType<PhotoManager>();

        currentState = clueData.currentState;

        UpdateVisual();
    }

    public void OnClick()
{
    if (photoManager.IsCurrentPhotoSolved())
        return;

    CycleState();
    clueData.currentState = currentState;
    UpdateVisual();

    photoManager.CheckPhotoCompletion();
}

    void CycleState()
{
    currentState = (ClueState)(((int)currentState + 1) % 3);
}

    void UpdateVisual()
    {
        markerImage.enabled = currentState != ClueState.None;

        if (currentState == ClueState.Relevant)
        {
            markerImage.sprite = relevantSprite;
            baseImage.color = Color.white; 
        }
        else if (currentState == ClueState.Irrelevant)
        {
            markerImage.sprite = irrelevantSprite;
            baseImage.color = Color.white; 
        }
        else
        {
            baseImage.color = Color.white;
        }
    }

    public bool IsCorrect()
    {
        if (currentState == ClueState.None)
            return false;

        return (currentState == ClueState.Relevant && clueData.isActuallyRelevant)
            || (currentState == ClueState.Irrelevant && !clueData.isActuallyRelevant);
    }

    public void SetClue(ClueData newClue)
{
    clueData = newClue;

    currentState = clueData.currentState;

    UpdateVisual();
}

    public ClueState GetState()
    {
        return currentState;
    }
}
