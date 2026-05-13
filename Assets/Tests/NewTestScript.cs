using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using System.Linq;

public class NewTestScript
{

    /// creo un ClueUI con baseImage y markerImage asignados pq sin esto, SetClue() / UpdateVisual() lanza NullReferenceException
    private ClueUI MakeClueUI()
    {
        GameObject obj = new GameObject();

        Image markerImage = new GameObject().AddComponent<Image>();
        Image baseImage = new GameObject().AddComponent<Image>();

        ClueUI clueUI = obj.AddComponent<ClueUI>();
        clueUI.markerImage = markerImage;
        clueUI.baseImage = baseImage;

        return clueUI;
    }

    // ========================
    // MINIJUEGO 1
    // ========================
    /// Conjunto de casos de prueba para GridToWorldCentered().
    /// Cada caso define:
    /// - posición en la grilla (col,row)
    /// - tamaño del bloque en slots
    /// - tamaño de cada slot
    /// - posición esperada en el mundo.
    /// Esto permite probar varios escenarios automáticamente.
    public static IEnumerable<TestCaseData> GridToWorldData()
    {
        // col, row, wSlots, hSlots, slotSize, expectedX, expectedY
        yield return new TestCaseData(0, 0, 1, 1, 2f, 1f, 1f)
            .SetName("Grid_OrigenSlot1x1");
        yield return new TestCaseData(1, 1, 2, 2, 2f, 4f, 4f)
            .SetName("Grid_Centro2x2");
        yield return new TestCaseData(0, 0, 2, 1, 2f, 2f, 1f)
            .SetName("Grid_Bloque2x1");
        yield return new TestCaseData(3, 2, 1, 2, 2f, 7f, 6f)
            .SetName("Grid_EsquinaBloque1x2");
    }

    [TestCaseSource(nameof(GridToWorldData))]
    public void GridToWorldCentered_ReturnsCorrectPosition(
        int col, int row, int wSlots, int hSlots,
        float slotSize, float expectedX, float expectedY)
    {
        GameObject boardObj = new GameObject();
        cajon board = boardObj.AddComponent<cajon>();
        board.boardOrigin = new GameObject().transform;
        board.slotSize = slotSize;

        Vector3 result = board.GridToWorldCentered(col, row, wSlots, hSlots);

        Assert.AreEqual(new Vector3(expectedX, expectedY, 0f), result);

        Object.DestroyImmediate(boardObj);
    }

    /// Casos de prueba para tamaños de bloques.
    /// Se valida que las escalas del objeto correspondan
    /// correctamente a widthInSlots y heightI

    public static IEnumerable<TestCaseData> BlockSizeData()
    {
        // scaleX, scaleY, expectedWidth, expectedHeight
        yield return new TestCaseData(4f, 2f, 2, 1).SetName("Block_4x2_da_2x1slots");
        yield return new TestCaseData(2f, 2f, 1, 1).SetName("Block_2x2_da_1x1slots");
        yield return new TestCaseData(4f, 4f, 2, 2).SetName("Block_4x4_da_2x2slots");
        yield return new TestCaseData(6f, 2f, 3, 1).SetName("Block_6x2_da_3x1slots");
    }

    /// Crea un tablero (cajon) listo para pruebas.
    ///
    /// Configura:
    /// - columnas
    /// - filas
    /// - tamaño de slot
    /// - boardOrigin
    ///
    /// Luego ejecuta Awake() manualmente porque en EditMode
    /// Unity no siempre llama automáticamente los ciclos de vida.
    private cajon MakeCajon(int columns = 4, int rows = 4, float slotSize = 2f)
    {
        GameObject boardObj = new GameObject();
        cajon board = boardObj.AddComponent<cajon>();
        board.boardOrigin = new GameObject().transform;
        board.columns = columns;
        board.rows = rows;
        board.slotSize = slotSize;
        board.Awake(); 
        return board;
    }

    /// Crea un bloque de prueba.
    ///
    /// Ajusta:
    /// - posición en la grilla
    /// - tamaño en slots
    /// - escala visual
    /// - si es bloque ganador o no
    ///
    /// Esto evita repetir código en cada test.

    private objetosCocina MakeBlock(int col, int row, int w, int h, bool isWin = false)
    {
        GameObject obj = new GameObject();
        obj.transform.localScale = new Vector3(w * 2f, h * 2f, 1f);
        objetosCocina block = obj.AddComponent<objetosCocina>();
        block.gridCol = col;
        block.gridRow = row;
        block.widthInSlots = w;
        block.heightInSlots = h;
        block.isWinBlock = isWin;
        return block;
    }

