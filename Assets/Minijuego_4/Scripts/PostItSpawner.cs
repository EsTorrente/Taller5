using UnityEngine;

public class PostItSpawner : MonoBehaviour
{
    [SerializeField] private GameObject postItPrefab;
    [SerializeField] private Transform boardArea;
    [SerializeField] private StickerManager stickerManager; 
    [SerializeField] private ThreadManager threadManager; 

    public void SpawnPostIt()
    {
        GameObject obj = Instantiate(postItPrefab, boardArea);
        RectTransform rt = obj.GetComponent<RectTransform>();

        rt.anchoredPosition = new Vector2(
            Random.Range(-100f, 100f),
            Random.Range(-100f, 100f)
        );

        if (obj.TryGetComponent(out Draggable draggable))
            draggable.Initialize(stickerManager);

        if (obj.TryGetComponent(out ThreadPoint threadPoint))
            threadPoint.Initialize(threadManager);
    }
}