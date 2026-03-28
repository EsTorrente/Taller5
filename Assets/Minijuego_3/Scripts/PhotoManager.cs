using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoManager : MonoBehaviour
{
    public List<PhotoData> photos;
    private bool currentPhotoSolved = false;
    private HashSet<int> solvedPhotos = new HashSet<int>();
    public Image photoDisplay;
    public List<ClueUI> clueSlots;
    public NotepadUI notepad;
    public AudioSource audioSource;

    private int index = 0;

    void Start()
    {
        UpdatePhoto();
    }

    public void NextPhoto()
    {
        index = (index + 1) % photos.Count;
        UpdatePhoto();
    }

    public void PreviousPhoto()
    {
        index = (index - 1 + photos.Count) % photos.Count;
        UpdatePhoto();
    }

    void UpdatePhoto()
{
    PhotoData currentPhoto = photos[index];

    photoDisplay.sprite = currentPhoto.photoSprite;
    currentPhotoSolved = solvedPhotos.Contains(index);

    for (int i = 0; i < clueSlots.Count; i++)
    {
        ClueData clue = currentPhoto.clues[i];

        clueSlots[i].SetClue(clue);

        RectTransform rt = clueSlots[i].GetComponent<RectTransform>();
        rt.anchoredPosition = clue.position;
    }
}

public bool IsCurrentPhotoSolved()
{
    return currentPhotoSolved;
}

    public void CheckPhotoCompletion()
{
    if (currentPhotoSolved) return;

    foreach (var clue in clueSlots)
    {
        if (!clue.IsCorrect())
            return;
    }

    SolveCurrentPhoto();
}

    void SolveCurrentPhoto()
{
    currentPhotoSolved = true;
    solvedPhotos.Add(index);

    Debug.Log("Foto resuelta");

    string clue = photos[index].finalClue;

    Debug.Log("Pista desbloqueada: " + clue);

    if (notepad != null)
    {
        notepad.AddNote(" " + clue + "\n");
    }
    else
    {
        Debug.LogError("NOTEPAD NO ASIGNADO EN INSPECTOR");
    }

    PlaySound();

}

public void PlaySound()
    {
        audioSource.Play();
    }
}