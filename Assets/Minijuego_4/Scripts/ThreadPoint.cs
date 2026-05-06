using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class ThreadPoint : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ThreadManager manager;
    private Draggable draggable;

    public List<ThreadLine> connections = new List<ThreadLine>();

    private Image img;
    private Color originalColor;

    [Header("Highlight")]
    [SerializeField] private Color selectedColor = new Color(0.4f, 0.2f, 0.2f);
    [SerializeField] private float pulseAmplitude = 0.05f;
    [SerializeField] private float pulseSpeed = 4f;

    private bool isSelected = false;
    private Vector3 originalScale;
    private float offset;

    public void Initialize(ThreadManager tm)
    {
        manager = tm;
    }

    void Start()
    {
        if (manager == null)
            manager = Object.FindFirstObjectByType<ThreadManager>();

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
        if (Input.GetKey(KeyCode.LeftShift) && manager != null)
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
                line.RemoveLine();
        }

        connections.Clear();
    }
}