using System;
using System.Collections;
using UnityEngine;

public class PhotoAnimator : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform photoContainer; 

    [Header("Configuración de Animación")]
    [SerializeField] private float animationDuration = 0.5f; 
    [SerializeField] private float slideOffset = 1000f; 

    // Saber si se está animando para bloquear otras acciones
    public bool IsAnimating { get; private set; } 

    public void PlayInitialAnimation()
    {
        StartCoroutine(InitialEnterAnimation(slideOffset));
    }

    public void PlayTransition(int indexStep, Action onMidTransition)
    {
        if (IsAnimating) return;
        
        float targetExitX = indexStep > 0 ? -slideOffset : slideOffset;
        float startEntryX = indexStep > 0 ? slideOffset : -slideOffset;
        
        StartCoroutine(TransitionRoutine(targetExitX, startEntryX, onMidTransition));
    }

    private IEnumerator TransitionRoutine(float targetExitX, float startEntryX, Action onMidTransition)
    {
        IsAnimating = true;
        float halfDuration = animationDuration / 2f; 
        float currentX = photoContainer.localPosition.x; 
        float originalY = photoContainer.localPosition.y;
        float originalZ = photoContainer.localPosition.z;
        float timeElapsed = 0f;

        while (timeElapsed < halfDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / halfDuration);
            float easeInT = t * t * t; 
            
            float newX = Mathf.Lerp(currentX, targetExitX, easeInT);
            photoContainer.localPosition = new Vector3(newX, originalY, originalZ);
            yield return null; 
        }

        photoContainer.localPosition = new Vector3(startEntryX, 0f, originalZ); 
        photoContainer.localScale = Vector3.one; 
        
        onMidTransition?.Invoke();

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
        IsAnimating = false;
    }

    private IEnumerator InitialEnterAnimation(float startX)
    {
        IsAnimating = true;
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
        IsAnimating = false;
    }
}