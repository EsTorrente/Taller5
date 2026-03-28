using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThreadLine : MonoBehaviour, IPointerClickHandler
{
    public RectTransform pointA;
    public RectTransform pointB;
    public Image line;

    private ThreadPoint pointARef;
    private ThreadPoint pointBRef;

    void Start()
    {
        pointARef = pointA.GetComponent<ThreadPoint>();
        pointBRef = pointB.GetComponent<ThreadPoint>();

        pointARef?.AddConnection(this);
        pointBRef?.AddConnection(this);
    }

    void Update()
    {
        if (pointA == null || pointB == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = pointB.position - pointA.position;
        float distance = dir.magnitude;

        line.rectTransform.sizeDelta = new Vector2(distance, 5f);
        line.rectTransform.position = (pointA.position + pointB.position) / 2f;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        line.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnDestroy()
    {
        pointARef?.RemoveConnection(this);
        pointBRef?.RemoveConnection(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Destroy(gameObject);
        }
    }
}