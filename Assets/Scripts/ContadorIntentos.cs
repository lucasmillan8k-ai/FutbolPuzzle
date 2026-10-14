using UnityEngine;
using UnityEngine.UI;

// Muestra en pantalla cuántas veces pateaste en el nivel actual: "Intentos: 3".
// Va en un Text de la UI (en el mismo Canvas que el texto "GOL").
// Cuenta solo: mira a PateoDrag y suma 1 cada vez que la pelota pasa de "sin patear" a "pateada".
// GestorNiveles lo vuelve a 0 cada vez que carga un nivel.
[RequireComponent(typeof(Text))]
public class ContadorIntentos : MonoBehaviour
{
    [Tooltip("Opcional: el PateoDrag de la Pelota. Si lo dejás vacío, lo busca solo en la escena")]
    [SerializeField] PateoDrag pateo;

    Text texto;
    int intentos;
    bool pateadoAntes; // cómo estaba YaPateo en el frame anterior

    void Awake()
    {
        texto = GetComponent<Text>();
        if (pateo == null) pateo = FindFirstObjectByType<PateoDrag>();
        Mostrar();
    }

    void Update()
    {
        if (pateo == null) return;

        bool pateadoAhora = pateo.YaPateo;

        // Antes no estaba pateada y ahora sí → acaba de salir una patada nueva
        if (pateadoAhora && !pateadoAntes) Sumar();

        pateadoAntes = pateadoAhora;
    }

    void Sumar()
    {
        intentos++; // lo mismo que intentos = intentos + 1
        Mostrar();
    }

    // Pública: la llama GestorNiveles al cargar cada nivel
    public void Reiniciar()
    {
        intentos = 0;
        Mostrar();
    }

    void Mostrar()
    {
        texto.text = $"Intentos: {intentos}";
    }
}
