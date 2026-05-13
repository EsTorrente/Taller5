using UnityEngine;

public class ClueFactory : MonoBehaviour
{
    [Header("Configuración de Fábrica")]
    [SerializeField] private ClueUI cluePrefab;
    [SerializeField] private Transform clueContainer;

    public ClueUI CreateClue(ClueData data, PhotoManager manager)
    {
        ClueUI newClue = Instantiate(cluePrefab, clueContainer);

        newClue.Init(data, manager);

        RectTransform rt = newClue.GetComponent<RectTransform>();
        rt.anchoredPosition = data.position;
        rt.localScale = data.scale;

        return newClue;
    }
}