using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoManager : MonoBehaviour
{
    [Header("Controladores Inyectados")]
    [SerializeField] private PhotoInteraction interactionController;
    [SerializeField] private PhotoAnimator animatorController;
    [SerializeField] private ClueFactory clueFactory;

    [Header("Datos y Referencias de UI")]
    [SerializeField] private List<PhotoData> photos; 
    [SerializeField] private Image photoDisplay;
    
    private List<ClueUI> activeClues = new List<ClueUI>();

    private bool currentPhotoSolved = false;
    private HashSet<int> solvedPhotos = new HashSet<int>();
    private int index = 0;

    public static event Action<string> OnPhotoSolved; // ESTE ES EL QUE GRITA QUE SE RESOLVIO UNA FOTO RAHHHHHHHHHHHHHHHHHHHHHHH

    void Start()
    {
        UpdatePhoto();
        animatorController.PlayInitialAnimation();
    }

    void Update()
    {
        interactionController.IsInteractionEnabled = !animatorController.IsAnimating; //mira a ver si está animando y bloquea el mouse
    }

    public void NextPhoto()
    {
        animatorController.PlayTransition(1, () => { //hace toda la animación creisi
            index = (index + 1 + photos.Count) % photos.Count; //pa mirar en cual de las fotos va
            UpdatePhoto(); //arma la foto
        });
    }

    public void PreviousPhoto() //literalmente lo mismo pero de pa atras
    {
        animatorController.PlayTransition(-1, () => { 
            index = (index - 1 + photos.Count) % photos.Count;
            UpdatePhoto();
        });
    }

    void UpdatePhoto() // carga la parte visual
    {
        foreach (var clue in activeClues) // recorre las pistas de la foto anterior
        {
            if (clue != null) Destroy(clue.gameObject); //las borra
        }
        activeClues.Clear(); //chaoooooooooo
        
        PhotoData currentPhoto = photos[index]; // saca los datos de la foto que toca
        photoDisplay.sprite = currentPhoto.photoSprite; //pega la imagen en la UI
        currentPhotoSolved = solvedPhotos.Contains(index); // revisa a ver si ya esta resuelta

        foreach (ClueData clueData in currentPhoto.clues) // recorre la lista de pistas de esta foto
        {
            ClueUI newClue = clueFactory.CreateClue(clueData, this); // Le manda a la fabrica crearla y le pasa los datos
            activeClues.Add(newClue); // guarda el clon nuevo en su lista
        }
    }

    public bool IsCurrentPhotoSolved() // pa que otros scripts pregunten
    {
        return currentPhotoSolved; // devuelve si esta foto ya está
    }

    public void CheckPhotoCompletion() // se llama cada vez que haces clic en una pista
    {
        if (currentPhotoSolved) return; // si ya está resuelta ignora todo

        foreach (var clue in activeClues) // revisa todas las pistas activas
        {
            if (!clue.IsCorrect()) //si una pista está mal pues entonces no hace nada
                return;
        }

        SolveCurrentPhoto(); //pa decirle q todo bien q la foto está resuelta
    }

    void SolveCurrentPhoto()
    {
        currentPhotoSolved = true;
        solvedPhotos.Add(index);

        string clue = photos[index].finalClue;

        OnPhotoSolved?.Invoke(clue); //ACA ES DONDE GRITA QUE ESTA FOTO SE RESOLVIO WOOOOOOOOOO
    }
}