    /// Casos de prueba para CanMoveTo().
    ///
    /// Simulan movimientos hacia:
    /// - espacios libres
    /// - espacios ocupados
    /// - esquinas
    ///
    /// Sirven para validar colisiones.
    public static IEnumerable<TestCaseData> CanMoveToData()
    {
        // newCol, newRow, ocuparSlot, expectedCanMove
        //el bloque a mover es 1x1 y hay un obstáculo en (2,2)
        yield return new TestCaseData(1, 1, false).SetName("CanMove_SlotLibre");
        yield return new TestCaseData(2, 2, true).SetName("CanMove_SlotOcupado");
        yield return new TestCaseData(0, 0, false).SetName("CanMove_Esquina");
        yield return new TestCaseData(3, 3, false).SetName("CanMove_OtroSlotLibre");
    }

    [TestCaseSource(nameof(CanMoveToData))]

    /// Comprueba si CanMoveTo() detecta correctamente
    /// cuándo un bloque puede o no moverse.
    ///
    /// El test:
    /// 1. Crea un tablero.
    /// 2. Crea un bloque que se moverá.
    /// 3. Opcionalmente agrega un obstáculo.
    /// 4. Intenta mover el bloque.
    /// 5. Verifica si el resultado coincide con lo esperado.
    ///
    /// Si existe obstáculo en el destino,
    /// el método debe devolver false.

    public void Cajon_CanMoveTo_DetectaColisiones(int newCol, int newRow, bool hayObstaculo)
    {
        cajon board = MakeCajon();

        objetosCocina mover = MakeBlock(0, 0, 1, 1);
        objetosCocina obstaculo = MakeBlock(2, 2, 1, 1);

        board.RegisterBlock(mover);
        if (hayObstaculo)
            board.RegisterBlock(obstaculo);

        bool result = board.CanMoveTo(mover, newCol, newRow);

        // si hay obstáculo en (2,2) y muevo a (2,2) debe ser false
        bool expected = !(hayObstaculo && newCol == 2 && newRow == 2);
        Assert.AreEqual(expected, result);

        Object.DestroyImmediate(board.gameObject);
        Object.DestroyImmediate(mover.gameObject);
        Object.DestroyImmediate(obstaculo.gameObject);
    }

    /// Casos de prueba para RegisterBlock().
    ///
    /// Definen diferentes tamaños y posiciones de bloques
    /// para validar que el grid se llene correctamente.
    public static IEnumerable<TestCaseData> RegisterBlockData()
    {
        // col, row, wSlots, hSlots
        yield return new TestCaseData(0, 0, 1, 1).SetName("Register_Bloque1x1_Origen");
        yield return new TestCaseData(1, 1, 2, 2).SetName("Register_Bloque2x2_Centro");
        yield return new TestCaseData(2, 0, 2, 1).SetName("Register_Bloque2x1_Fila0");
        yield return new TestCaseData(0, 2, 1, 2).SetName("Register_Bloque1x2_Col0");
    }

    [TestCaseSource(nameof(RegisterBlockData))]

    /// Verifica que RegisterBlock() registre correctamente
    /// el bloque en todas las celdas del grid que ocupa.
    ///
    /// El test recorre cada slot ocupado y comprueba
    /// que la referencia almacenada en grid[c,r]
    /// sea exactamente el bloque registrado.
    public void Cajon_RegisterBlock_LlenaGridCorrectamente(int col, int row, int w, int h)
    {
        cajon board = MakeCajon();
        objetosCocina block = MakeBlock(col, row, w, h);

        board.RegisterBlock(block);

        for (int c = col; c < col + w; c++)
            for (int r = row; r < row + h; r++)
                Assert.AreEqual(block, board.grid[c, r], $"grid[{c},{r}] debe ser el bloque");

        Object.DestroyImmediate(board.gameObject);
        Object.DestroyImmediate(block.gameObject);
    }

    /// el bloque ganador debe estar dentro del rango de filas de salida para poder escapar
    public static IEnumerable<TestCaseData> WinConditionData()
    {
        yield return new TestCaseData(2, true).SetName("Win_FilaCorrecta_2");
        yield return new TestCaseData(0, false).SetName("Win_FilaIncorrecta_0");
        yield return new TestCaseData(1, false).SetName("Win_FilaIncorrecta_1");
        yield return new TestCaseData(3, false).SetName("Win_FilaIncorrecta_3_SaleDelRango");
    }

