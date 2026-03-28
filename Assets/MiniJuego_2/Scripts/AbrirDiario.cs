using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbrirDiario : MonoBehaviour
{
    public DiarioManager diarioManager;

    void OnMouseDown()
    {
        diarioManager.AbrirDiario();
        gameObject.SetActive(false);
    }
}
