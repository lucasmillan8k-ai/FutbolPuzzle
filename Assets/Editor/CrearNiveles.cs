using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Script de EDITOR (no va en el juego): agrega el menú "FutbolPuzzle > Crear niveles 1 a 5".
// Crea los assets Assets/Niveles/Nivel01..Nivel05 con los datos de Diseno/niveles_1_a_5.png.
// Se usa una sola vez. Después los niveles se tocan en el Inspector, no acá.
// Tiene que estar en una carpeta llamada "Editor": Unity no la incluye en las builds.
public static class CrearNiveles
{
    const string Carpeta = "Assets/Niveles";

    [MenuItem("FutbolPuzzle/Crear niveles 1 a 5")]
    static void Crear()
    {
        ObjetoColocable pared = AssetDatabase.LoadAssetAtPath<ObjetoColocable>("Assets/Prefabs/ParedColocable.prefab");
        if (pared == null)
        {
            EditorUtility.DisplayDialog("Crear niveles", "No encontré Assets/Prefabs/ParedColocable.prefab", "OK");
            return;
        }

        if (!AssetDatabase.IsValidFolder(Carpeta))
            AssetDatabase.CreateFolder("Assets", "Niveles");

        // Si ya existen, preguntar antes de pisar (pueden tener ajustes hechos a mano)
        if (AssetDatabase.LoadAssetAtPath<DatosNivel>($"{Carpeta}/Nivel01.asset") != null &&
            !EditorUtility.DisplayDialog("Crear niveles",
                "Los niveles ya existen. ¿Pisarlos con los valores originales? Se pierden los cambios hechos en el Inspector.",
                "Pisar", "Cancelar"))
            return;

        // Coordenadas: ver DISENO_NIVELES.md. Paredes = (centro x, centro y, ancho, alto)
        Guardar(1, Nivel("Túnel", pelota: new Vector2(-4.5f, -7.25f), arco: new Vector2(1.5f, -1.5f), anchoArco: 2f,
            zonaCentro: new Vector2(2f, -6f), zonaTamano: new Vector2(3f, 3.5f), // 3.5 de alto: la pared puede dar la vuelta entera (14/10)
            paredes: new[] {
                new ParedFija(1.5f, 0.75f, 3f, 0.5f),      // techo de la copa
                new ParedFija(0.25f, -1f, 0.5f, 3f),       // lado izquierdo de la copa
                new ParedFija(2.75f, -1f, 0.5f, 3f),       // lado derecho de la copa
                new ParedFija(-3.25f, -6.25f, 4.5f, 0.5f), // techo del túnel
            },
            objetos: new[] { Obj(pared, 2f, -6.5f, -135f) }));

        Guardar(2, Nivel("Girala", pelota: new Vector2(-4.5f, -7.25f), arco: new Vector2(3.75f, 2.25f), anchoArco: 2f,
            zonaCentro: new Vector2(-4.125f, 3f), zonaTamano: new Vector2(2.25f, 3f),
            paredes: new[] {
                new ParedFija(3.375f, 4f, 4.25f, 0.5f),      // techo del arco
                new ParedFija(3.375f, 0.5f, 4.25f, 0.5f),    // piso del arco
                new ParedFija(1.5f, 1.25f, 0.5f, 1f),        // tope
                new ParedFija(-3.5f, -3.875f, 0.5f, 7.75f),  // chimenea
            },
            objetos: new[] { Obj(pared, -4f, 3f, 30f) }));

        Guardar(3, Nivel("Que caiga", pelota: new Vector2(4.5f, -7.25f), arco: new Vector2(-4.25f, -7f), anchoArco: 1.5f,
            zonaCentro: new Vector2(-1f, -5.75f), zonaTamano: new Vector2(3.5f, 4f),
            paredes: new[] {
                new ParedFija(-3.25f, -5.75f, 0.5f, 4f),   // poste junto al arco
                new ParedFija(3.25f, -6.25f, 4.5f, 0.5f),  // techo del túnel
            },
            objetos: new[] { Obj(pared, -1f, -6f, 0f) }));

        Guardar(4, Nivel("Bajala", pelota: new Vector2(4.5f, -7.25f), arco: new Vector2(-4.25f, -7f), anchoArco: 1.5f,
            zonaCentro: new Vector2(2.125f, 8.125f), zonaTamano: new Vector2(6.25f, 3.25f),
            paredes: new[] {
                new ParedFija(3.5f, -0.875f, 0.5f, 13.75f), // chimenea
                new ParedFija(-3f, -5.75f, 0.5f, 4f),       // poste junto al arco
            },
            objetos: new[] { Obj(pared, 0f, 8f, 0f) }));

        Guardar(5, Nivel("Dos bandas", pelota: new Vector2(-4.5f, -7.25f), arco: new Vector2(4.25f, 4.75f), anchoArco: 1.5f,
            zonaCentro: new Vector2(1.5f, -1f), zonaTamano: new Vector2(4.5f, 13.5f),
            paredes: new[] {
                new ParedFija(3.875f, 6.5f, 3.25f, 0.5f),   // techo del arco
                new ParedFija(3.875f, 3f, 3.25f, 0.5f),     // piso del arco
                new ParedFija(2.5f, 3.75f, 0.5f, 1f),       // tope
                new ParedFija(-3.25f, -6.25f, 4.5f, 0.5f),  // techo del túnel
            },
            objetos: new[] { Obj(pared, 0f, 0f, 0f), Obj(pared, 2.5f, 0f, 0f) }));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Crear niveles", "Listo: 5 niveles en Assets/Niveles.", "OK");
    }

    static DatosNivel Nivel(string nombre, Vector2 pelota, Vector2 arco, float anchoArco,
        Vector2 zonaCentro, Vector2 zonaTamano, ParedFija[] paredes, ObjetoDelNivel[] objetos)
    {
        DatosNivel n = ScriptableObject.CreateInstance<DatosNivel>();
        n.nombre = nombre;
        n.pelota = pelota;
        n.arco = arco;
        n.anchoArco = anchoArco;
        n.zonaCentro = zonaCentro;
        n.zonaTamano = zonaTamano;
        n.paredes = new List<ParedFija>(paredes);
        n.objetos = new List<ObjetoDelNivel>(objetos);
        return n;
    }

    static ObjetoDelNivel Obj(ObjetoColocable prefab, float x, float y, float rotacion)
    {
        return new ObjetoDelNivel { prefab = prefab, posicion = new Vector2(x, y), rotacion = rotacion };
    }

    static void Guardar(int numero, DatosNivel nivel)
    {
        string ruta = $"{Carpeta}/Nivel{numero:00}.asset";
        DatosNivel existente = AssetDatabase.LoadAssetAtPath<DatosNivel>(ruta);

        if (existente == null)
        {
            AssetDatabase.CreateAsset(nivel, ruta);
        }
        else
        {
            // Copiar los valores ADENTRO del asset que ya existe (y no borrarlo y crearlo de nuevo):
            // así la lista de GestorNiveles lo sigue encontrando
            EditorUtility.CopySerialized(nivel, existente);
            existente.name = $"Nivel{numero:00}";
            EditorUtility.SetDirty(existente);
            Object.DestroyImmediate(nivel);
        }
    }
}
