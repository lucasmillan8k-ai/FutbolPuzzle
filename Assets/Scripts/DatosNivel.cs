using System.Collections.Generic;
using UnityEngine;

// Un nivel guardado como ARCHIVO del proyecto (un ScriptableObject), no como código.
// Cada nivel es un asset en Assets/Niveles/ (Nivel01, Nivel02...). Se edita en el Inspector
// y se prueba sin tocar ningún script.
//
// Coordenadas en unidades de mundo, igual que en DISENO_NIVELES.md:
// x va de -5.5 a 5.5 (paredes laterales), el piso está en y = -8, la pelota apoyada queda en y = -7.25.
[CreateAssetMenu(fileName = "Nivel", menuName = "FutbolPuzzle/Nivel")]
public class DatosNivel : ScriptableObject
{
    [Tooltip("Nombre corto del nivel (se usa para identificarlo, ej. 'Túnel')")]
    public string nombre;

    [Header("Pelota y arco")]
    public Vector2 pelota = new Vector2(0f, -7.25f);
    public Vector2 arco = new Vector2(0f, 6f);
    [Tooltip("Ancho del arco en unidades de mundo (el alto no cambia)")]
    public float anchoArco = 2f;

    [Header("Paredes fijas (las grises, el jugador no las mueve)")]
    public List<ParedFija> paredes = new List<ParedFija>();

    [Header("Zona permitida (donde el jugador puede poner los objetos)")]
    public Vector2 zonaCentro;
    public Vector2 zonaTamano = new Vector2(3f, 3f);

    [Header("Objetos que el jugador puede mover")]
    [Tooltip("Cada uno aparece en su posición inicial al cargar el nivel. Ponelos adentro de la zona y SIN resolver el nivel")]
    public List<ObjetoDelNivel> objetos = new List<ObjetoDelNivel>();
}

// Una pared fija: centro, tamaño y rotación (grados)
[System.Serializable]
public class ParedFija
{
    public Vector2 centro;
    public Vector2 tamano = Vector2.one;
    public float rotacion;

    public ParedFija() { }

    public ParedFija(float x, float y, float ancho, float alto, float rot = 0f)
    {
        centro = new Vector2(x, y);
        tamano = new Vector2(ancho, alto);
        rotacion = rot;
    }
}

// Un objeto colocable del nivel: qué prefab es y dónde arranca
[System.Serializable]
public class ObjetoDelNivel
{
    [Tooltip("Arrastrá un prefab de Assets/Prefabs (ParedColocable, Caja o Rampa)")]
    public ObjetoColocable prefab;
    public Vector2 posicion;
    [Tooltip("Grados. Negativo = sentido horario, como el toque (-15, -30...)")]
    public float rotacion;
}
