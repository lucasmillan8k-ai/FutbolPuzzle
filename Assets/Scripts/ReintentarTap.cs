using UnityEngine;
using UnityEngine.InputSystem;

// Retry instantáneo: después de patear, un toque en cualquier lado vuelve todo al inicio.
// Sin animación ni espera. Va en el GameObject "Pelota", junto a PateoDrag.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PateoDrag))]
public class ReintentarTap : MonoBehaviour
{
    [Tooltip("Opcional: el GolDetector del Arco. Si lo dejás vacío, lo busca solo en la escena")]
    [SerializeField] GolDetector golDetector;

    Rigidbody2D rb;
    PateoDrag pateo;
    Vector2 posicionInicial;
    float rotacionInicial;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pateo = GetComponent<PateoDrag>();
        if (golDetector == null) golDetector = FindFirstObjectByType<GolDetector>();

        // Guardamos dónde arranca la pelota para poder volver ahí
        posicionInicial = rb.position;
        rotacionInicial = rb.rotation;
    }

    void Update()
    {
        // Solo se puede reintentar si ya se pateó en este intento
        if (!pateo.YaPateo) return;

        // Si fue gol, el GestorNiveles pasa solo al siguiente: el tap no hace nada
        if (golDetector != null && golDetector.HuboGol) return;

        Pointer puntero = Pointer.current;
        if (puntero == null) return;

        // Toque nuevo (no el mismo que soltó la patada) → reiniciar en el acto
        if (puntero.press.wasPressedThisFrame) Reiniciar();
    }

    // La usa GestorNiveles: cada nivel tiene su propio punto de partida
    public void FijarPosicionInicial(Vector2 posicion)
    {
        posicionInicial = posicion;
    }

    // Pública: la usa también GestorNiveles al cargar un nivel
    public void Reiniciar()
    {
        // Frenar todo y teletransportar la pelota al punto de partida
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.position = posicionInicial;
        rb.rotation = rotacionInicial;
        transform.position = posicionInicial; // para que se vea en el mismo frame, sin esperar a la física

        if (golDetector != null) golDetector.OcultarGol();
        pateo.Rearmar();
    }
}
