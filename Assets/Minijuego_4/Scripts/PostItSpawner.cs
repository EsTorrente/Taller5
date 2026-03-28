using UnityEngine;

public class PostItSpawner : MonoBehaviour
{
    public GameObject postItPrefab;
    public Transform boardArea;

    public void SpawnPostIt()
    {
        GameObject obj = Instantiate(postItPrefab, boardArea);

        RectTransform rt = obj.GetComponent<RectTransform>();

        Vector2 randomOffset = new Vector2(
            Random.Range(-100f, 100f),
            Random.Range(-100f, 100f)
        );

        rt.anchoredPosition = randomOffset;
    }
}