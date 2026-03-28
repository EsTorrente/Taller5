using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ThreadClickCatcher : MonoBehaviour
{
    void Update()
    {
        if (!Input.GetMouseButtonDown(1)) return;

        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        foreach (var hit in results)
        {
            //  borrar hilo
            ThreadLine thread = hit.gameObject.GetComponent<ThreadLine>();
            if (thread != null)
            {
                Destroy(thread.gameObject);
                return;
            }

            // buscar post-it
            Transform t = hit.gameObject.transform;

            while (t != null)
            {
                if (t.CompareTag("PostIt"))
                {
                    ThreadPoint point = t.GetComponent<ThreadPoint>();

                    if (point != null)
                        point.ClearConnections();

                    Destroy(t.gameObject);
                    return;
                }

                t = t.parent;
            }
        }
    }
}