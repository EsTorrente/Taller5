using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class ThreadPoint : MonoBehaviour, IPointerClickHandler
{
    public ThreadManager manager;
    public Draggable draggable;

    public List<ThreadLine> connections = new List<ThreadLine>();

    private Image img;
    private Color originalColor;

    [Header("Highlight")]
    public Color selectedColor = new Color(0.4f, 0.2f, 0.2f); 
    public float pulseAmplitude = 0.05f;
    public float pulseSpeed = 4f;

    private bool isSelected = false;
    private Vector3 originalScale;
    private float offset;

    void Start()
    {
        if (manager == null)
            manager = FindObjectOfType<ThreadManager>();

        draggable = GetComponent<Draggable>();

        img = GetComponent<Image>();
        if (img != null)
            originalColor = img.color;

        originalScale = transform.localScale;
        offset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        if (isSelected)
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed + offset) * pulseAmplitude;
            transform.localScale = originalScale * pulse;
        }
        else
        {
            transform.localScale = originalScale;
        }

        if (!Input.GetKey(KeyCode.LeftShift) && draggable != null)
        {
            draggable.enabled = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            bool shouldHighlight = manager.SelectPoint(this);

            if (shouldHighlight)
                SetSelected(true);

            if (draggable != null)
                draggable.enabled = false;
        }
    }

    public void SetSelected(bool value)
    {
        isSelected = value;

        if (img != null)
            img.color = value ? selectedColor : originalColor;
    }

    public void AddConnection(ThreadLine line)
    {
        if (!connections.Contains(line))
            connections.Add(line);
    }

    public void RemoveConnection(ThreadLine line)
    {
        if (connections.Contains(line))
            connections.Remove(line);
    }

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