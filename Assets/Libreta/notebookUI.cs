using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class notebookUI : MonoBehaviour
{
    public GameObject notebookPanel;
    public TMP_InputField inputField;
    public Animator animator;

    [Header("Botón")]
    public Image buttonImage;
    public Sprite arrowUp;
    public Sprite arrowDown;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openSFX;
    public AudioClip closeSFX;

    private bool isOpen = false;

    void Start()
    {
        //cargar texto guardado
        inputField.text = PlayerPrefs.GetString("NotebookText", "");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleNotebook();
        }
    }

    public void ToggleNotebook()
    {
        PlaySFX();
        isOpen = !isOpen;

        animator.SetBool("isOpen", isOpen);

        notebookPanel.SetActive(isOpen);
        StartCoroutine(FocusInputField());

        UpdateButtonSprite();
    }

    IEnumerator FocusInputField() //porque se estaba seleccionando el texto al volver a activar :(
    {
        yield return null;

        inputField.ActivateInputField();

        inputField.caretPosition = inputField.text.Length;
        inputField.selectionAnchorPosition = inputField.text.Length;
        inputField.selectionFocusPosition = inputField.text.Length;
    }

    void UpdateButtonSprite()
    {
        buttonImage.sprite = isOpen ? arrowDown : arrowUp;
    }

    void PlaySFX()
    {
        audioSource.PlayOneShot(isOpen ? openSFX : closeSFX);
    }

    public void SaveText()
    {
        PlayerPrefs.SetString("NotebookText", inputField.text);
        PlayerPrefs.Save();
    }

    public void AddClue(string clue)
    {
        if (inputField.text.Contains(clue))
            return;

        inputField.text += "\n• " + clue;

        SaveText();
    }
}