    [TestCaseSource(nameof(WinConditionData))]

    /// Verifica si el bloque ganador está dentro
    /// del rango válido de salida.
    ///
    /// El test:
    /// 1. Configura las filas de salida.
    /// 2. Crea un bloque ganador.
    /// 3. Calcula si entra completamente en el rango.
    /// 4. Compara con el resultado esperado.
    ///
    /// Esto valida la lógica de escape del puzzle.
    public void Cajon_WinCondition_FilasDeSalida(int blockRow, bool expectedWin)
    {
        cajon board = MakeCajon();
        board.exitRowMin = 2;
        board.exitRowMax = 3;
        board.columns = 4;

        // bloque ganador 2x2 (ocupa 2 filas)
        objetosCocina block = MakeBlock(2, blockRow, 2, 2, isWin: true);

        bool enFilasSalida = (block.gridRow >= board.exitRowMin &&
                              block.gridRow + block.heightInSlots - 1 <= board.exitRowMax);

        Assert.AreEqual(expectedWin, enFilasSalida);

        Object.DestroyImmediate(board.gameObject);
        Object.DestroyImmediate(block.gameObject);
    }

    // ========================
    // MINIJUEGO 2
    // ========================


    /// creo un DiarioManager con todos los GameObjects y TextMeshProUGUI que necesita para q Start() / AbrirDiario() no lancen NullReferenceException.
    private DiarioManager MakeDiarioManager()
    {
        GameObject obj = new GameObject();
        DiarioManager diario = obj.AddComponent<DiarioManager>();

        diario.diarioCerrado = new GameObject();
        diario.panelCombinacion = new GameObject();
        diario.diarioLyra = new GameObject();

        // TextMeshProUGUI pide un Canvas padre para no lanzar warnings
        GameObject canvasObj = new GameObject();
        canvasObj.AddComponent<Canvas>();

        GameObject t1 = new GameObject(); t1.transform.SetParent(canvasObj.transform);
        GameObject t2 = new GameObject(); t2.transform.SetParent(canvasObj.transform);
        GameObject t3 = new GameObject(); t3.transform.SetParent(canvasObj.transform);

        diario.texto1 = t1.AddComponent<TMPro.TextMeshProUGUI>();
        diario.texto2 = t2.AddComponent<TMPro.TextMeshProUGUI>();
        diario.texto3 = t3.AddComponent<TMPro.TextMeshProUGUI>();

        return diario;
    }

    /// Casos de prueba para subir y bajar dígitos.
    ///
    /// Incluyen:
    /// - incrementos normales
    /// - decrementos normales
    /// - wrap de 9->0
    /// - wrap de 0->9
    ///
    /// Permiten validar el comportamiento circular.

    public static IEnumerable<TestCaseData> DigitoData()
    {
        // operacion, veces, digitoInicial, esperado
        yield return new TestCaseData("subir", 1, 0, 1).SetName("Digito_Subir_0a1");
        yield return new TestCaseData("subir", 1, 8, 9).SetName("Digito_Subir_8a9");
        yield return new TestCaseData("subir", 1, 9, 0).SetName("Digito_Subir_9Wrappea0");
        yield return new TestCaseData("subir", 3, 0, 3).SetName("Digito_Subir_3veces");
        yield return new TestCaseData("bajar", 1, 5, 4).SetName("Digito_Bajar_5a4");
        yield return new TestCaseData("bajar", 1, 0, 9).SetName("Digito_Bajar_0Wrappea9");
        yield return new TestCaseData("bajar", 3, 2, 9).SetName("Digito_Bajar_3vecesDesde2");
    }

    [TestCaseSource(nameof(DigitoData))]

    /// Verifica que SubirDigito() y BajarDigito()
    /// modifiquen correctamente los números.
    ///
    /// El test:
    /// 1. Inicializa un dígito.
    /// 2. Ejecuta la operación varias veces.
    /// 3. Comprueba el valor final.
    ///
    /// También valida el comportamiento tipo "contador circular".
    public void DiarioManager_Digito_SubirYBajar(
        string operacion, int veces, int digitoInicial, int esperado)
    {
        DiarioManager diario = MakeDiarioManager();
        diario.digitos[0] = digitoInicial;

        for (int i = 0; i < veces; i++)
        {
            if (operacion == "subir") diario.SubirDigito(0);
            else diario.BajarDigito(0);
        }

        Assert.AreEqual(esperado, diario.digitos[0]);
        Object.DestroyImmediate(diario.gameObject);
    }

