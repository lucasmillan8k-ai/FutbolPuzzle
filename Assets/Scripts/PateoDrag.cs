using UnityEngine;
using UnityEngine.InputSystem;

// Arrastrar y soltar para patear la pelota.
// Va en el GameObject "Pelota" (necesita Rigidbody2D).
// Funciona con mouse (editor / PC) y con el dedo (celular / WebGL).
[RequireComponent(typeof(Rigidbody2D))]
public class PateoDrag : MonoBehaviour
{
    [Header("Potencia")]
    [Tooltip("Cuánta fuerza por cada unidad de mundo que arrastrás")]
    [SerializeField] float fuerzaPorUnidad = 5f;

    [Tooltip("Arrastres más largos que esto no suman más fuerza")]
    [SerializeField] float distanciaMaxima = 4f;

    [Tooltip("Arrastres más cortos que esto se ignoran (evita patear con un tap)")]
    [SerializeField] float distanciaMinima = 0.2f;

    [Header("Dirección")]
    [Tooltip("Tildado: tirás para atrás y sale para adelante (honda). Destildado: sale hacia donde arrastrás")]
    [SerializeField] bool estiloHonda = true;

    Rigidbody2D rb;
    Camera cam;
    Vector2 inicioDrag;
    bool arrastrando;
    bool yaPateo;          // un solo pateo por intento; se rearma con Rearmar()
    int frameRearmado = -1; // el toque que reinicia no tiene que empezar un drag

    // Para que TrayectoriaLinea pueda dibujar la previsualización sin duplicar la lectura del puntero
    public bool EstaArrastrando => arrastrando;
    public Vector2 DireccionPateo { get; private set; }
    public float DistanciaMaxima => distanciaMaxima;
    public float FuerzaPorUnidad => fuerzaPorUnidad;
    public bool YaPateo => yaPateo;

    // Lo llama ReintentarTap al reiniciar el intento
    public void Rearmar()
    {
        yaPateo = false;
        arrastrando = false;
        frameRearmado = Time.frameCount;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    void Update()
    {
        // Pointer = el último mouse o pantalla táctil que se usó
        Pointer puntero = Pointer.current;
        if (puntero == null) return;

        // Posición del puntero pasada de píxeles de pantalla a coordenadas del mundo
        Vector2 posMundo = cam.ScreenToWorldPoint(puntero.position.ReadValue());

        // Ya pateaste en este intento, o este mismo toque fue el que reinició: no se arrastra
        if (yaPateo || Time.frameCount == frameRearmado) return;

        if (puntero.press.wasPressedThisFrame)
        {
            inicioDrag = posMundo;
            arrastrando = true;
        }

        if (arrastrando)
        {
            Vector2 drag = posMundo - inicioDrag;
            DireccionPateo = estiloHonda ? -drag : drag;
        }

        if (arrastrando && puntero.press.wasReleasedThisFrame)
        {
            arrastrando = false;
            Patear(posMundo - inicioDrag);
        }
    }

    void Patear(Vector2 drag)
    {
        if (drag.magnitude < distanciaMinima) return;

        if (estiloHonda) drag = -drag;
        drag = Vector2.ClampMagnitude(drag, distanciaMaxima);

        // Frenar la pelota antes de patear: cada patada sale igual, sin importar cómo venía
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        yaPateo = true;

        // Impulse = golpe instantáneo (una patada), no un empuje continuo
        rb.AddForce(drag * fuerzaPorUnidad, ForceMode2D.Impulse);
    }
}
