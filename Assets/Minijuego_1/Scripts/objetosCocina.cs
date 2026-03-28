using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objetosCocina : MonoBehaviour
{

    [Header("Configuración del bloque")]
    public int gridCol;       // columna inicial (0-3)
    public int gridRow;       // fila inicial (0-3)
    public int widthInSlots;  // ancho en slots: escala 4 = 2 slots, escala 2 = 1 slot
    public int heightInSlots; // alto en slots: escala 2 = 1 slot, escala 4 = 2 slots
    public bool isWinBlock;   // ¿es el bloque amarillo ganador?

    [HideInInspector] public bool isSelected = false;

    void Awake()
    {
        // widthInSlots  = (int)(transform.localScale.x / 2)
        // heightInSlots = (int)(transform.localScale.y / 2)
        // Puedes calcularlo automáticamente así:
        widthInSlots = Mathf.RoundToInt(transform.localScale.x / 2);
        heightInSlots = Mathf.RoundToInt(transform.localScale.y / 2);
    }
}

