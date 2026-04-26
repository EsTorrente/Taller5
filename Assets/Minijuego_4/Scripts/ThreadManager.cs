using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    public GameObject linePrefab;

    private ThreadPoint firstPoint;

    public bool SelectPoint(ThreadPoint point)
    {
        if (firstPoint == null)
        {
            firstPoint = point;
            return true;
        }
        else
        {
            CreateLine(firstPoint, point);

            firstPoint.SetSelected(false);
            point.SetSelected(false);

            firstPoint = null;
            return false;
        }
    }

    void CreateLine(ThreadPoint a, ThreadPoint b)
    {
        GameObject obj = Instantiate(linePrefab, transform);

        ThreadLine line = obj.GetComponent<ThreadLine>();
        line.pointA = a.GetComponent<RectTransform>();
        line.pointB = b.GetComponent<RectTransform>();

        a.AddConnection(line);
        b.AddConnection(line);
    }
}