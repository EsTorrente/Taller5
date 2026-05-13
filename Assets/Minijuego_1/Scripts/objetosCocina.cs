using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objetosCocina : MonoBehaviour
{

    [Header("Configuración del bloque")]
    public int gridCol;       // columna inicial va de 0-3 (es un 4x4)
    public int gridRow;       // fila inicial va de 0-3
    public int widthInSlots;  // ancho en slots
    public int heightInSlots; // alto en slots
    public bool isWinBlock;   // es la llave

    [HideInInspector] public bool isSelected = false;

    
}

