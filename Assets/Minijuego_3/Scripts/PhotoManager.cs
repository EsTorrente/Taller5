using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoManager : MonoBehaviour
{
    [Header("Controladores Inyectados")]
    [SerializeField] private PhotoInteraction interactionController;
    [SerializeField] private PhotoAnimator animatorController;
    [SerializeField] private ClueFactory clueFactory; // <-- AÑADIDO: Referencia a la fábrica

    [Header("Datos y Referencias de UI")]
    [SerializeField] private List<PhotoData> photos;
    [SerializeField] private Image photoDisplay;
    
    // <-- ELIMINADO: [SerializeField] private List<ClueUI> clueSlots;
    // <-- AÑADIDO: Lista interna para rastrear las pistas creadas por la fábrica
    private List<ClueUI> activeClues = new List<ClueUI>(); 

    private bool currentPhotoSolved = false;
    private HashSet<int> solvedPhotos = new HashSet<int>();
    private int index = 0;

    public static event Action<string> OnPhotoSolved;

    void Start()
    {
        UpdatePhoto();
        animatorController.PlayInitialAnimation();
    }

    void Update()
    {
        interactionController.IsInteractionEnabled = !animatorController.IsAnimating;
    }

    public void NextPhoto()
    {
        animatorController.PlayTransition(1, () => {
            index = (index + 1 + photos.Count) % photos.Count;
            UpdatePhoto();
        });
    }

    public void PreviousPhoto()
    {
        animatorController.PlayTransition(-1, () => {
            index = (index - 1 + photos.Count) % photos.Count;
            UpdatePhoto();
        });
    }

    void UpdatePhoto()
    {
        // 1. LIMPIEZA: Eliminamos las pistas instanciadas de la foto anterior
        foreach (var clue in activeClues)
        {
            if (clue != null) Destroy(clue.gameObject);
        }
        activeClues.Clear();

        // 2. ACTUALIZACIÓN DE FOTO:
        PhotoData currentPhoto = photos[index];
        photoDisplay.sprite = currentPhoto.photoSprite;
        currentPhotoSolved = solvedPhotos.Contains(index);

        // 3. FABRICACIÓN: Creamos dinámicamente las pistas que necesita esta foto en particular
        foreach (ClueData clueData in currentPhoto.clues)
        {
            ClueUI newClue = clueFactory.CreateClue(clueData, this);
            activeClues.Add(newClue);
        }
    }

    public bool IsCurrentPhotoSolved()
    {
        return currentPhotoSolved;
    }

    public void CheckPhotoCompletion()
    {
        if (currentPhotoSolved) return;

        // Comprobamos la lista dinámica en lugar de los slots fijos
        foreach (var clue in activeClues)
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

        string clue = photos[index].finalClue;
        Debug.Log("Foto resuelta. Pista desbloqueada: " + clue);

        OnPhotoSolved?.Invoke(clue); 
    }
}