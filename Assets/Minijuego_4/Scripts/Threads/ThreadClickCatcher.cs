using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ThreadClickCatcher : MonoBehaviour
{
    //========================================================
    // REFERENCIAS
    //========================================================

    // referencia al manager de stickers, se usa para limpiar stickers antes de destruir post-its
    [SerializeField] private StickerManager stickerManager;

    //========================================================
    // UNITY METHODS
    //========================================================

    private void Start()
    {
        //----------------------------------------------------
        // BUSCAR MANAGER
        //----------------------------------------------------

        if (stickerManager == null)
        {
            stickerManager =
                Object.FindFirstObjectByType<StickerManager>();
        }
    }

    private void Update()
    {
        DetectRightClick();
    }

    //========================================================
    // CLICK DERECHO
    //========================================================

    // detecta clicks derechos sobre elementos UI
    private void DetectRightClick()
    {
        //----------------------------------------------------
        // VALIDACIÓN
        //----------------------------------------------------

        if (!Input.GetMouseButtonDown(1))
            return;

        //----------------------------------------------------
        // CREAR RAYCAST
        //----------------------------------------------------

        PointerEventData data =
            new PointerEventData(EventSystem.current);

        data.position = Input.mousePosition;

        //----------------------------------------------------
        // BUSCAR OBJETOS BAJO EL CURSOR
        //----------------------------------------------------

        List<RaycastResult> results = new();

        EventSystem.current.RaycastAll(data, results);

        //----------------------------------------------------
        // RECORRER RESULTADOS
        //----------------------------------------------------

        foreach (RaycastResult hit in results)
        {
            //------------------------------------------------
            // ELIMINAR HILO
            //------------------------------------------------

            ThreadLine thread =
                hit.gameObject.GetComponent<ThreadLine>();

            if (thread != null)
            {
                thread.RemoveLine();
                return;
            }

            //------------------------------------------------
            // BUSCAR POST-IT PADRE
            //------------------------------------------------

            // sube por la jerarquía buscando un objeto con tag PostIt
            Transform t = hit.gameObject.transform;

            while (t != null)
            {
                if (t.CompareTag("PostIt"))
                {
                    DestroyPostIt(t);
                    return;
                }

                t = t.parent;
            }
        }
    }

    //========================================================
    // ELIMINAR POST-ITS
    //========================================================

    // elimina un post-it junto con sus conexiones y stickers
    private void DestroyPostIt(Transform postIt)
    {
        //----------------------------------------------------
        // ELIMINAR CONEXIONES
        //----------------------------------------------------

        ThreadPoint point =
            postIt.GetComponent<ThreadPoint>();

        if (point != null)
        {
            point.ClearConnections();
        }

        //----------------------------------------------------
        // ELIMINAR STICKERS
        //----------------------------------------------------

        if (stickerManager != null)
        {
            stickerManager.RemoveStickersFromParent(postIt);
        }

        //----------------------------------------------------
        // DESTRUIR OBJETO
        //----------------------------------------------------

        Destroy(postIt.gameObject);
    }
}