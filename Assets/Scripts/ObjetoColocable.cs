using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Un objeto que el jugador puede arrastrar ANTES de patear (pared, y después caja y rampa).
// - Solo se mueve dentro de la zona permitida.
// - Se acomoda a la grilla (snap).
// - Un toque corto (sin arrastrar) lo gira 15° en sentido horario.
// - Si al soltarlo queda encimado con otra cosa (pared fija, pelota, arco, otro objeto), vuelve a donde estaba.
// - Después de patear queda bloqueado hasta el próximo intento.
// Va en el objeto colocable (necesita un Collider2D). Sin Rigidbody2D: es fijo, la pelota rebota en él.
[RequireComponent(typeof(Collider2D))]
public class ObjetoColocable : MonoBehaviour
{
    [Header("Zona")]
    [Tooltip("El sprite que marca la zona permitida. El objeto entero tiene que quedar adentro de ese rectángulo")]
    [SerializeField] SpriteRenderer zonaPermitida;

    [Header("Grilla")]
    [Tooltip("Paso de la grilla en unidades de mundo. 0.5 = un cuadradito del papel. 0 = sin grilla")]
    [SerializeField] float tamanoGrilla = 0.5f;

    [Header("Rotación")]
    [Tooltip("Destildalo para objetos que no giran (la caja)")]
    [SerializeField] bool puedeRotar = true;
    [Tooltip("Cuántos grados gira con cada toque")]
    [SerializeField] float pasoRotacion = 15f;

    [Header("Toque")]
    [Tooltip("Cuánto por fuera del objeto todavía cuenta como agarrarlo (la pared es finita y el dedo es grueso)")]
    [SerializeField] float margenToque = 0.3f;
    [Tooltip("Si el dedo se mueve menos que esto entre apretar y soltar, es un toque (gira) y no un arrastre")]
    [SerializeField] float distanciaToque = 0.25f;

    [Header("Colores")]
    [Tooltip("Color mientras está en un lugar donde NO se puede soltar")]
    [SerializeField] Color colorInvalido = new Color(1f, 0.35f, 0.35f);

    // ---- Lista de todos los colocables de la escena (la usa PateoDrag para no patear si tocaste uno) ----
    static readonly List<ObjetoColocable> todos = new List<ObjetoColocable>();
    static ObjetoColocable arrastrado; // el que se está moviendo ahora (como mucho uno)

    // true si en ese punto hay un objeto que se puede agarrar
    public static bool HayAlgunoEn(Vector2 punto)
    {
        foreach (ObjetoColocable o in todos)
            if (o.PuedeAgarrarse(punto)) return true;
        return false;
    }

    Collider2D col;
    SpriteRenderer sr;
    Camera cam;
    PateoDrag pateo;
    Color colorNormal;
    Color colorArrastrando;
    Vector2 offsetAgarre;              // distancia entre el dedo y el centro, para que no "salte" al agarrarlo
    Vector3 posicionAntesDeArrastrar;  // a dónde vuelve si lo soltás en un lugar inválido
    Vector2 puntoDondeSeApreto;        // para saber si fue un toque o un arrastre
    float maximoQueSeMovioElDedo;
    readonly List<Collider2D> encimados = new List<Collider2D>();

    // Después de patear no se puede tocar nada hasta reintentar
    bool Bloqueado => pateo != null && pateo.YaPateo;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;
        pateo = FindFirstObjectByType<PateoDrag>();

