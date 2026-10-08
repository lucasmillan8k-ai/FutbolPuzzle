using UnityEngine;

// Línea punteada que muestra el principio del recorrido de la pelota mientras arrastrás.
// Dibuja la CURVA real (con gravedad), no una recta: así lo que ves es lo que va a pasar.
// No calcula rebotes: muestra solo el primer tramo del vuelo.
// Va en el mismo GameObject "Pelota" (necesita PateoDrag, Rigidbody2D y un LineRenderer).
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(PateoDrag))]
[RequireComponent(typeof(Rigidbody2D))]
public class TrayectoriaLinea : MonoBehaviour
{
    [Tooltip("Cuántos segundos de vuelo se dibujan. Más = más fácil. 0.4 es un buen punto de partida")]
    [SerializeField, Range(0.05f, 2f)] float segundosVisibles = 0.4f;

    [Tooltip("Cuántos puntos tiene la curva (más = más suave)")]
    [SerializeField] int cantidadPuntos = 20;

    [Tooltip("Largo de cada guion y del espacio entre guiones, en unidades de mundo")]
    [SerializeField] float largoDeGuion = 0.15f;

    PateoDrag pateo;
    LineRenderer linea;
    Rigidbody2D rb;

    void Awake()
    {
        pateo = GetComponent<PateoDrag>();
        linea = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        linea.enabled = false;
        linea.useWorldSpace = true;
    }

    void Update()
    {
        if (!pateo.EstaArrastrando || pateo.DireccionPateo.magnitude < 0.01f)
        {
            linea.enabled = false;
            return;
        }

        linea.enabled = true;

        // Velocidad con la que va a salir: el mismo cálculo que hace PateoDrag al patear
        // (impulso = drag recortado × fuerza; velocidad = impulso / masa)
        Vector2 drag = Vector2.ClampMagnitude(pateo.DireccionPateo, pateo.DistanciaMaxima);
        Vector2 velocidad = drag * pateo.FuerzaPorUnidad / rb.mass;

        // Gravedad que sufre ESTA pelota (la del proyecto × su Gravity Scale)
        Vector2 gravedad = Physics2D.gravity * rb.gravityScale;

        Vector2 origen = rb.position;
        linea.positionCount = cantidadPuntos;
        float largoTotal = 0f;
        Vector2 anterior = origen;

        for (int i = 0; i < cantidadPuntos; i++)
        {
            // Fórmula del tiro oblicuo: posición = origen + v·t + ½·g·t²
            float t = segundosVisibles * i / (cantidadPuntos - 1);
            Vector2 punto = origen + velocidad * t + 0.5f * gravedad * t * t;
            linea.SetPosition(i, punto);
            largoTotal += Vector2.Distance(anterior, punto);
            anterior = punto;
        }

        // Efecto punteado: repetir la textura del material cada "largoDeGuion" unidades.
        // El material tiene que tener Wrap Mode = Repeat y una textura raya-espacio.
        if (linea.material != null)
        {
            linea.material.mainTextureScale = new Vector2(largoTotal / largoDeGuion, 1f);
        }
    }
}
