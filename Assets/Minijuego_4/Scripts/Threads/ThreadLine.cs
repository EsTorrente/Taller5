using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThreadLine : MonoBehaviour, IPointerClickHandler
{
    //========================================================
    // REFERENCIAS
    //========================================================

    // imagen UI usada como línea visual
    [SerializeField] private Image line;

    // puntos conectados
    private RectTransform pointA;
    private RectTransform pointB;

    // referencias a los scripts de los puntos
    private ThreadPoint pointARef;
    private ThreadPoint pointBRef;

    // referencia al manager para devolver líneas al pool
    private ThreadManager manager;

    //========================================================
    // INICIALIZACIÓN
    //========================================================

    // configura una nueva conexión entre dos puntos
    public void Initialize(
        RectTransform a,
        RectTransform b,
        ThreadManager mgr)
    {
        //----------------------------------------------------
        // GUARDAR REFERENCIAS
        //----------------------------------------------------

        pointA = a;
        pointB = b;

        manager = mgr;

        //----------------------------------------------------
        // OBTENER THREAD POINTS
        //----------------------------------------------------

        pointARef = pointA.GetComponent<ThreadPoint>();
        pointBRef = pointB.GetComponent<ThreadPoint>();

        //----------------------------------------------------
        // REGISTRAR CONEXIONES
        //----------------------------------------------------

        // cada punto guarda referencia a esta línea
        pointARef?.AddConnection(this);
        pointBRef?.AddConnection(this);
    }

    //========================================================
    // UNITY METHODS
    //========================================================

    private void Update()
    {
        UpdateLine();
    }

    //========================================================
    // ACTUALIZACIÓN VISUAL
    //========================================================

    // recalcula posición, tamaño y rotación de la línea
    private void UpdateLine()
    {
        //----------------------------------------------------
        // VALIDACIÓN
        //----------------------------------------------------

        // si alguno de los puntos desaparece, elimino automáticamente la línea
        if (pointA == null || pointB == null)
        {
            RemoveLine();
            return;
        }

        //----------------------------------------------------
        // DIRECCIÓN
        //----------------------------------------------------

        Vector3 dir =
            pointB.position - pointA.position;

        //----------------------------------------------------
        // TAMAÑO
        //----------------------------------------------------

        line.rectTransform.sizeDelta =
            new Vector2(dir.magnitude, 5f);

        //----------------------------------------------------
        // POSICIÓN
        //----------------------------------------------------

        line.rectTransform.position =
            (pointA.position + pointB.position) / 2f;

        //----------------------------------------------------
        // ROTACIÓN
        //----------------------------------------------------

        float angle =
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        line.rectTransform.rotation =
            Quaternion.Euler(0, 0, angle);
    }

    //========================================================
    // ELIMINACIÓN
    //========================================================

    // quita referencias mutuas y devuelve la línea al pool
    public void RemoveLine()
    {
        //----------------------------------------------------
        // REMOVER REFERENCIAS
        //----------------------------------------------------

        pointARef?.RemoveConnection(this);
        pointBRef?.RemoveConnection(this);

        //----------------------------------------------------
        // DEVOLVER AL POOL
        //----------------------------------------------------

        if (manager != null) // para que sí funcione con el test, porque en las pruebas me daba error por no tener manager
        {
            manager.ReturnLineToPool(this);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    //========================================================
    // INPUT
    //========================================================

    // permite eliminar líneas con click derecho
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button ==
            PointerEventData.InputButton.Right)
        {
            RemoveLine();
        }
    }
}