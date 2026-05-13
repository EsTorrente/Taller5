using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class ThreadManager : MonoBehaviour
{
    //========================================================
    // PREFABS
    //========================================================

    [SerializeField] private GameObject linePrefab;

    //========================================================
    // ESTADO
    //========================================================

    // primer punto seleccionado al crear una conexión
    private ThreadPoint firstPoint;

    //========================================================
    // OBJECT POOL
    //========================================================

    // reutiliza líneas destruidas para evitar instanciación/destrucción constante
    private Queue<ThreadLine> linePool = new();

    //========================================================
    // SELECCIÓN DE PUNTOS
    //========================================================

    // maneja el flujo de selección de dos puntos
    public bool SelectPoint(ThreadPoint point)
    {
        //----------------------------------------------------
        // PRIMER CLICK
        //----------------------------------------------------

        if (firstPoint == null)
        {
            firstPoint = point;

            return true;
        }

        //----------------------------------------------------
        // SEGUNDO CLICK
        //----------------------------------------------------

        CreateLine(firstPoint, point);

        firstPoint.SetSelected(false);
        point.SetSelected(false);

        firstPoint = null;

        return false;
    }

    //========================================================
    // CREACIÓN DE HILOS
    //========================================================

    // crea una línea entre dos puntos
    private void CreateLine(ThreadPoint a, ThreadPoint b)
    {
        ThreadLine line = GetLineFromPool();

        line.Initialize(
            a.GetComponent<RectTransform>(),
            b.GetComponent<RectTransform>(),
            this
        );
    }

    //========================================================
    // POOL
    //========================================================

    // obtiene una línea reutilizable del pool
    private ThreadLine GetLineFromPool()
    {
        //----------------------------------------------------
        // REUTILIZAR
        //----------------------------------------------------

        if (linePool.Count > 0)
        {
            ThreadLine line = linePool.Dequeue();

            line.gameObject.SetActive(true);

            return line;
        }

        //----------------------------------------------------
        // CREAR NUEVA
        //----------------------------------------------------

        GameObject obj = Instantiate(linePrefab, transform);

        return obj.GetComponent<ThreadLine>();
    }

    // devuelve una línea al pool
    public void ReturnLineToPool(ThreadLine line)
    {
        line.gameObject.SetActive(false);

        linePool.Enqueue(line);
    }
}