# Diseño de los 15 niveles — FutbolPuzzle v1.0

> Creado: 08/10/2026. Lo usa Lucas para dibujar en papel. Cuando un nivel esté dibujado, sacale foto (o pasá las coordenadas) y Claude lo verifica simulando.

---

## Decisión base (08/10): el tiro sigue siendo libre

Se mantiene el drag de `PateoDrag.cs`. El riesgo: que un nivel se resuelva con puntería sin colocar nada, y el puzzle desaparezca (eso es lo que pasa hoy con los 5 niveles actuales).

**Regla de oro de cada nivel:** desde donde está la pelota, **ningún tiro directo llega al arco**, con ninguna fuerza ni ángulo. El gol solo es posible usando los objetos. Claude lo comprueba con simulación antes de que el nivel pase a Unity.

Formas de bloquear el tiro directo (usá estas, son las que funcionan):
- **Arco escondido:** el arco está en un hueco o debajo de un alero, donde ninguna parábola desde la pelota puede entrar.
- **Pelota encerrada:** la pelota arranca bajo un techo bajo → solo puede salir rasante. Para subir necesita una rampa.
- **Pozo trampa:** el camino "obvio" termina en un pozo del que la pelota no sale. Un objeto tiene que tapar o desviar.

---

## Los 3 objetos (propuesta — se confirma en la semana 2)

| Objeto | Tamaño | Rota | Para qué sirve |
|---|---|---|---|
| **Pared** | 0.5 × 3 | Sí, de a 15° | Rebotar la pelota o frenarla |
| **Rampa** | triángulo 2 × 1 | Sí, de a 15° | Convertir un tiro rasante en uno que sube |
| **Caja** | 1 × 1 | No | Tope, escalón, tapar un hueco, hacer de esquina |

Las tres son **fijas** (no se mueven cuando la pelota les pega). Física igual que hoy: gravedad 1, rebote 0.8, fricción 0.

Cada nivel define: **zona permitida** (rectángulo donde se pueden poner objetos) + **qué objetos hay y cuántos**.

---

## Cómo dibujar en papel cuadriculado

- **1 cuadradito = 0.5 unidades.** La pantalla de juego es un rectángulo de **22 cuadraditos de ancho × 36 de alto**.
- Coordenadas (iguales a las de `GestorNiveles.cs`): x va de **−5.5 a 5.5** (paredes laterales), y va de **−8 (piso) a 10 (arriba)**.
  - x = 0 es la columna del medio (11 cuadraditos desde el borde izquierdo).
  - y = 0 está 16 cuadraditos arriba del piso.
- Una hoja por nivel (o media hoja). En cada dibujo marcá:
  1. **Pelota** (círculo) y **arco** (rectángulo con su ancho)
  2. **Paredes fijas** del nivel (pintadas)
  3. **Zona permitida** (línea punteada)
  4. Al costado: **objetos disponibles** ("1 pared, 1 rampa")
  5. **La solución** con otro color: dónde va cada objeto, en qué ángulo, y la flecha del recorrido de la pelota
  6. Una línea: **qué enseña** el nivel

Si al dibujar no encontrás cómo bloquear el tiro directo, dibujalo igual y anotalo: lo resolvemos al revisar.

---

## La curva: qué enseña cada nivel

Un objeto nuevo por bloque, primero solo y después combinado. Los primeros de cada bloque son casi obvios **a propósito**: enseñan sin tutorial.

### Bloque A — Pared (niveles 1 a 5)

| # | Nombre | Objetos | Idea | Enseña |
|---|---|---|---|---|
| 1 | Primer rebote | 1 pared | Arco en un hueco lateral que no se ve desde la pelota. Zona chica: casi solo hay un lugar posible | Que existe colocar |
| 2 | Girala | 1 pared | La zona es del tamaño justo de la pared: el lugar está dado, lo que hay que encontrar es el ángulo | Rotar de a 15° |
| 3 | Bajala | 1 pared | Arco debajo de un alero. Hay que tirar alto y que la pared, arriba, mande la pelota para abajo | Rebote hacia abajo |
| 4 | Freno | 1 pared | El arco está al pie de una pared fija; sin nada, la pelota pasa de largo. La pared colocada la frena y cae adentro | La pared no solo rebota: también para |
| 5 | Dos bandas | 2 paredes | Un rebote no alcanza, hacen falta dos | Encadenar |

### Bloque B — Rampa (niveles 6 a 9)

| # | Nombre | Objetos | Idea | Enseña |
|---|---|---|---|---|
| 6 | Túnel | 1 rampa | Pelota bajo un techo bajo: solo sale rasante. Arco arriba. La rampa al final del túnel la levanta | Qué hace la rampa |
| 7 | Rampa y banda | 1 rampa + 1 pared | La rampa la levanta, la pared la mete | Combinar dos objetos |
| 8 | Rampa en el aire | 1 rampa | La rampa no va en el piso: va arriba, girada, como desvío | Usar un objeto de forma no obvia |
| 9 | Sobra uno | 1 rampa + 2 paredes | Hay más objetos de los que hacen falta; uno solo sirve | Descartar |

### Bloque C — Caja (niveles 10 a 12)

| # | Nombre | Objetos | Idea | Enseña |
|---|---|---|---|---|
| 10 | Tapar el pozo | 1 caja | El camino obvio cae en un pozo; la caja lo tapa y la pelota sigue | Caja como tapón |
| 11 | Escalón | 1 caja + 1 rampa | La rampa sola sale muy baja; apoyada sobre la caja sale más alta | Apilar objetos |
| 12 | Esquina | 2 cajas | Dos cajas forman una esquina que devuelve la pelota | Rebote en ángulo recto |

### Bloque D — Final (niveles 13 a 15)

| # | Nombre | Objetos | Idea | Enseña |
|---|---|---|---|---|
| 13 | Uno de cada | 1 pared + 1 rampa + 1 caja | Los tres, cada uno con un rol | Repaso |
| 14 | Al grado | 2 paredes | Zona chica, ángulo exacto: con 15° de más no entra | Precisión |
| 15 | La final | 2 paredes + 1 rampa + 1 caja | Dos zonas permitidas separadas. El nivel más largo (recorrido de varios segundos — bueno para clip) | Todo |

---

## Orden de trabajo

1. Dibujar 1 a 5 → mandar foto → Claude verifica la regla de oro → ajustar
2. Igual con 6 a 9, 10 a 12, 13 a 15
3. Recién con los 15 aprobados, se pasan a datos (semana 2, ScriptableObjects)

**Ojo (aviso del PLAN):** si desde el nivel 10 sentís que son todos variantes del mismo, frená y avisá. Es mejor 12 niveles distintos que 15 repetidos, y eso se decide antes de codear.