    /// Casos de prueba para comprobar códigos.
    ///
    /// Se prueban:
    /// - el código correcto
    /// - varias combinaciones incorrectas
    ///
    /// Esto valida la lógica de combinación.

    /// no llamo Comprobar() directamente pq en el caso correcto llama a SceneManager.LoadScene(), que crashea en Edit Mode
    public static IEnumerable<TestCaseData> ComprobarData()
    {
        yield return new TestCaseData(5, 6, 2, true).SetName("Codigo_Correcto_562");
        yield return new TestCaseData(0, 0, 0, false).SetName("Codigo_Incorrecto_000");
        yield return new TestCaseData(5, 8, 0, false).SetName("Codigo_Incorrecto_580");
        yield return new TestCaseData(5, 0, 2, false).SetName("Codigo_Incorrecto_502");
        yield return new TestCaseData(0, 8, 2, false).SetName("Codigo_Incorrecto_082");
        yield return new TestCaseData(2, 8, 5, false).SetName("Codigo_Incorrecto_Invertido");
    }

    [TestCaseSource(nameof(ComprobarData))]

    /// Comprueba si los dígitos ingresados coinciden
    /// con el código correcto.
    ///
    /// No se llama directamente Comprobar()
    /// porque ese método carga una escena
    /// y puede causar errores en EditMode.
    ///
    /// En cambio, el test replica la comparación lógica.
    public void DiarioManager_Comprobar_CoincideConCodigo(
        int d0, int d1, int d2, bool esperadoCorrecto)
    {
        DiarioManager diario = MakeDiarioManager();

        diario.digitos[0] = d0;
        diario.digitos[1] = d1;
        diario.digitos[2] = d2;

        bool esCorrecto = diario.digitos[0] == diario.codigoCorrecto[0] &&
                          diario.digitos[1] == diario.codigoCorrecto[1] &&
                          diario.digitos[2] == diario.codigoCorrecto[2];

        Assert.AreEqual(esperadoCorrecto, esCorrecto);
        Object.DestroyImmediate(diario.gameObject);
    }

    [Test]
    public void DiarioManager_AbrirDiario_ActivaPaneles()
    {
        DiarioManager diario = MakeDiarioManager();

        diario.diarioCerrado.SetActive(false);
        diario.panelCombinacion.SetActive(false);

        diario.AbrirDiario();

        Assert.IsTrue(diario.diarioCerrado.activeSelf, "diarioCerrado debe activarse");
        Assert.IsTrue(diario.panelCombinacion.activeSelf, "panelCombinacion debe activarse");

        Object.DestroyImmediate(diario.gameObject);
    }

    [Test]


    /// Verifica que AbrirDiario():
    /// - active diarioCerrado
    /// - active panelCombinacion
    ///
    /// El test desactiva primero los paneles,
    /// ejecuta el método y comprueba que queden activos.
    public void DiarioManager_CerrarDiario_DesactivaPaneles()
    {
        DiarioManager diario = MakeDiarioManager();

        diario.diarioCerrado.SetActive(true);
        diario.panelCombinacion.SetActive(true);
        diario.diarioLyra.SetActive(false);

        diario.CerrarDiario();

        Assert.IsFalse(diario.diarioCerrado.activeSelf, "diarioCerrado debe desactivarse");
        Assert.IsFalse(diario.panelCombinacion.activeSelf, "panelCombinacion debe desactivarse");
        Assert.IsTrue(diario.diarioLyra.activeSelf, "diarioLyra debe activarse");

        Object.DestroyImmediate(diario.gameObject);
    }

    // ========================
    // MINIJUEGO 3
    // ========================

    /// Se prueban:
    /// - pistas relevantes correctamente marcadas
    /// - pistas irrelevantes correctamente marcadas
    /// - estados incorrectos
    /// - pistas sin marcar
    ///
    /// Así se valida toda la lógica de evaluación.
    public static IEnumerable<TestCaseData> ClueIsCorrectData()
    {
        //isActuallyRelevant, stateToSet, expectedResult
        yield return new TestCaseData(true, ClueState.Relevant, true)
            .SetName("Clue_Relevante_MarcadaRelevante_Correcto");
        yield return new TestCaseData(false, ClueState.Irrelevant, true)
            .SetName("Clue_Irrelevante_MarcadaIrrelevante_Correcto");
        yield return new TestCaseData(true, ClueState.Irrelevant, false)
            .SetName("Clue_Relevante_MarcadaIrrelevante_Incorrecto");
        yield return new TestCaseData(false, ClueState.Relevant, false)
            .SetName("Clue_Irrelevante_MarcadaRelevante_Incorrecto");
        yield return new TestCaseData(true, ClueState.None, false)
            .SetName("Clue_SinMarcar_Incorrecto");
    }

