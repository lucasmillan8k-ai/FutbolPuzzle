using UnityEngine;
using UnityEngine.UI;

// Detecta cuando la pelota entra al arco y muestra el texto "GOL".
// Va en el GameObject "Arco" (un BoxCollider2D marcado como Trigger).
[RequireComponent(typeof(BoxCollider2D))]
public class GolDetector : MonoBehaviour
{
    [Tooltip("El texto 'GOL' de la UI. Arrastralo acá desde la Hierarchy")]
    [SerializeField] Text textoGol;

    [Tooltip("Nombre exacto del GameObject de la pelota, para no reaccionar si otra cosa entra")]
    [SerializeField] string nombrePelota = "Pelota";

    // Avisa a quien esté escuchando (GestorNiveles) que hubo gol
    public event System.Action AlHacerGol;

    // true desde el gol hasta que se reinicia el intento
    public bool HuboGol { get; private set; }

    void Reset()
    {
        // Al agregar el componente por primera vez, forzamos que el collider sea Trigger
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    void Start()
    {
        if (textoGol != null) textoGol.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Filtro por nombre: así una pared o cualquier otra cosa que pase cerca no cuenta como gol
        if (other.gameObject.name != nombrePelota) return;

        // Si la pelota sale y vuelve a entrar, no cuenta dos veces
        if (HuboGol) return;
        HuboGol = true;

        Debug.Log("GOL!");
        if (textoGol != null) textoGol.gameObject.SetActive(true);
        AlHacerGol?.Invoke();
    }

    // Lo llama ReintentarTap al reiniciar el intento
    public void OcultarGol()
    {
        HuboGol = false;
        if (textoGol != null) textoGol.gameObject.SetActive(false);
    }
}
