using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Sistema de niveles: UNA sola escena, los niveles son datos (una lista).
// Cada nivel dice dónde arranca la pelota, dónde está el arco y qué paredes hay.
// Al hacer gol, espera un momento y carga el siguiente. Después del último, vuelve al 1.
// Va en un GameObject vacío llamado "Niveles".
public class GestorNiveles : MonoBehaviour
{
    // Una pared/obstáculo: centro, tamaño y rotación (en grados), en unidades de mundo
    [System.Serializable]
    public class Pared
    {
        public Vector2 centro;
        public Vector2 tamano = Vector2.one;
        public float rotacion;

        public Pared() { }

        public Pared(float x, float y, float ancho, float alto, float rot = 0f)
        {
            centro = new Vector2(x, y);
            tamano = new Vector2(ancho, alto);
            rotacion = rot;
        }
    }

    [System.Serializable]
    public class Nivel
    {
        public string nombre;
        public Vector2 pelota;
        public Vector2 arco;
        [Tooltip("Ancho del arco en unidades de mundo (el alto no cambia)")]
        public float anchoArco = 3f;
        public List<Pared> paredes = new List<Pared>();
    }

    [Header("Referencias (arrastralas desde la Hierarchy)")]
    [SerializeField] ReintentarTap pelota;
    [SerializeField] GolDetector arco;
    [Tooltip("Opcional: un Text de la UI para mostrar 'Nivel 2/5'")]
    [SerializeField] Text textoNivel;

    [Header("Ajustes")]
    [Tooltip("Segundos entre el gol y el nivel siguiente")]
    [SerializeField] float esperaTrasGol = 1.5f;
    [Tooltip("Color de las paredes de cada nivel")]
    [SerializeField] Color colorParedes = new Color(0.55f, 0.6f, 0.7f);
    [Tooltip("Para probar: en qué nivel arranca el juego (0 = el primero)")]
    [SerializeField] int nivelInicial = 0;

    // Los 5 niveles. Coordenadas pensadas para la escena actual:
    // cámara ortográfica tamaño 10, paredes laterales en x = ±5.5, piso en y = -8.
    // La pelota apoyada en el piso está en y = -7.25.
    [Header("Niveles")]
    [SerializeField] List<Nivel> niveles = new List<Nivel>
    {
        new Nivel { nombre = "Directo", pelota = new Vector2(0f, -7.25f), arco = new Vector2(0f, 6f), anchoArco = 3f },

        new Nivel { nombre = "Globito", pelota = new Vector2(-3f, -7.25f), arco = new Vector2(3.2f, -7f), anchoArco = 3f,
            paredes = { new Pared(0.5f, -4f, 0.5f, 7.5f) } },

        new Nivel { nombre = "Bajo techo", pelota = new Vector2(3.5f, -7.25f), arco = new Vector2(-3.75f, -7f), anchoArco = 2.5f,
            paredes = { new Pared(-2.5f, -4.5f, 5.5f, 0.5f), new Pared(-0.5f, -6.75f, 0.5f, 1.5f) } },

        new Nivel { nombre = "Por el hueco", pelota = new Vector2(2f, -7.25f), arco = new Vector2(-3.5f, 4f), anchoArco = 3f,
            paredes = { new Pared(1.625f, 0f, 7.25f, 0.5f) } },

        new Nivel { nombre = "Banda", pelota = new Vector2(-3.5f, -7.25f), arco = new Vector2(-4f, 4f), anchoArco = 2f,
            paredes = { new Pared(-1.75f, 1.5f, 6.5f, 0.5f), new Pared(-4.75f, 2.75f, 0.5f, 2f) } },
    };

    int nivelActual;
    Transform contenedorParedes;
    Sprite spriteCuadrado;
    Vector3 escalaBaseArco;
    float anchoBaseArco;
    bool cambiando;

    void Awake()
    {
        // Un cuadrado blanco de 1x1 unidad creado por código, para no depender de ningún asset
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        spriteCuadrado = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

        // Ancho real del arco tal como está en la escena, para poder escalarlo por nivel
        escalaBaseArco = arco.transform.localScale;
        anchoBaseArco = arco.GetComponent<BoxCollider2D>().size.x * escalaBaseArco.x;

        contenedorParedes = new GameObject("ParedesDelNivel").transform;
    }

    void OnEnable()  { if (arco != null) arco.AlHacerGol += CuandoHayGol; }
    void OnDisable() { if (arco != null) arco.AlHacerGol -= CuandoHayGol; }

    void Start()
    {
        CargarNivel(Mathf.Clamp(nivelInicial, 0, niveles.Count - 1));
    }

    void CuandoHayGol()
    {
        if (!cambiando) StartCoroutine(PasarAlSiguiente());
    }

    IEnumerator PasarAlSiguiente()
    {
        cambiando = true;
        yield return new WaitForSecondsRealtime(esperaTrasGol);
        CargarNivel((nivelActual + 1) % niveles.Count); // después del último vuelve al primero
        cambiando = false;
    }

    public void CargarNivel(int indice)
    {
        nivelActual = indice;
        Nivel n = niveles[indice];

        // 1. Borrar las paredes del nivel anterior
        for (int i = contenedorParedes.childCount - 1; i >= 0; i--)
            Destroy(contenedorParedes.GetChild(i).gameObject);

        // 2. Crear las paredes de este nivel
        foreach (Pared p in n.paredes) CrearPared(p);

        // 3. Mover el arco y ajustar su ancho
        arco.transform.position = new Vector3(n.arco.x, n.arco.y, arco.transform.position.z);
        float factor = n.anchoArco / anchoBaseArco;
        arco.transform.localScale = new Vector3(escalaBaseArco.x * factor, escalaBaseArco.y, escalaBaseArco.z);

        // 4. Poner la pelota en su lugar y dejar todo listo para patear
        pelota.FijarPosicionInicial(n.pelota);
        pelota.Reiniciar();

        if (textoNivel != null) textoNivel.text = $"Nivel {indice + 1}/{niveles.Count}";
    }

    void CrearPared(Pared p)
    {
        GameObject go = new GameObject("Pared");
        go.transform.SetParent(contenedorParedes);
        go.transform.position = p.centro;
        go.transform.rotation = Quaternion.Euler(0f, 0f, p.rotacion);
        go.transform.localScale = new Vector3(p.tamano.x, p.tamano.y, 1f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = spriteCuadrado;
        sr.color = colorParedes;

        // Sin Rigidbody2D: es una pared fija. El collider toma el tamaño del sprite (1x1) y la escala lo agranda
        go.AddComponent<BoxCollider2D>();
    }
}
