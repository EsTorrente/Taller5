using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    [SerializeField] private GameObject linePrefab;
    private ThreadPoint firstPoint;

    // pool
    private Queue<ThreadLine> linePool = new Queue<ThreadLine>();

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

    private void CreateLine(ThreadPoint a, ThreadPoint b)
    {
        ThreadLine line = GetLineFromPool();

        line.Initialize(a.GetComponent<RectTransform>(), b.GetComponent<RectTransform>(), this);
    }

    private ThreadLine GetLineFromPool()
    {
        if (linePool.Count > 0)
        {
            ThreadLine line = linePool.Dequeue();
            line.gameObject.SetActive(true);
            return line;
        }

        GameObject obj = Instantiate(linePrefab, transform);
        return obj.GetComponent<ThreadLine>();
    }

    public void ReturnLineToPool(ThreadLine line)
    {
        line.gameObject.SetActive(false);
        linePool.Enqueue(line);
    }
}