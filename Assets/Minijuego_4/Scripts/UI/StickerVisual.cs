using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StickerVisual : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image image;

    [Header("Scale")]
    [SerializeField] private float cursorScale = 1.1f;
    [SerializeField] private float placedScale = 0.9f;

    [Header("Pulse")]
    [SerializeField] private float pulseAmplitude = 0.05f;
    [SerializeField] private float pulseSpeed = 4f;

    private bool isFollowing = false;

    private Vector3 baseScale;
    private float offset;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        baseScale = rectTransform.localScale;

        offset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        PulseWhileFollowing();
    }

    // genera el pulso mientras el sticker sigue el mouse
    private void PulseWhileFollowing()
    {
        if (!isFollowing)
            return;

        float pulse =
            1f + Mathf.Sin(Time.time * pulseSpeed + offset) * pulseAmplitude;

        rectTransform.localScale = baseScale * cursorScale * pulse;
    }

    // activa o desactiva el modo seguimiento
    public void SetFollowing(bool value)
    {
        isFollowing = value;

        // cuando se coloca, queda un poquito más pequeño
        if (!value)
        {
            rectTransform.localScale = baseScale * placedScale;
        }
    }

    // feedback visual cuando un sticker no puede colocarse
    public void BlinkRed()
    {
        StartCoroutine(BlinkRedCoroutine());
    }

    private IEnumerator BlinkRedCoroutine()
    {
        if (image == null)
            yield break;

        Color colorDeReposo = Color.white;

        image.color = Color.red;

        yield return new WaitForSeconds(0.15f);

        if (image != null)
        {
            image.color = colorDeReposo;
        }
    }
}