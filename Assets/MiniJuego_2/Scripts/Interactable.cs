using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interactable : MonoBehaviour
{
    public GameObject[] objetosAbrir;
    public GameObject[] objetosCerrar;

    private bool ignorarPrimerClick = true;
    private Collider2D col;

    void OnEnable()
    {
        ignorarPrimerClick = true;
    }

    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    void OnMouseDown()
    {
        // ignorar clics sobre UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        foreach (GameObject obj in objetosAbrir)
            if (obj != null) obj.SetActive(true);

        foreach (GameObject obj in objetosCerrar)
            if (obj != null) obj.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // iignorar clics sobre UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (ignorarPrimerClick)
            {
                ignorarPrimerClick = false;
                return;
            }

            if (col == null) return;

            Vector2 puntoClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //si el clic fue fuera del collider,cerrar
            if (!col.OverlapPoint(puntoClick))
            {
                foreach (GameObject obj in objetosAbrir)
                    if (obj != null) obj.SetActive(false);

                foreach (GameObject obj in objetosCerrar)
                    if (obj != null) obj.SetActive(true);
            }
        }
    }
}