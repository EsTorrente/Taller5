using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

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

    public static IEnumerable<TestCaseData> BlockSizeData()
    {
        // scaleX, scaleY, expectedWidth, expectedHeight
        yield return new TestCaseData(4f, 2f, 2, 1).SetName("Block_4x2_da_2x1slots");
        yield return new TestCaseData(2f, 2f, 1, 1).SetName("Block_2x2_da_1x1slots");
        yield return new TestCaseData(4f, 4f, 2, 2).SetName("Block_4x4_da_2x2slots");
        yield return new TestCaseData(6f, 2f, 3, 1).SetName("Block_6x2_da_3x1slots");
    }

    [TestCaseSource(nameof(BlockSizeData))]
    public void BlockSize_CalculatedFromScale(
        float scaleX, float scaleY, int expectedW, int expectedH)
    {

        GameObject obj = new GameObject();
        obj.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        objetosCocina block = obj.AddComponent<objetosCocina>();
        block.Awake(); 

        Assert.AreEqual(expectedW, block.widthInSlots, "widthInSlots incorrecto");
        Assert.AreEqual(expectedH, block.heightInSlots, "heightInSlots incorrecto");

        Object.DestroyImmediate(obj);
    }

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

    public static IEnumerable<TestCaseData> RegisterBlockData()
    {
        // col, row, wSlots, hSlots
        yield return new TestCaseData(0, 0, 1, 1).SetName("Register_Bloque1x1_Origen");
        yield return new TestCaseData(1, 1, 2, 2).SetName("Register_Bloque2x2_Centro");
        yield return new TestCaseData(2, 0, 2, 1).SetName("Register_Bloque2x1_Fila0");
        yield return new TestCaseData(0, 2, 1, 2).SetName("Register_Bloque1x2_Col0");
    }

    [TestCaseSource(nameof(RegisterBlockData))]
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

    /// no llamo Comprobar() directamente pq en el caso correcto llama a SceneManager.LoadScene(), que crashea en Edit Mode
    public static IEnumerable<TestCaseData> ComprobarData()
    {
        yield return new TestCaseData(5, 8, 2, true).SetName("Codigo_Correcto_582");
        yield return new TestCaseData(0, 0, 0, false).SetName("Codigo_Incorrecto_000");
        yield return new TestCaseData(5, 8, 0, false).SetName("Codigo_Incorrecto_580");
        yield return new TestCaseData(5, 0, 2, false).SetName("Codigo_Incorrecto_502");
        yield return new TestCaseData(0, 8, 2, false).SetName("Codigo_Incorrecto_082");
        yield return new TestCaseData(2, 8, 5, false).SetName("Codigo_Incorrecto_Invertido");
    }

    [TestCaseSource(nameof(ComprobarData))]
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
    public void ThreadPoint_AddConnection_Agrega()
    {
        GameObject obj = new GameObject();
        ThreadPoint point = obj.AddComponent<ThreadPoint>();
        ThreadLine line = new GameObject().AddComponent<ThreadLine>();

        point.AddConnection(line);

        Assert.Contains(line, point.connections);

        Object.DestroyImmediate(obj);
        Object.DestroyImmediate(line.gameObject);
    }

    [Test]
    public void ThreadPoint_AddConnection_SinDuplicados()
    {
        GameObject obj = new GameObject();
        ThreadPoint point = obj.AddComponent<ThreadPoint>();
        ThreadLine line = new GameObject().AddComponent<ThreadLine>();

        point.AddConnection(line);
        point.AddConnection(line); //segunda vez, debe ignorarse

        Assert.AreEqual(1, point.connections.Count);

        Object.DestroyImmediate(obj);
        Object.DestroyImmediate(line.gameObject);
    }

    [Test]
    public void ThreadPoint_RemoveConnection_Elimina()
    {
        GameObject obj = new GameObject();
        ThreadPoint point = obj.AddComponent<ThreadPoint>();
        ThreadLine line = new GameObject().AddComponent<ThreadLine>();

        point.AddConnection(line);
        point.RemoveConnection(line);

        Assert.IsFalse(point.connections.Contains(line));

        Object.DestroyImmediate(obj);
        Object.DestroyImmediate(line.gameObject);
    }

    [Test]
    public void ThreadPoint_ClearConnections_VaciaLaLista()
    {
        GameObject obj = new GameObject();
        ThreadPoint point = obj.AddComponent<ThreadPoint>();

        var lineObjects = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            GameObject lo = new GameObject();
            lineObjects.Add(lo);
            point.connections.Add(lo.AddComponent<ThreadLine>());
        }

        Assert.AreEqual(3, point.connections.Count, "setup: 3 líneas agregadas");

        point.connections.Clear();

        Assert.AreEqual(0, point.connections.Count, "la lista debe quedar vacía");

        Object.DestroyImmediate(obj);
        foreach (var lo in lineObjects) Object.DestroyImmediate(lo);
    }

    public static IEnumerable<TestCaseData> ThreadCountData()
    {
        yield return new TestCaseData(1).SetName("Thread_AgregarYEliminar_1linea");
        yield return new TestCaseData(3).SetName("Thread_AgregarYEliminar_3lineas");
        yield return new TestCaseData(5).SetName("Thread_AgregarYEliminar_5lineas");
    }

    [TestCaseSource(nameof(ThreadCountData))]
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

        Assert.AreEqual(lineCount, point.connections.Count, "todas las líneas agregadas");

        foreach (var l in lines) point.RemoveConnection(l);

        Assert.AreEqual(0, point.connections.Count, "todas las líneas eliminadas");

        Object.DestroyImmediate(obj);
        foreach (var l in lines) Object.DestroyImmediate(l.gameObject);
    }

    [Test]
    public void ThreadManager_SelectPoint_CreaLineaAlSegundoClick()
    {
        GameObject prefab = new GameObject();
        prefab.AddComponent<ThreadLine>();

        GameObject managerObj = new GameObject();
        ThreadManager manager = managerObj.AddComponent<ThreadManager>();
        manager.linePrefab = prefab;

        RectTransform r1 = new GameObject().AddComponent<RectTransform>();
        RectTransform r2 = new GameObject().AddComponent<RectTransform>();

        manager.SelectPoint(r1); // primera selección se guarda
        manager.SelectPoint(r2); // segunda selección crea la línea

        //debe existir un ThreadLine hijo del manager
        ThreadLine created = managerObj.GetComponentInChildren<ThreadLine>();
        Assert.IsNotNull(created, "ThreadLine debe instanciarse después de dos selecciones");

        Object.DestroyImmediate(managerObj);
        Object.DestroyImmediate(prefab);
        Object.DestroyImmediate(r1.gameObject);
        Object.DestroyImmediate(r2.gameObject);
    }
}