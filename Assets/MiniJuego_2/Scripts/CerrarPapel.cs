using UnityEngine;

public class CerrarPapel : MonoBehaviour
{
    public GameObject papelCerrado;
    private bool ignorarPrimerClick = true;

    void OnEnable()
    {
        ignorarPrimerClick = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ignorarPrimerClick)
            {
                ignorarPrimerClick = false;
                return;
            }

            Vector2 puntoClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!GetComponent<Collider2D>().OverlapPoint(puntoClick))
            {
                gameObject.SetActive(false);
                papelCerrado.SetActive(true);
            }
        }
    }
}