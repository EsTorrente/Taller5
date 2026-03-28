using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class NotepadUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public TextMeshProUGUI text;

    private List<string> notes = new List<string>();

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void Show()
    {
        if (panel != null)
            panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void AddNote(string note)
    {
        if (!notes.Contains(note))
        {
            notes.Add(note);
            RefreshText();
        }
    }

    void RefreshText()
    {
        text.text = "";

        foreach (var note in notes)
        {
            text.text += "• " + note + "\n";
        }
    }
}