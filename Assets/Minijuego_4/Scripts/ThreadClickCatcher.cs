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

            ThreadLine thread = hit.gameObject.GetComponent<ThreadLine>();
            if (thread != null)
            {
                Destroy(thread.gameObject);
                return;
            }


            Transform t = hit.gameObject.transform;

            while (t != null)
            {
                if (t.CompareTag("PostIt"))
                {
                    ThreadPoint point = t.GetComponent<ThreadPoint>();

                    if (point != null)
                        point.ClearConnections();

                    if (StickerManager.Instance != null)
                    {
                        StickerManager.Instance.RemoveStickersFromParent(t);
                    }


                    Destroy(t.gameObject);
                    return;
                }

                t = t.parent;
            }
        }
    }
}