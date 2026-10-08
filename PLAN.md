# FutbolPuzzle — de v0.1 a v1.0

> Este archivo es el estado del proyecto.
> Ubicación: `C:\Proyectos\FutbolPuzzle\PLAN.md`
> Última actualización: 08/10/2026

---

## Cómo arrancar y cerrar cada sesión con Claude

Este archivo **no se lee solo**: una tarea nueva no sabe que existe hasta que se le indica. Por eso:

- **Dónde:** app de escritorio → proyecto con la carpeta `C:\Proyectos\` conectada. Una **tarea nueva por día** (o por tema), nunca un mismo chat estirado durante días.
- **Al empezar:** no hace falta pedir nada. Las Instrucciones del proyecto ya le dicen a Claude que lea este archivo.
- **Días sin sesión (instrucción para Claude):** al empezar, si en el Registro diario faltan días desde la última línea, anotalos como "Sin avance" antes de la de hoy. Los huecos tienen que verse, porque fueron lo que hizo fallar el sprint 1.
- **Al terminar:** Lucas escribe **"cerramos"**. Ahí Claude agrega la línea del día en el **Registro diario** (qué se hizo y qué se trabó) y actualiza la fecha de "Última actualización". Si Lucas no escribe "cerramos", Claude no tiene cómo saber que la sesión terminó.
- Claude trabaja en `Assets/`. Las carpetas `Library/`, `Temp/`, `Logs/` y `obj/` son caché de Unity: no se leen ni se tocan.

### Reparto de tareas (acordado el 16/09, ajustado el 08/10 — leer siempre)

| Qué | Quién |
|---|---|
| Scripts `.cs` nuevos y grandes en `Assets/Scripts/` | **Claude los escribe directo en disco.** Lucas después los explica con sus palabras |
| Cambios chicos en scripts existentes (un valor, una condición, un campo nuevo) | **Lucas.** Claude dice qué archivo y qué buscar, no el código entero |
| 1 script chico por semana (ver cronograma) | **Lucas lo intenta primero**, Claude lo revisa |
| Este `PLAN.md`, notas, archivos de datos | **Claude** |
| Escenas `.unity`, prefabs, `.asset`, `ProjectSettings` | **Lucas, desde el editor de Unity.** Claude dicta los valores exactos |

Por qué Claude no toca escenas ni settings: son YAML de Unity con IDs y GUIDs cruzados, se editan a ciegas (no puede ver el resultado) y si Unity está abierto el guardado del editor pisa el cambio. Además, armarlo en el Inspector es cómo se entiende qué hace cada componente.
**Excepción válida:** trabajo repetitivo (ej. generar 15 niveles) → Claude escribe un script de editor en C# que lo hace desde adentro de Unity. Eso es código, no edición a ciegas.

**Por qué Lucas toca código (08/10):** el objetivo no es escribir C# de memoria, es poder programar con IA. Pero Claude no ve Unity: cuando algo falla, quien tiene que entender qué hace el script y dónde mirar es Lucas. Leer, modificar y debuggear es la habilidad; escribir desde cero es solo la práctica que la entrena.

**Claude no tiene consola en esta PC.** No puede correr `git`, ni abrir Unity, ni ejecutar nada acá. Todo comando lo corre Lucas y pega la salida completa si falla.

**Después de que Claude escribe un `.cs`:** volver a Unity para que recompile, y revisar la Console antes de seguir.

### Qué modelo usar

- **Sonnet, esfuerzo medio** → el día a día. Un script, una duda, un error concreto. Es el default
- **Opus, esfuerzo medio/alto** → decisiones de estructura (objetos colocables, formato de niveles, guardado) o un bug que ya resistió 20 minutos
- **Esfuerzo alto/max** → solo para el bug feo que ya se intentó dos veces. No "por las dudas"

---

## Qué es esta etapa

**Sprint 1 (16/09 → 07/10): no se cumplió la fecha.** Quedó: patear con drag, trayectoria con gravedad, arco + "GOL", retry con un tap, 5 niveles (`GestorNiveles.cs`), build WebGL en itch.io como borrador. La causa no fue el alcance: fueron las dos semanas sin tocarlo (20/09 → 03/10).

**Esta etapa:** terminar el juego **completo** (no básico) para aprender bien Unity, antes de arrancar otro proyecto. El próximo proyecto puede usar otro motor (candidato: Godot) para aprender a comparar; se decide cuando este esté terminado.

**Se mide por:** v1.0 publicada antes de la fecha, con todo lo de la lista de abajo.

---

## LA REGLA

**Deadline v1.0: jueves 5 de noviembre de 2026, 23:59.**

- **Mínimo 30 minutos todos los días.** Esto es lo que falló en el sprint 1, no la fecha. Un día de 30 minutos cuenta igual que uno de 4 horas.
- Lo que no entra en la lista v1.0 va a "Ideas para después". La lista no crece.
- Si una semana se atrasa, se recorta de la semana 4 (pulido), no se mueve la fecha.

---

## Qué es la v1.0 (lista cerrada)

| Qué | Qué se aprende de Unity |
|---|---|
| **Mecánica puzzle:** colocar y rotar objetos (pared, caja, rampa) en una zona permitida, snap a grilla y a 15°, límite por nivel | Input sobre objetos, prefabs, colliders en runtime |
| **15 niveles** con curva de dificultad, diseñados en papel primero | Niveles como datos (ScriptableObjects), diseño de niveles |
| **Menú principal + selector de niveles + progreso guardado** | UI (Canvas, botones), escenas, `PlayerPrefs` |
| **Feel:** cámara lenta en el último tramo antes del gol, screen shake chico, 3-4 sonidos (patada, rebote, red, tribuna) | `Time.timeScale`, `AudioSource`, cámara |
| **Builds:** WebGL en itch.io + APK de Android instalada en tu celular | Build Settings, player settings, mobile |

---

## Cronograma

### Hoy — jueves 8/10
1. Si la build de itch no tiene los 5 niveles: build WebGL nueva y resubir
2. Página de itch mínima (título, una línea de descripción, una captura) → **pasarla a pública. Esto es la v0.1**
3. Git + GitHub (repo privado) con `.gitignore` de Unity. Claude dicta los pasos, Lucas corre los comandos

### Semana 1 — vie 9 a jue 15: *objetos colocables*
- Niveles (cambiado 08/10, ya no se dibujan en papel): **Claude arma una base de los 15, verificada con simulación**, y Lucas los edita jugándolos (desde la semana 2 son datos que se tocan en el Inspector). Si Lucas mueve algo grande, pasa los números y Claude re-simula. Lucas diseña al menos 1 nivel propio. Si a partir del 10 son todos variantes del mismo, avisar. Guía, curva, reglas y coordenadas: `DISENO_NIVELES.md`. Decidido 08/10: **el tiro sigue libre**; cada nivel debe hacer imposible el tiro directo
  - Base 1 a 5 (bloque Pared): **hecha** → `Diseno/niveles_1_a_5.png`
  - Resto **por bloque, cada uno después de jugar el anterior** (lo que se aprende jugando corrige los siguientes): 6–9 Rampa y 10–12 Caja en la semana 2, cuando esos objetos existan en Unity; 13–15 Final en la semana 3
- Arrastrar un objeto dentro de una zona permitida, snap a grilla
- Rotación con snap de 15°
- **Script de Lucas:** contador de intentos por nivel en pantalla
- **Hito:** en un nivel podés poner una pared, rotarla y patear

### Semana 2 — vie 16 a jue 22: *niveles de puzzle*
- 3 tipos de objeto como prefabs: pared, caja, rampa
- Pasar los niveles a ScriptableObjects: posición de pelota, arco, paredes fijas y qué objetos hay disponibles
- Construir niveles 1 a 8
- **Script de Lucas:** botón "reiniciar nivel" que vuelve los objetos a su lugar
- **Hito:** 8 niveles de puzzle jugables de corrido

### Semana 3 — vie 23 a jue 29: *juego entero*
- Niveles 9 a 15
- Menú principal + selector de niveles (bloqueados hasta pasar el anterior)
- Guardar progreso con `PlayerPrefs`
- **Script de Lucas:** guardar y leer el último nivel desbloqueado
- **Hito:** se abre, se elige nivel, se juegan los 15, se cierra y se recuerda el progreso

### Semana 4 — vie 30/10 a jue 5/11: *que se sienta bien y publicar*
- Cámara lenta + shake + sonidos
- Jugarlo entero, anotar lo roto, arreglar solo lo que rompe la partida
- Build Android (APK) al celular. Build WebGL final
- **Jue 5/11: PUBLICAR v1.0** en itch. Mandarlo a 10 personas. 5 líneas de qué aprendiste

---

## LISTA NO — no entra en la v1.0

- ❌ Arte definitivo — **formas geométricas de colores**
- ❌ Arquero, imán, viento, hielo, explosivos, portales
- ❌ Estrellas, puntaje de estilo, rankings
- ❌ Modo carrera, temporadas, mapa de mundos
- ❌ Ajustes, idiomas, tutorial elaborado
- ❌ Anuncios, IAP, SDKs, analytics
- ❌ Play Store (requiere 12 testers durante 14 días; opcional post v1.0)
- ❌ Más de 15 niveles
- ❌ Cuentas, nube, multijugador
- ❌ Cambiar de motor (eso es para el próximo proyecto)

---

## Stack (decidido)

| Qué | Cuál |
|---|---|
| Motor | **Unity 6.3 LTS** (6000.3.24f1), plantilla Universal 2D |
| Módulos | WebGL Build Support + Android Build Support |
| Editor de código | Visual Studio Code |
| Control de versiones | Git + GitHub (repo privado) — **obligatorio desde el 08/10** |
| Orientación | Portrait (9:16), 1080×1920 |
| Carpetas | Padre: `C:\Proyectos\` · Proyecto: `C:\Proyectos\FutbolPuzzle\` |

---

## Cómo trabajar con la IA (esto es la mitad del aprendizaje)

- **Una tarea nueva por día o por tema.** El contexto vive en este archivo, no en el chat
- **Un script por vez.** Nunca "hacéme el juego"
- **Después de cada script de Claude:** explicarlo vos en 3 líneas. Si no podés, preguntá antes de seguir
- **Cambios chicos los hacés vos.** Pedí "dónde y qué", no "el código entero"
- **Cuando rompe, pegá el error exacto.** Completo, con el número de línea
- **Dudas conceptuales y de volumen** → ChatGPT Go. **Código, archivos y decisiones del proyecto** → Claude con `C:\Proyectos\` conectada

---

## Registro diario

| Fecha | Qué hice | Qué se trabó |
|---|---|---|
| 08/10 | Cierre del sprint 1: la fecha del 7/10 no se cumplió (6 y 7 sin avance; quedaron los 5 niveles de `GestorNiveles.cs`). Decidido: terminar el juego completo antes de otro proyecto, para aprender bien Unity; el próximo puede ser con otro motor. Nueva meta v1.0 al 05/11 con lista cerrada (objetos colocables, 15 niveles, menú + selector + guardado, feel, WebGL + APK). Plan reescrito. Reparto ajustado: Lucas hace los cambios chicos y 1 script chico por semana. **v0.1 pública en itch** (se verificó que la build del 05/10 ya tenía los 5 niveles; página con título, descripción y captura, embed 540×960). **Git + GitHub armados:** Git 2.55 instalado con winget, `.gitignore` de Unity (lo escribió Claude; deja afuera `Library/`, `Builds/`, `Logs/`, `UserSettings/`, archivos de VS Code), primer commit y push a `github.com/lucasmillan8k-ai/FutbolPuzzle` (privado, rama `main`). Las 3 tareas del día cumplidas. Próxima sesión: diseño de los 15 niveles en papel, con Opus esfuerzo medio | El primer push quedó colgado esperando login porque el navegador predeterminado estaba mal; se resolvió con Ctrl+C y repetir `git push` |
| 05/10 | Build WebGL subida a itch.io y andando (lo hizo Lucas). Charla de diseño: hoy el juego es de puntería, no puzzle; la parte puzzle (colocar/rotar objetos) quedó recortada y va post-sprint. Para el 7/10 el "puzzle" lo dan los niveles: obstáculos que obligan a globear, rodear o picar en la pared. Claude escribió `GestorNiveles.cs` (5 niveles como datos en una sola escena, paredes generadas por código, pasa solo al siguiente tras el gol) y modificó `GolDetector.cs` (evento `AlHacerGol`, `HuboGol`), `ReintentarTap.cs` (`FijarPosicionInicial`, sin retry durante el gol), `PateoDrag.cs` (expone `FuerzaPorUnidad`) y `TrayectoriaLinea.cs` (dibuja la curva con gravedad, no una recta). Niveles verificados con una simulación aproximada (gravedad 1, rebote 0.8, fricción 0, rotación congelada, fuerza 5, distancia máx 4) | Pendiente Lucas en Unity: ajustar física a esos valores, crear `Niveles` con `GestorNiveles`, probar los 5 niveles, nueva build y resubir |
| 04/10 | Retomado tras 2 semanas parado. Recorte del plan. Verificado que el 19/09 también quedaron hechos `TrayectoriaLinea.cs` y `GolDetector.cs` + `Arco`. Arreglado el texto "GOL" que no se veía (caja 160×30 con fuente 120 → 900×250, overflow, Canvas Scaler 1080×1920). Claude escribió `ReintentarTap.cs` y modificó `PateoDrag.cs` (un pateo por intento) y `GolDetector.cs` (`OcultarGol`). Probado: patear → gol → tap → reinicia, OK | 20/09–03/10 sin trabajar. El texto GOL quedó negro (pendiente cambiar color) |
| 19/09 | Bloque Sáb 19 (input): Claude escribió `Assets/Scripts/PateoDrag.cs` — arrastrar y soltar → `AddForce` Impulse, estilo honda, con Input System (mouse + touch). Las paredes del Vie 18 ya estaban en la escena desde el 17/09 (±5.5, sin Physics Material propio) | Pendiente: Lucas agrega el script a `Pelota`, prueba en Play y ajusta `fuerzaPorUnidad`. Vie 18 sin registrar: falta confirmar si se probaron fricción/rebote |
| 17/09 | Bloque día 1 (Jue 17) completo: escena `Level.unity` en portrait 1080x1920, cámara Orthographic configurada, `Piso` con Box Collider 2D, `Pelota` con Circle Collider 2D + Rigidbody 2D (Gravity Scale 2, Continuous, Interpolate) y Physics Material 2D `Rebote` (Friction 0.4, Bounciness 0.7). Play: la pelota cae y rebota perdiendo altura. Entendidos Rigidbody2D, Collider2D y Physics Material 2D | Al crear la cámara se duplicó (quedó `Main Camera` + `Camera`, con warning de 2 Audio Listeners) — se borró la de más y quedó resuelto |
| 16/09 | Unity Hub + Unity 6000.3.24f1 instalados. Proyecto `FutbolPuzzle` creado (Universal 2D) en `C:\Proyectos\`. `PLAN.md` en su lugar. Carpeta conectada y primera sesión con Claude probada. Acordado el reparto de tareas y qué modelo usar. **Prueba de conexión OK:** Claude lee `C:\Proyectos\` y escribe archivos en disco (este renglón lo escribió él) | Nada roto. Queda por confirmar en el Hub que WebGL Build Support y Android Build Support estén tildados. Claude no puede borrar ni renombrar archivos en la carpeta: solo leer, crear y sobrescribir |

---

## Ideas para después (cementerio de tentaciones)

- Imán, viento, hielo, explosivos, portales, arquero
- Capa de relato: relator, tribuna, repetición, título automático del golazo
- Tiro del Día (un nivel diario compartido)
- Puntaje de estilo, estrellas por nivel
- Carrera: potrero → inferiores → segunda → primera → internacional
- Play Store con closed testing
- Arte definitivo

---

## Definición de TERMINADO (v1.0)

Al 5 de noviembre existe un link público en itch.io y una APK en tu celular donde se abre un menú, se elige nivel, se juegan 15 niveles en los que colocás y rotás objetos antes de patear, el progreso se guarda, y el gol se siente bien (cámara lenta, sonido).
