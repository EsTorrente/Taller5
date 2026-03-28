using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ThreadPoint : MonoBehaviour, IPointerClickHandler
{
    public ThreadManager manager;
    public Draggable draggable;

    public List<ThreadLine> connections = new List<ThreadLine>();

    void Start()
    {
        if (manager == null)
        {
            manager = FindObjectOfType<ThreadManager>();
        }

        draggable = GetComponent<Draggable>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            manager.SelectPoint(GetComponent<RectTransform>());

            if (draggable != null)
                draggable.enabled = false;
        }
    }

    void Update()
    {
        if (!Input.GetKey(KeyCode.LeftShift) && draggable != null)
        {
            draggable.enabled = true;
        }
    }

    // agregar conexión
    public void AddConnection(ThreadLine line)
    {
        if (!connections.Contains(line))
            connections.Add(line);
    }

    // remover conexión
    public void RemoveConnection(ThreadLine line)
    {
        if (connections.Contains(line))
            connections.Remove(line);
    }

    // borrar todas las conexiones
    public void ClearConnections()
    {
        var copy = new List<ThreadLine>(connections);

        foreach (var line in copy)
        {
            if (line != null)
                Destroy(line.gameObject);
        }

        connections.Clear();
    }
}