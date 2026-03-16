using UnityEngine;

public class PapelController : MonoBehaviour
{
    public GameObject papelAbierto;

    void OnMouseDown()
    {
        papelAbierto.SetActive(true);
        gameObject.SetActive(false);
    }
}