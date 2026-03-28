using UnityEngine;
using UnityEngine.UI;

public class ThreadLine : MonoBehaviour
{
    public RectTransform pointA;
    public RectTransform pointB;
    public Image line;

    void Update()
    {
        Vector3 dir = pointB.position - pointA.position;
        float dist = dir.magnitude;

        line.rectTransform.sizeDelta = new Vector2(dist, 5f);
        line.rectTransform.position = pointA.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        line.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}