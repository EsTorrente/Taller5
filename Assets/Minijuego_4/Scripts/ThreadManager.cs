using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    public GameObject linePrefab;

    private RectTransform first;

    public void SelectPoint(RectTransform point)
    {
        if (first == null)
        {
            first = point;
        }
        else
        {
            CreateLine(first, point);
            first = null;
        }
    }

    void CreateLine(RectTransform a, RectTransform b)
    {
        GameObject obj = Instantiate(linePrefab, transform);

        ThreadLine line = obj.GetComponent<ThreadLine>();
        line.pointA = a;
        line.pointB = b;
    }
}