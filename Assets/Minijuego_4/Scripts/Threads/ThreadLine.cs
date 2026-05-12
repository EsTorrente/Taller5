using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThreadLine : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image line;

    private RectTransform pointA;
    private RectTransform pointB;
    private ThreadPoint pointARef;
    private ThreadPoint pointBRef;
    private ThreadManager manager;

    public void Initialize(RectTransform a, RectTransform b, ThreadManager mgr)
    {
        pointA = a;
        pointB = b;
        manager = mgr;

        pointARef = pointA.GetComponent<ThreadPoint>();
        pointBRef = pointB.GetComponent<ThreadPoint>();

        pointARef?.AddConnection(this);
        pointBRef?.AddConnection(this);
    }

    void Update()
    {
        if (pointA == null || pointB == null)
        {
            RemoveLine();
            return;
        }

        Vector3 dir = pointB.position - pointA.position;
        line.rectTransform.sizeDelta = new Vector2(dir.magnitude, 5f);
        line.rectTransform.position = (pointA.position + pointB.position) / 2f;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        line.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void RemoveLine()
    {
        pointARef?.RemoveConnection(this);
        pointBRef?.RemoveConnection(this);
        manager.ReturnLineToPool(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            RemoveLine();
        }
    }
}