        if (sr != null)
        {
            colorNormal = sr.color;
            colorArrastrando = Color.Lerp(colorNormal, Color.white, 0.35f); // un poco más claro = "lo tenés agarrado"
        }
    }

    void OnEnable() => todos.Add(this);

    void OnDisable()
    {
        todos.Remove(this);
        if (arrastrado == this) arrastrado = null;
    }

    void Update()
    {
        Pointer puntero = Pointer.current;
        if (puntero == null) return;

        Vector2 posMundo = cam.ScreenToWorldPoint(puntero.position.ReadValue());

        // 1. Agarrar: toque nuevo, nadie más agarrado, y el toque cae sobre este objeto
        if (arrastrado == null && puntero.press.wasPressedThisFrame && PuedeAgarrarse(posMundo))
        {
            arrastrado = this;
            posicionAntesDeArrastrar = transform.position;
            offsetAgarre = (Vector2)transform.position - posMundo;
            puntoDondeSeApreto = posMundo;
            maximoQueSeMovioElDedo = 0f;
        }

        if (arrastrado != this) return;

        maximoQueSeMovioElDedo = Mathf.Max(maximoQueSeMovioElDedo, Vector2.Distance(posMundo, puntoDondeSeApreto));
        bool esToque = maximoQueSeMovioElDedo < distanciaToque;

        // 2. Mover: seguir al dedo, con snap y sin salir de la zona (si todavía es un toque, no se mueve)
        if (!esToque) Ubicar(Limitar(posMundo + offsetAgarre));
        bool valido = !EstaEncimado();
        if (sr != null) sr.color = valido ? colorArrastrando : colorInvalido;

        // 3. Soltar: si fue un toque, gira. Si fue un arrastre y quedó encimado, vuelve a donde estaba
        if (puntero.press.wasReleasedThisFrame)
        {
            if (esToque) Girar();
            else if (!valido) Ubicar(posicionAntesDeArrastrar);
            if (sr != null) sr.color = colorNormal;
            arrastrado = null;
        }
    }

    // Gira un paso en sentido horario. Si después del giro no entra o queda encimado, se deshace
    void Girar()
    {
        if (!puedeRotar) return;

        Vector3 posicionAntes = transform.position;
        Quaternion rotacionAntes = transform.rotation;

        // Girar: en 2D se gira alrededor del eje Z. Negativo = sentido de las agujas del reloj
        transform.Rotate(0f, 0f, -pasoRotacion);
        Physics2D.SyncTransforms(); // para que el collider se entere del giro antes de medirlo

        // Al girar cambia cuánto ocupa (una pared acostada es más ancha): reacomodarla dentro de la zona
        Ubicar(Limitar(transform.position));

        if (EstaEncimado())
        {
            transform.rotation = rotacionAntes;
            Ubicar(posicionAntes);
        }
    }

    bool PuedeAgarrarse(Vector2 punto)
    {
        if (Bloqueado || !isActiveAndEnabled) return false;
        // ClosestPoint devuelve el mismo punto si está adentro; si está afuera, el borde más cercano
        return Vector2.Distance(col.ClosestPoint(punto), punto) <= margenToque;
    }

    // Aplica grilla y zona a una posición deseada
    Vector3 Limitar(Vector2 deseada)
    {
        if (zonaPermitida == null)
            return new Vector3(Redondear(deseada.x), Redondear(deseada.y), transform.position.z);

        Bounds zona = zonaPermitida.bounds;
        Vector2 mitad = col.bounds.extents; // medio ancho y medio alto del objeto (ya incluye la rotación)

        float x = LimitarEje(deseada.x, zona.min.x, zona.max.x, mitad.x);
        float y = LimitarEje(deseada.y, zona.min.y, zona.max.y, mitad.y);
        return new Vector3(x, y, transform.position.z);
    }

    // En un eje: el centro tiene que quedar a "mitad" de distancia de cada borde, y sobre la grilla
    float LimitarEje(float valor, float zonaMin, float zonaMax, float mitad)
    {
        float min = zonaMin + mitad;
        float max = zonaMax - mitad;

        if (tamanoGrilla > 0f)
        {
            // Los extremos también van a la grilla, pero hacia adentro (para no asomar de la zona)
            min = Mathf.Ceil(min / tamanoGrilla - 0.001f) * tamanoGrilla;
            max = Mathf.Floor(max / tamanoGrilla + 0.001f) * tamanoGrilla;
        }

        if (min > max) return (zonaMin + zonaMax) / 2f; // el objeto es más grande que la zona: centrado
        return Mathf.Clamp(Redondear(valor), min, max);
    }

    float Redondear(float v)
    {
        if (tamanoGrilla <= 0f) return v;
        return Mathf.Round(v / tamanoGrilla) * tamanoGrilla;
    }

    void Ubicar(Vector3 posicion)
    {
        transform.position = posicion;
        // La física 2D no se entera sola de que movimos el transform: se lo avisamos ya,
        // así el chequeo de encimado usa la posición nueva y no la del frame anterior
        Physics2D.SyncTransforms();
    }

    bool EstaEncimado()
    {
        // Filtro vacío = cualquier collider cuenta, incluidos los triggers (así no se puede tapar el arco)
        ContactFilter2D filtro = new ContactFilter2D { useTriggers = true };
        return col.Overlap(filtro, encimados) > 0;
    }
}
