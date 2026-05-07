using UnityEngine;

public class FloatAnim : MonoBehaviour
{
    [Header("Movimiento vertical")]
    public float amplitude = 0.5f;   // Qué tan alto/bajo se mueve
    public float frequency = 1f;     // Velocidad de la oscilación

    [Header("Rotación")]
    public Vector3 rotationSpeed = new Vector3(0, 50f, 0);

    [Header("Opcional")]
    public bool randomOffset = true; // Para que no todos se muevan igual


    private Vector3 startPos;
    private float offset;

    void Start()
    {
        startPos = transform.position;

        if (randomOffset)
        {
            offset = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * frequency + offset) * amplitude;
        transform.position = startPos + new Vector3(0, y, 0);

        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}