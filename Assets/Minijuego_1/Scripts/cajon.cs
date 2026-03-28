using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cajon : MonoBehaviour
{

    [Header("Configuración del tablero")]
    public int columns = 4;
    public int rows = 4;
    public float slotSize = 2f;          // cada slot mide 2 unidades Unity
    public Transform boardOrigin;        // esquina inferior-izquierda del tablero

    [Header("Salida para ganar")]
    // El amarillo sale por la DERECHA, en las filas 2-3 (mitad inferior)
    // Define la columna de salida: col >= 4 = fuera del tablero
    public int exitColumn = 4;
    public int exitRowMin = 2;
    public int exitRowMax = 3;

    [Header("UI")]
    public GameObject winPanel;

    private objetosCocina selectedBlock;
    private Vector2 dragStart;
    private bool gameWon = false;

    // Cuadrícula lógica: null = vacío, referencia = bloque que ocupa ese slot
    private objetosCocina[,] grid;
    void Awake()
    {
        grid = new objetosCocina[columns, rows];
    }
    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        RegisterAllBlocks();
        PositionAllBlocks();
    }

    // Registra todos los bloques en la cuadrícula lógica
    void RegisterAllBlocks()
    {
        grid = new objetosCocina[columns, rows];
        objetosCocina[] blocks = FindObjectsOfType<objetosCocina>();
        foreach (var block in blocks)
        {
            RegisterBlock(block);
        }
    }

    void PositionAllBlocks()
    {
        objetosCocina[] blocks = FindObjectsOfType<objetosCocina>();
        foreach (var block in blocks)
        {
            Vector3 pos = GridToWorldCentered(
                block.gridCol, block.gridRow,
                block.widthInSlots, block.heightInSlots);
            block.transform.position = pos;
        }
    }
    void RegisterBlock(objetosCocina block)
    {
        for (int c = block.gridCol; c < block.gridCol + block.widthInSlots; c++)
            for (int r = block.gridRow; r < block.gridRow + block.heightInSlots; r++)
                if (c >= 0 && c < columns && r >= 0 && r < rows)
                    grid[c, r] = block;
    }

    void UnregisterBlock(objetosCocina block)
    {
        for (int c = 0; c < columns; c++)
            for (int r = 0; r < rows; r++)
                if (grid[c, r] == block)
                    grid[c, r] = null;
    }

    // Convierte posición de grid a posición en el mundo
    public Vector3 GridToWorld(int col, int row)
    {
        return boardOrigin.position
             + new Vector3(col * slotSize + slotSize * 0.5f,
                           row * slotSize + slotSize * 0.5f, 0f);
        // slotSize * 0.5f centra el bloque 1x1 en su slot.
        // Para bloques más grandes el centro se ajusta sumando la mitad extra:
        // Se ajusta en MoveBlockTo()
    }

    public Vector3 GridToWorldCentered(int col, int row, int wSlots, int hSlots)
    {
        return boardOrigin.position
             + new Vector3(col * slotSize + (wSlots * slotSize) * 0.5f,
                           row * slotSize + (hSlots * slotSize) * 0.5f, 0f);
    }

    void Update()
    {
        if (gameWon) return;

        // --- Input: Mouse o Touch ---
        if (Input.GetMouseButtonDown(0))
            OnPressDown(Input.mousePosition);

        if (Input.GetMouseButton(0) && selectedBlock != null)
            OnPressDrag(Input.mousePosition);

        if (Input.GetMouseButtonUp(0) && selectedBlock != null)
            OnPressUp(Input.mousePosition);

        // Touch
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) OnPressDown(t.position);
            if (t.phase == TouchPhase.Moved) OnPressDrag(t.position);
            if (t.phase == TouchPhase.Ended) OnPressUp(t.position);
        }
    }

    void OnPressDown(Vector2 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z));

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null)
        {
            objetosCocina block = hit.collider.GetComponent<objetosCocina>();
            if (block == null)
                block = hit.collider.GetComponentInParent<objetosCocina>();

            if (block != null)
            {
                selectedBlock = block;
                dragStart = worldPos;
            }
        }
    }

    void OnPressDrag(Vector2 screenPos)
    {
        // Visual feedback opcional: mover el sprite ligeramente mientras arrastra
        // (no es necesario para la lógica, se aplica al soltar)
    }

    void OnPressUp(Vector2 screenPos)
    {
        if (selectedBlock == null) return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z));

        Vector2 delta = (Vector2)worldPos - dragStart;

        // Determinar dirección dominante del swipe
        int dc = 0, dr = 0;
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            dc = delta.x > 0 ? 1 : -1;   // horizontal
        else
            dr = delta.y > 0 ? 1 : -1;   // vertical

        // Mínimo de arrastre para considerar movimiento
        float minDrag = slotSize * 0.3f;
        if (Mathf.Abs(delta.x) < minDrag && Mathf.Abs(delta.y) < minDrag)
        {
            selectedBlock = null;
            return;
        }

        TryMove(selectedBlock, dc, dr);
        selectedBlock = null;
    }

    // Intenta mover el bloque 1 slot en la dirección (dc, dr)
    void TryMove(objetosCocina block, int dc, int dr)
    {
        int newCol = block.gridCol + dc;
        int newRow = block.gridRow + dr;

        // --- Caso especial: bloque ganador sale por la derecha ---
        if (block.isWinBlock && dc == 1)
        {
            // Verificar que el bloque está en los rows de salida
            bool enFilasSalida = (block.gridRow >= exitRowMin &&
                                  block.gridRow + block.heightInSlots - 1 <= exitRowMax);
            if (enFilasSalida && newCol + block.widthInSlots > columns)
            {
                // ¡El bloque sale del tablero!
                WinGame(block);
                return;
            }
        }

        // --- Validar límites del tablero ---
        if (newCol < 0 || newCol + block.widthInSlots > columns) return;
        if (newRow < 0 || newRow + block.heightInSlots > rows) return;

        // --- Verificar que los slots destino estén libres ---
        if (!CanMoveTo(block, newCol, newRow)) return;

        // --- Ejecutar movimiento ---
        UnregisterBlock(block);
        block.gridCol = newCol;
        block.gridRow = newRow;
        RegisterBlock(block);

        // Mover visualmente con animación
        Vector3 targetPos = GridToWorldCentered(newCol, newRow, block.widthInSlots, block.heightInSlots);
        StartCoroutine(AnimateMove(block.transform, targetPos));
    }

    bool CanMoveTo(objetosCocina block, int newCol, int newRow)
    {
        for (int c = newCol; c < newCol + block.widthInSlots; c++)
        {
            for (int r = newRow; r < newRow + block.heightInSlots; r++)
            {
                // Slot fuera del tablero
                if (c < 0 || c >= columns || r < 0 || r >= rows) return false;

                // Slot ocupado por OTRO bloque
                if (grid[c, r] != null && grid[c, r] != block) return false;
            }
        }
        return true;
    }

    IEnumerator AnimateMove(Transform t, Vector3 target)
    {
        float duration = 0.12f;
        float elapsed = 0f;
        Vector3 start = t.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            t.position = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }
        t.position = target;
    }

    void WinGame(objetosCocina block)
    {
        gameWon = true;
        StartCoroutine(WinSequence(block));
        winPanel.SetActive(true);
    }

    IEnumerator WinSequence(objetosCocina block)
    {
        // Animación de salida del tablero
        Vector3 exitPos = GridToWorldCentered(exitColumn + 1, block.gridRow,
                                              block.widthInSlots, block.heightInSlots);
        yield return StartCoroutine(AnimateMove(block.transform, exitPos));
        yield return new WaitForSeconds(0.3f);

        if (winPanel != null) winPanel.SetActive(true);
        Debug.Log("¡GANASTE!");
    }
}