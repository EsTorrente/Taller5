using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoManager : MonoBehaviour
{
    #region variables
    public List<PhotoData> photos;
    private bool currentPhotoSolved = false;
    private HashSet<int> solvedPhotos = new HashSet<int>();
    public Image photoDisplay;
    public List<ClueUI> clueSlots;
    public notebookUI notepad;
    public AudioSource audioSource;

    [Header("Configuración de Animación")]
    [Tooltip("El Transform del objeto que contiene la foto.")]
    public Transform photoContainer; 
    [Tooltip("Tiempo TOTAL de la transición en segundos (salir + entrar).")]
    public float animationDuration = 0.5f; 
    [Tooltip("Distancia en píxeles para salir/entrar de la pantalla.")]
    public float slideOffset = 1000f; 

    [Header("Configuración de Zoom y Arrastre")]
    [Tooltip("Velocidad a la que se acerca/aleja la foto con la rueda del ratón.")]
    public float zoomSpeed = 2f;
    [Tooltip("Escala mínima (1 = tamaño original).")]
    public float minZoom = 1f;
    [Tooltip("Escala máxima permitida.")]
    public float maxZoom = 4f;
    [Tooltip("Multiplicador de velocidad al arrastrar con el clic izquierdo.")]
    public float dragSpeed = 1f;

    private Coroutine currentAnimation;
    private int index = 0;
    private Vector3 lastMousePosition; 
    public float baseWidth = 1920f;
    public float baseHeight = 1080f;

    #endregion

    void Start()
    {
        UpdatePhoto();
        currentAnimation = StartCoroutine(InitialEnterAnimation(slideOffset));
    }

    void Update()
    {

        if (currentAnimation != null) return;

        HandleZoom();
        HandleDrag();
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll != 0f)
        {
            Vector3 newScale = photoContainer.localScale + Vector3.one * scroll * zoomSpeed;
            newScale.x = Mathf.Clamp(newScale.x, minZoom, maxZoom);
            newScale.y = Mathf.Clamp(newScale.y, minZoom, maxZoom);
            newScale.z = Mathf.Clamp(newScale.z, minZoom, maxZoom); 

            photoContainer.localScale = newScale;

            // Si quitamos el zoom (escala vuelve a 1), forzamos a que la imagen se centre
            if (newScale.x <= minZoom)
            {
                photoContainer.localPosition = new Vector3(0f, 0f, photoContainer.localPosition.z);
            }
            else
            {
                // Al hacer zoom out, puede que la imagen quede fuera de los nuevos límites. 
                // Llamamos a esta función para reajustar su posición.
                ApplyDragLimits();
            }
        }
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            photoContainer.localPosition += delta * dragSpeed;
            ApplyDragLimits();

            lastMousePosition = Input.mousePosition;
        }
    }


    private void ApplyDragLimits()
    {
        float currentScale = photoContainer.localScale.x;
        if (currentScale < 0.5f)
        {
            photoContainer.localPosition = new Vector3(0f, 0f, photoContainer.localPosition.z);
            return;
        }
        float maxLimitX = (baseWidth * (currentScale - 1f)) / 2f;
        float maxLimitY = (baseHeight * (currentScale - 1f)) / 2f;

        Vector3 currentPos = photoContainer.localPosition;
        currentPos.x = Mathf.Clamp(currentPos.x, -maxLimitX, maxLimitX);
        currentPos.y = Mathf.Clamp(currentPos.y, -maxLimitY, maxLimitY);

        photoContainer.localPosition = currentPos;
    }

    public void NextPhoto()
    {
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(TransitionRoutine(-slideOffset, slideOffset, 1));
    }

    public void PreviousPhoto()
    {
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(TransitionRoutine(slideOffset, -slideOffset, -1));
    }

    private IEnumerator TransitionRoutine(float targetExitX, float startEntryX, int indexStep)
    {
        float halfDuration = animationDuration / 2f; 
        float currentX = photoContainer.localPosition.x; 
        float originalY = photoContainer.localPosition.y;
        float originalZ = photoContainer.localPosition.z;
        float timeElapsed = 0f;

        // FASE 1: SALIR
        while (timeElapsed < halfDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / halfDuration);
            float easeInT = t * t * t; 
            
            float newX = Mathf.Lerp(currentX, targetExitX, easeInT);
            photoContainer.localPosition = new Vector3(newX, originalY, originalZ);
            yield return null; 
        }

        // FASE 2: TELETRANSPORTE Y CAMBIO
        photoContainer.localPosition = new Vector3(startEntryX, 0f, originalZ); // Reseteamos 'Y' a 0 para que no quede movida por el arrastre
        photoContainer.localScale = Vector3.one; // Reseteamos el zoom a 1

        index = (index + indexStep + photos.Count) % photos.Count;
        UpdatePhoto();

        // FASE 3: ENTRAR
        timeElapsed = 0f;
        while (timeElapsed < halfDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / halfDuration);
            float easeOutT = 1f - Mathf.Pow(1f - t, 3f); 
            
            float newX = Mathf.Lerp(startEntryX, 0f, easeOutT);
            photoContainer.localPosition = new Vector3(newX, 0f, originalZ);
            yield return null; 
        }

        photoContainer.localPosition = new Vector3(0f, 0f, originalZ);
        currentAnimation = null;
    }

    private IEnumerator InitialEnterAnimation(float startX)
    {
        float timeElapsed = 0f;
        float originalZ = photoContainer.localPosition.z;

        photoContainer.localScale = Vector3.one;

        while (timeElapsed < animationDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / animationDuration);
            float easeOutT = 1f - Mathf.Pow(1f - t, 3f);
            
            float newX = Mathf.Lerp(startX, 0f, easeOutT);
            photoContainer.localPosition = new Vector3(newX, 0f, originalZ);
            yield return null; 
        }

        photoContainer.localPosition = new Vector3(0f, 0f, originalZ);
        currentAnimation = null;
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
            rt.localScale = clue.scale;
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
            notepad.AddClue(clue);
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