using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Sistema de niveles: UNA sola escena. Cada nivel es un asset DatosNivel (Assets/Niveles/).
// Al cargar un nivel: pone pelota y arco, crea las paredes fijas, mueve la zona permitida
// y crea los objetos colocables desde sus prefabs.
// Al hacer gol, espera un momento y carga el siguiente. Después del último, vuelve al 1.
// Va en un GameObject vacío llamado "Niveles".
public class GestorNiveles : MonoBehaviour
{
    [Header("Referencias (arrastralas desde la Hierarchy)")]
    [SerializeField] ReintentarTap pelota;
    [SerializeField] GolDetector arco;
    [Tooltip("El sprite ZonaPermitida de la escena. Se mueve y se agranda según cada nivel")]
    [SerializeField] SpriteRenderer zonaPermitida;
    [Tooltip("Opcional: un Text de la UI para mostrar 'Nivel 2/5'")]
    [SerializeField] Text textoNivel;
    [Tooltip("Opcional: el contador de intentos, para ponerlo en 0 al cambiar de nivel")]
    [SerializeField] ContadorIntentos contador;

    [Header("Niveles (arrastrá los assets de Assets/Niveles, en orden)")]
    [SerializeField] List<DatosNivel> niveles = new List<DatosNivel>();

    [Header("Ajustes")]
    [Tooltip("Segundos entre el gol y el nivel siguiente")]
    [SerializeField] float esperaTrasGol = 1.5f;
    [Tooltip("Color de las paredes fijas")]
    [SerializeField] Color colorParedes = new Color(0.55f, 0.6f, 0.7f);
    [Tooltip("Para probar: en qué nivel arranca el juego (0 = el primero)")]
    [SerializeField] int nivelInicial = 0;

    int nivelActual;
    Transform contenedorParedes;
    Transform contenedorObjetos;
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

        // Dos "carpetas" vacías en la Hierarchy: lo que se crea en cada nivel va adentro
        contenedorParedes = new GameObject("ParedesDelNivel").transform;
        contenedorObjetos = new GameObject("ObjetosDelNivel").transform;
    }

    void OnEnable()  { if (arco != null) arco.AlHacerGol += CuandoHayGol; }
    void OnDisable() { if (arco != null) arco.AlHacerGol -= CuandoHayGol; }

    void Start()
    {
        if (niveles.Count == 0)
        {
            Debug.LogError("GestorNiveles: la lista de niveles está vacía. Arrastrá los assets de Assets/Niveles.");
            return;
        }
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
        DatosNivel n = niveles[indice];

        // 1. Borrar lo del nivel anterior (paredes fijas y objetos colocables)
        Vaciar(contenedorParedes);
        Vaciar(contenedorObjetos);

        // 2. Paredes fijas de este nivel
        foreach (ParedFija p in n.paredes) CrearPared(p);

        // 3. Arco: lugar y ancho
        arco.transform.position = new Vector3(n.arco.x, n.arco.y, arco.transform.position.z);
        float factor = n.anchoArco / anchoBaseArco;
        arco.transform.localScale = new Vector3(escalaBaseArco.x * factor, escalaBaseArco.y, escalaBaseArco.z);

        // 4. Zona permitida: el sprite mide 1x1, así que la escala ES el tamaño
        zonaPermitida.transform.position = new Vector3(n.zonaCentro.x, n.zonaCentro.y, zonaPermitida.transform.position.z);
        zonaPermitida.transform.localScale = new Vector3(n.zonaTamano.x, n.zonaTamano.y, 1f);

        // 5. Objetos colocables, cada uno desde su prefab
        foreach (ObjetoDelNivel o in n.objetos) CrearObjeto(o);

        // 6. Pelota en su lugar y lista para patear
        pelota.FijarPosicionInicial(n.pelota);
        pelota.Reiniciar();

        if (textoNivel != null) textoNivel.text = $"Nivel {indice + 1}/{niveles.Count}";

        // 7. Nivel nuevo, intentos en 0
        if (contador != null) contador.Reiniciar();
    }

    void Vaciar(Transform contenedor)
    {
        for (int i = contenedor.childCount - 1; i >= 0; i--)
        {
            GameObject hijo = contenedor.GetChild(i).gameObject;
            // Destroy recién lo borra al final del frame: lo apagamos ya para que
            // no estorbe al chequear encimados de los objetos nuevos
            hijo.SetActive(false);
            Destroy(hijo);
        }
    }

    void CrearPared(ParedFija p)
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

    void CrearObjeto(ObjetoDelNivel o)
    {
        if (o.prefab == null)
        {
            Debug.LogWarning("GestorNiveles: hay un objeto del nivel sin prefab asignado.");
            return;
        }

        // Instantiate = crear una copia del prefab en la escena, en esa posición y rotación
        ObjetoColocable nuevo = Instantiate(
            o.prefab,
            new Vector3(o.posicion.x, o.posicion.y, 0f),
            Quaternion.Euler(0f, 0f, o.rotacion),
            contenedorObjetos);

        // El prefab no puede guardar referencias a cosas de la escena: la zona se la pasamos acá
        nuevo.Configurar(zonaPermitida);
    }
}