    [TestCaseSource(nameof(ClueIsCorrectData))]

    /// Comprueba si IsCorrect() devuelve
    /// el resultado esperado según:
    /// - el tipo real de pista
    /// - el estado asignado por el jugador.
    ///
    /// El test:
    /// 1. Crea una pista.
    /// 2. Configura relevancia y estado.
    /// 3. Llama SetClue().
    /// 4. Verifica el resultado.
    public void ClueUI_IsCorrect_Parametrizado(
        bool isActuallyRelevant, ClueState stateToSet, bool expectedResult)
    {
        ClueUI clueUI = MakeClueUI();
        clueUI.clueData = new ClueData
        {
            isActuallyRelevant = isActuallyRelevant,
            currentState = stateToSet
        };

        clueUI.SetClue(clueUI.clueData);

        Assert.AreEqual(expectedResult, clueUI.IsCorrect());

        Object.DestroyImmediate(clueUI.gameObject);
    }

    [Test]

    /// Verifica que CycleState() rote correctamente
    /// entre los estados:
    ///
    /// None -> Relevant -> Irrelevant -> None
    ///
    /// Esto asegura que el sistema de selección
    /// del jugador funcione correctamente.
    public void Clue_CycleState_WrapsCorrectly()
    {
        Clue clue = new RelevantClue("test");

        Assert.AreEqual(ClueState.None, clue.GetState(), "empieza en None");
        clue.CycleState();
        Assert.AreEqual(ClueState.Relevant, clue.GetState(), "1er ciclo es Relevant");
        clue.CycleState();
        Assert.AreEqual(ClueState.Irrelevant, clue.GetState(), "2do ciclo es Irrelevant");
        clue.CycleState();
        Assert.AreEqual(ClueState.None, clue.GetState(), "3er ciclo vuelve a None");
    }

    [Test]

    /// Comprueba que una pista relevante
    /// solo sea considerada correcta
    /// cuando está marcada como Relevant.
    ///
    /// Los demás estados deben dar false.
    public void RelevantClue_IsCorrect_SoloEnRelevant()
    {
        Clue clue = new RelevantClue("test");

        Assert.IsFalse(clue.IsCorrect(), "None es incorrecto");
        clue.CycleState(); // rel
        Assert.IsTrue(clue.IsCorrect(), "Relevant es correcto");
        clue.CycleState(); // irr
        Assert.IsFalse(clue.IsCorrect(), "Irrelevant es incorrecto");
    }

    [Test]

    /// Comprueba que una pista relevante
    /// solo sea considerada correcta
    /// cuando está marcada como Relevant.
    ///
    /// Los demás estados deben dar false.
    public void IrrelevantClue_IsCorrect_SoloEnIrrelevant()
    {
        Clue clue = new IrrelevantClue("test");

        Assert.IsFalse(clue.IsCorrect(), "None es incorrecto");
        clue.CycleState(); // rel
        Assert.IsFalse(clue.IsCorrect(), "Relevant es incorrecto");
        clue.CycleState(); // irr
        Assert.IsTrue(clue.IsCorrect(), "Irrelevant es correcto");
    }

    // ========================
    // MINIJUEGO 4
    // ========================

    [Test]

    /// Verifica que AddConnection()
    /// agregue correctamente una línea
    /// a la lista de conexiones del punto.
    ///
    /// El test:
    /// 1. Crea un ThreadPoint.
    /// 2. Crea una línea.
    /// 3. Agrega la conexión.
    /// 4. Comprueba que exista en la lista.

    public void ThreadPoint_AddConnection_Agrega()
    {
        GameObject obj = new GameObject();
        ThreadPoint point = obj.AddComponent<ThreadPoint>();
        ThreadLine line = new GameObject().AddComponent<ThreadLine>();

        GameObject mgrObj = new GameObject();
        ThreadManager manager = mgrObj.AddComponent<ThreadManager>();
        point.Initialize(manager);

        point.AddConnection(line);
        Assert.IsTrue(point.Connections.Contains(line));

        Object.DestroyImmediate(obj);
        Object.DestroyImmediate(mgrObj);
        Object.DestroyImmediate(line.gameObject);
    }

