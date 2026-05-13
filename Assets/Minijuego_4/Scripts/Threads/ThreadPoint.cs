using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class ThreadPoint : MonoBehaviour, IPointerClickHandler
{
    //========================================================
    // REFERENCIAS
    //========================================================

    [SerializeField] private ThreadManager manager;

    // referencia al script de drag del mismo objeto
    private Draggable draggable;

    // componente visual usado para cambiar color
    private Image img;

    //========================================================
    // CONEXIONES
    //========================================================

    // lista de todos los hilos conectados a este punto
    private List<ThreadLine> connections = new();

    // permite leer conexiones desde otros scripts sin permitir modificar la lista directamente
    public IReadOnlyList<ThreadLine> Connections => connections;

    //========================================================
    // CONFIGURACIÓN VISUAL
    //========================================================

    [Header("Highlight")]

    // color temporal mientras el punto está seleccionado
    [SerializeField]
    private Color selectedColor =
        new Color(0.4f, 0.2f, 0.2f);

    // intensidad del pulso visual
    [SerializeField] private float pulseAmplitude = 0.05f;

    // velocidad del pulso visual
    [SerializeField] private float pulseSpeed = 4f;

    //========================================================
    // ESTADO
    //========================================================

    // color original del objeto
    private Color originalColor;

    // indica si el punto está seleccionado
    private bool isSelected = false;

    // escala inicial del objeto
    private Vector3 originalScale;

    // offset aleatorio para variar el pulso
    private float offset;

    //========================================================
    // INICIALIZACIÓN
    //========================================================

    // permite inyectar el manager luego de instanciar
    public void Initialize(ThreadManager tm)
    {
        manager = tm;
    }

    //========================================================
    // UNITY METHODS
    //========================================================

    private void Start()
    {
        //----------------------------------------------------
        // BUSCAR MANAGER
        //----------------------------------------------------

        if (manager == null)
        {
            manager = Object.FindFirstObjectByType<ThreadManager>();
        }

        //----------------------------------------------------
        // REFERENCIAS
        //----------------------------------------------------

        draggable = GetComponent<Draggable>();

        img = GetComponent<Image>();

        //----------------------------------------------------
        // GUARDAR ESTADO VISUAL
        //----------------------------------------------------

        if (img != null)
        {
            originalColor = img.color;
        }

        originalScale = transform.localScale;

        // offset aleatorio para evitar pulso sincronizado
        offset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        ApplySelectionPulse();
        RestoreDrag();
    }

    //========================================================
    // EFECTO VISUAL
    //========================================================

    // genera un pequeño pulso mientras el punto está seleccionado para crear conexiones
    private void ApplySelectionPulse()
    {
        if (isSelected)
        {
            float pulse =
                1f + Mathf.Sin(Time.time * pulseSpeed + offset)
                * pulseAmplitude;

            transform.localScale = originalScale * pulse;
        }
        else
        {
            transform.localScale = originalScale;
        }
    }

    //========================================================
    // DRAG
    //========================================================

    // vuelve a activar drag cuando se deja de presionar shift
    private void RestoreDrag()
    {
        if (!Input.GetKey(KeyCode.LeftShift) &&
            draggable != null)
        {
            draggable.enabled = true;
        }
    }

    //========================================================
    // INPUT
    //========================================================

    // selecciona puntos para crear conexiones
    public void OnPointerClick(PointerEventData eventData)
    {
        //----------------------------------------------------
        // VALIDACIÓN
        //----------------------------------------------------

        // solo funciona mientras shift está presionado
        if (!Input.GetKey(KeyCode.LeftShift) ||
            manager == null)
            return;

        //----------------------------------------------------
        // SELECCIÓN
        //----------------------------------------------------

        bool shouldHighlight =
            manager.SelectPoint(this);

        //----------------------------------------------------
        // FEEDBACK VISUAL
        //----------------------------------------------------

        if (shouldHighlight)
        {
            SetSelected(true);
        }

        //----------------------------------------------------
        // DESACTIVAR DRAG TEMPORALMENTE
        //----------------------------------------------------

        // evita mover el post-it mientras se crean conexiones
        if (draggable != null)
        {
            draggable.enabled = false;
        }
    }

    //========================================================
    // ESTADO VISUAL
    //========================================================

    // activa o desactiva highlight visual
    public void SetSelected(bool value)
    {
        isSelected = value;

        if (img != null)
        {
            img.color =
                value ? selectedColor : originalColor;
        }
    }

    //========================================================
    // CONEXIONES
    //========================================================

    // agrega una línea a la lista de conexiones
    public void AddConnection(ThreadLine line)
    {
        if (!connections.Contains(line))
        {
            connections.Add(line);
        }
    }

    // elimina una línea de la lista de conexiones
    public void RemoveConnection(ThreadLine line)
    {
        if (connections.Contains(line))
        {
            connections.Remove(line);
        }
    }

    // elimina todas las conexiones del punto
    public void ClearConnections()
    {
        //----------------------------------------------------
        // COPIA DE SEGURIDAD
        //----------------------------------------------------

        // hago una copia porque RemoveLine modifica la lista
        var copy = new List<ThreadLine>(connections);

        //----------------------------------------------------
        // ELIMINAR HILOS
        //----------------------------------------------------

        foreach (ThreadLine line in copy)
        {
            if (line != null)
            {
                line.RemoveLine();
            }
        }

        //----------------------------------------------------
        // LIMPIAR LISTA
        //----------------------------------------------------

        connections.Clear();
    }
}