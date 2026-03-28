using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject[] objetosAbrir;
    public GameObject[] objetosCerrar;

    private bool ignorarPrimerClick = true;

    void OnEnable()
    {
        ignorarPrimerClick = true;
    }

    void OnMouseDown()
    {
        foreach (GameObject obj in objetosAbrir)
            if (obj != null) obj.SetActive(true);

        foreach (GameObject obj in objetosCerrar)
            if (obj != null) obj.SetActive(false);
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
            Collider2D col = GetComponent<Collider2D>();

            if (col != null && !col.OverlapPoint(puntoClick))
            {
                foreach (GameObject obj in objetosAbrir)
                    if (obj != null) obj.SetActive(false);

                foreach (GameObject obj in objetosCerrar)
                    if (obj != null) obj.SetActive(true);
            }
        }
    }
}