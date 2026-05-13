using UnityEngine;

public class FloatAnim : MonoBehaviour
{
    [Header("Movimiento vertical")]
    public float amplitude = 0.5f;   // Qué tan alto/bajo se mueve
    public float frequency = 1f;     // Velocidad de la oscilación

    [Header("Rotación")]
    public Vector3 rotationSpeed = new Vector3(0, 50f, 0); // Un valor de rotacion solo en y

    [Header("Opcional")]
    public bool randomOffset = true; // Para que no todos se muevan igual

    // Estas variables privadas se escriben aqui afuera para que todo este codigo pueda usarlas
    private Vector3 startPos;
    private float offset;

    void Start()
    {
        //Esto guarda la posicion exacta del objeto al momento de darle play.
        startPos = transform.position;

        //El if pregunta si el bool esta activo o no
        //En caso de estarlo hace que el offset tenga valores aleatorios
        if (randomOffset)
        {
            offset = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    void Update()
    {
        //En el update esta constantemente haciendo operaciones con el sin (oscilaciones)
        float y = Mathf.Sin(Time.time * frequency + offset) * amplitude;
        //Al valor original de la posicion le sumamos el valor de la opreacion anterior solo en y (eje vertical)
        transform.position = startPos + new Vector3(0, y, 0);
       
        //Rota basandose en lo que tarda en renderizar el ultimo frame multiplicado por el valor de rotacion antes dado
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}