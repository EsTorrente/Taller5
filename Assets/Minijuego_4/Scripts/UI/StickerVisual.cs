using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StickerVisual : MonoBehaviour
{
    //========================================================
    // REFERENCIAS
    //========================================================

    private RectTransform rectTransform;
    private Image image;

    //========================================================
    // CONFIGURACIÓN DE ESCALA
    //========================================================

    [Header("Scale")]
    [SerializeField] private float cursorScale = 1.1f;
    [SerializeField] private float placedScale = 0.9f;

    //========================================================
    // CONFIGURACIÓN DE PULSO
    //========================================================

    [Header("Pulse")]
    [SerializeField] private float pulseAmplitude = 0.05f;
    [SerializeField] private float pulseSpeed = 4f;

    //========================================================
    // ESTADO
    //========================================================

    // indica si el sticker está siguiendo el mouse
    private bool isFollowing = false;

    private Vector3 baseScale;
    private float offset;

    //========================================================
    // UNITY METHODS
    //========================================================

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        baseScale = rectTransform.localScale;

        // offset aleatorio para variar el pulso
        offset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        PulseWhileFollowing();
    }

    //========================================================
    // EFECTO VISUAL
    //========================================================

    // genera un pulso mientras el sticker sigue el mouse
    private void PulseWhileFollowing()
    {
        if (!isFollowing)
            return;

        float pulse =
            1f + Mathf.Sin(Time.time * pulseSpeed + offset)
            * pulseAmplitude;

        rectTransform.localScale =
            baseScale * cursorScale * pulse;
    }

    //========================================================
    // ESTADO VISUAL
    //========================================================

    // activa o desactiva el seguimiento visual
    public void SetFollowing(bool value)
    {
        isFollowing = value;

        // cuando el sticker se coloca, queda un poquito más pequeño
        if (!value)
        {
            rectTransform.localScale =
                baseScale * placedScale;
        }
    }

    //========================================================
    // FEEDBACK VISUAL
    //========================================================

    // feedback visual cuando el sticker no puede colocarse
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