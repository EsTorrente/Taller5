using UnityEngine;
using UnityEngine.EventSystems;

public class NotepadHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public NotepadUI notepad;

    public void OnPointerEnter(PointerEventData eventData)
    {
        notepad.Show();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        notepad.Hide();
    }
}