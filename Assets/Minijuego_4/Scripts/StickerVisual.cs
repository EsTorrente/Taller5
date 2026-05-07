using UnityEngine;

public class StickerVisual : MonoBehaviour
{
    private RectTransform rectTransform;

    [Header("Scale")]
    public float cursorScale = 1.1f;  
    public float placedScale = 0.9f;  

    [Header("Pulse")]
    public float pulseAmplitude = 0.05f;
    public float pulseSpeed = 4f;

    private bool isFollowing = false;
    private Vector3 baseScale;
    private float offset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        baseScale = rectTransform.localScale;
        offset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        if (isFollowing)
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed + offset) * pulseAmplitude;
            rectTransform.localScale = baseScale * cursorScale * pulse;
        }
    }

    public void SetFollowing(bool value)
    {
        isFollowing = value;

        if (!value)
        {
            rectTransform.localScale = baseScale * placedScale;
        }
    }
}