    [Test]

    /// Verifica que AddConnection()
    /// no agregue líneas duplicadas.
    ///
    /// Se intenta añadir la misma línea dos veces
    /// y luego se comprueba que la lista
    /// solo tenga un elemento.
    public void ThreadPoint_AddConnection_SinDuplicados()
    {
        GameObject obj = new GameObject();
        ThreadPoint point = obj.AddComponent<ThreadPoint>();
        ThreadLine line = new GameObject().AddComponent<ThreadLine>();

        point.AddConnection(line);
        point.AddConnection(line); //segunda vez, debe ignorarse

        Assert.AreEqual(1, point.Connections.Count);

        Object.DestroyImmediate(obj);
        Object.DestroyImmediate(line.gameObject);
    }

    [Test]

    /// Verifica que RemoveConnection()
    /// elimine correctamente una conexión.
    ///
    /// El test:
    /// 1. Agrega una línea.
    /// 2. La elimina.
    /// 3. Comprueba que ya no esté en la lista.
    public void ThreadPoint_RemoveConnection_Elimina()
    {
        GameObject obj = new GameObject();
        ThreadPoint point = obj.AddComponent<ThreadPoint>();
        ThreadLine line = new GameObject().AddComponent<ThreadLine>();

        point.AddConnection(line);
        point.RemoveConnection(line);

        Assert.IsFalse(point.Connections.Contains(line));

        Object.DestroyImmediate(obj);
        Object.DestroyImmediate(line.gameObject);
    }

    [Test]

    /// Verifica que limpiar la lista de conexiones
    /// deje el ThreadPoint sin líneas asociadas.
    ///
    /// Primero se agregan varias conexiones,
    /// luego se ejecuta Clear()
    /// y finalmente se valida que Count sea 0.
    public void ThreadPoint_ClearConnections_VaciaLaLista()
    {
        GameObject obj = new GameObject();
        ThreadPoint point = obj.AddComponent<ThreadPoint>();

        var lineObjects = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            GameObject lo = new GameObject();
            lineObjects.Add(lo);
            ThreadLine line = lo.AddComponent<ThreadLine>();
            point.AddConnection(line);
        }

        Assert.AreEqual(3, point.Connections.Count, "setup: 3 líneas agregadas");

        point.ClearConnections();

        Assert.AreEqual(0, point.Connections.Count, "la lista debe quedar vacía");

        Object.DestroyImmediate(obj);
        foreach (var lo in lineObjects) Object.DestroyImmediate(lo);
    }

    /// Casos parametrizados para probar
    /// agregar y eliminar múltiples líneas.
    ///
    /// Se prueban diferentes cantidades
    /// para asegurar que la lógica escale bien.
    public static IEnumerable<TestCaseData> ThreadCountData()
    {
        yield return new TestCaseData(1).SetName("Thread_AgregarYEliminar_1linea");
        yield return new TestCaseData(3).SetName("Thread_AgregarYEliminar_3lineas");
        yield return new TestCaseData(5).SetName("Thread_AgregarYEliminar_5lineas");
    }

    [TestCaseSource(nameof(ThreadCountData))]

    /// Verifica que:
    /// 1. Todas las líneas se agreguen correctamente.
    /// 2. Todas las líneas puedan eliminarse.
    ///
    /// El test:
    /// - crea varias líneas
    /// - las agrega
    /// - comprueba la cantidad
    /// - luego las elimina
    /// - verifica que la lista quede vacía
    public void ThreadPoint_AddRemove_Parametrizado(int lineCount)
    {
        GameObject obj = new GameObject();
        ThreadPoint point = obj.AddComponent<ThreadPoint>();
        var lines = new List<ThreadLine>();

        for (int i = 0; i < lineCount; i++)
        {
            ThreadLine tl = new GameObject().AddComponent<ThreadLine>();
            point.AddConnection(tl);
            lines.Add(tl);
        }

        Assert.AreEqual(lineCount, point.Connections.Count, "todas las líneas agregadas");

        foreach (var l in lines) point.RemoveConnection(l);

        Assert.AreEqual(0, point.Connections.Count, "todas las líneas eliminadas");

        Object.DestroyImmediate(obj);
        foreach (var l in lines) Object.DestroyImmediate(l.gameObject);
    }

}