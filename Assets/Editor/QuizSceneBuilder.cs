using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
using UnityEngine.Events;

public static class QuizSceneBuilder
{
    // ─── Color palette ────────────────────────────────────────────────────
    static readonly Color ColVerde      = new Color(0.133f, 0.533f, 0.243f); // #22883E
    static readonly Color ColVerdeOsc   = new Color(0.10f,  0.40f,  0.18f);
    static readonly Color ColBgGreen    = new Color(0.910f, 0.965f, 0.910f); // #E8F7E8
    static readonly Color ColBlanco     = Color.white;
    static readonly Color ColGris       = new Color(0.96f,  0.96f,  0.96f);
    static readonly Color ColSombraCard = new Color(0.60f,  0.75f,  0.60f,  0.40f);
    static readonly Color ColTextoOsc   = new Color(0.15f,  0.15f,  0.15f);
    static readonly Color ColTextoMed   = new Color(0.35f,  0.35f,  0.35f);
    static readonly Color ColReading    = new Color(0.20f,  0.60f,  0.25f);
    static readonly Color ColListening  = new Color(0.18f,  0.48f,  0.88f);
    static readonly Color ColWriting    = new Color(0.92f,  0.55f,  0.12f);

    [MenuItem("EcoPet/Build QuizScene")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/QuizScene.unity");

        // ── Canvas ──────────────────────────────────────────────────────
        var canvasGO = GameObject.Find("Canvas");
        if (canvasGO == null) { Debug.LogError("Canvas not found!"); return; }

        for (int i = canvasGO.transform.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(canvasGO.transform.GetChild(i).gameObject);

        // ── GameManager / QuizManager ───────────────────────────────────
        var gmGO = GameObject.Find("GameManager");
        if (gmGO == null) gmGO = new GameObject("GameManager");
        QuizManager qm = gmGO.GetComponent<QuizManager>();
        if (qm == null) qm = gmGO.AddComponent<QuizManager>();

        // ── Background ──────────────────────────────────────────────────
        var bg = Img(canvasGO, "BgPanel", ColBgGreen);
        Stretch(bg);

        // ── Top Bar (green, 200px from top) ─────────────────────────────
        var topBar = Img(canvasGO, "TopBar", ColVerde);
        var rtTop  = RT(topBar);
        rtTop.anchorMin = new Vector2(0, 1); rtTop.anchorMax = new Vector2(1, 1);
        rtTop.pivot     = new Vector2(0.5f, 1f);
        rtTop.offsetMin = new Vector2(0, -200); rtTop.offsetMax = Vector2.zero;

        // Decorative green-dark footer line inside TopBar
        var lineBot = Img(topBar, "TopBarLine", new Color(0,0,0,0.12f));
        var rtLB    = RT(lineBot);
        rtLB.anchorMin = new Vector2(0,0); rtLB.anchorMax = new Vector2(1,0);
        rtLB.pivot     = new Vector2(0.5f,0f);
        rtLB.offsetMin = new Vector2(0,-3); rtLB.offsetMax = new Vector2(0,3);

        // Back button
        var btnBackGO  = Btn(topBar, "BtnBack");
        Center(btnBackGO, -445f, 0f, 80f, 80f);
        var btnBackImg = btnBackGO.GetComponent<UnityEngine.UI.Image>();
        btnBackImg.color = new Color(1,1,1,0.12f);
        var txtBack = TMP(btnBackGO, "TxtBack", "<  ", 52, ColBlanco);
        Stretch(txtBack); TMP_C(txtBack, TextAlignmentOptions.Center, true);

        // Title
        var txtTitulo = TMP(topBar, "TituloQuiz", "QUIZ", 60, ColBlanco);
        Center(txtTitulo, 0f, 0f, 500f, 90f);
        TMP_C(txtTitulo, TextAlignmentOptions.Center, true);

        // EcoPoints badge (right)
        var ecoBox = Img(topBar, "EcoBox", new Color(1,1,1,0.16f));
        Center(ecoBox, 390f, 0f, 185f, 75f);
        var txtEP_Label = TMP(ecoBox, "EpLabel", "EP", 26, new Color(0.7f,1f,0.6f));
        var rtEPL = RT(txtEP_Label);
        rtEPL.anchorMin = Vector2.zero; rtEPL.anchorMax = new Vector2(0.38f,1);
        rtEPL.offsetMin = Vector2.zero; rtEPL.offsetMax = Vector2.zero;
        TMP_C(txtEP_Label, TextAlignmentOptions.MidlineRight, true);
        var txtEP_Num = TMP(ecoBox, "TextoEcoPoints", "50", 40, ColBlanco);
        var rtEPN     = RT(txtEP_Num);
        rtEPN.anchorMin = new Vector2(0.42f,0); rtEPN.anchorMax = Vector2.one;
        rtEPN.offsetMin = Vector2.zero; rtEPN.offsetMax = Vector2.zero;
        TMP_C(txtEP_Num, TextAlignmentOptions.MidlineLeft, true);

        // ── Progress bar (below topBar) ──────────────────────────────────
        var progArea = Container(canvasGO, "ProgressArea");
        Center(progArea, 0f, 665f, 960f, 55f);

        var progTrack = Img(progArea, "ProgressTrack", new Color(0.82f, 0.88f, 0.82f));
        var rtPT = RT(progTrack);
        rtPT.anchorMin = new Vector2(0, 0.5f); rtPT.anchorMax = new Vector2(1, 0.5f);
        rtPT.pivot = new Vector2(0.5f, 0.5f);
        rtPT.offsetMin = new Vector2(0, -11); rtPT.offsetMax = new Vector2(-145, 11);

        var progFill = Img(progTrack, "ProgressFill", ColVerde);
        Stretch(progFill);
        var imgFill = progFill.GetComponent<UnityEngine.UI.Image>();
        imgFill.type       = UnityEngine.UI.Image.Type.Filled;
        imgFill.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
        imgFill.fillAmount = 0f;

        var txtProg = TMP(progArea, "TextoProgreso", "1 / 5", 32, new Color(0.25f,0.5f,0.25f));
        var rtTPG   = RT(txtProg);
        rtTPG.anchorMin = new Vector2(1,0); rtTPG.anchorMax = Vector2.one;
        rtTPG.pivot     = new Vector2(1,0.5f);
        rtTPG.offsetMin = new Vector2(-140, 0); rtTPG.offsetMax = Vector2.zero;
        TMP_C(txtProg, TextAlignmentOptions.MidlineRight, true);

        // ── Category badge ───────────────────────────────────────────────
        var catBadge = Img(canvasGO, "CategoryBadge", ColReading);
        Center(catBadge, 0f, 592f, 268f, 64f);
        var txtCat = TMP(catBadge, "TextoCategoria", "READING", 32, ColBlanco);
        Stretch(txtCat); TMP_C(txtCat, TextAlignmentOptions.Center, true);

        // ── Question card ────────────────────────────────────────────────
        var cardSombra = Img(canvasGO, "CardSombra", ColSombraCard);
        Center(cardSombra, 5f, 274f, 972f, 472f);

        var card = Img(canvasGO, "QuestionCard", ColBlanco);
        Center(card, 0f, 278f, 960f, 465f);

        // Colored top-accent bar on card
        var catLine = Img(card, "CatLine", ColReading);
        var rtCL    = RT(catLine);
        rtCL.anchorMin = new Vector2(0,1); rtCL.anchorMax = Vector2.one;
        rtCL.pivot = new Vector2(0.5f,1f);
        rtCL.offsetMin = new Vector2(0,-10); rtCL.offsetMax = Vector2.zero;

        var txtPregunta = TMP(card, "TextoPregunta",
            "What does 'deforestation' mean?", 46, ColTextoOsc);
        var rtQ = RT(txtPregunta);
        rtQ.anchorMin = Vector2.zero; rtQ.anchorMax = Vector2.one;
        rtQ.offsetMin = new Vector2(55, 45); rtQ.offsetMax = new Vector2(-55, -55);
        var tmpQ = txtPregunta.GetComponent<TextMeshProUGUI>();
        tmpQ.alignment = TextAlignmentOptions.Center;
        tmpQ.enableWordWrapping = true;

        // ── Answer option buttons (2x2 grid) ─────────────────────────────
        string[] letras = { "A", "B", "C", "D" };
        float[]  xs     = { -268f,  268f, -268f,  268f };
        float[]  ys     = {  -90f,  -90f,  -325f, -325f };

        var botonesOpcion = new Button[4];
        var textosOpcion  = new TextMeshProUGUI[4];
        var letrasOpcion  = new TextMeshProUGUI[4];
        var bgOpcion      = new UnityEngine.UI.Image[4];

        for (int i = 0; i < 4; i++)
        {
            // Drop shadow
            var sombra = Img(canvasGO, "SombraOpc" + letras[i], new Color(0.65f,0.70f,0.65f,0.35f));
            Center(sombra, xs[i] + 4f, ys[i] - 4f, 456f, 214f);

            // Button container
            var opc    = new GameObject("Opcion" + letras[i]);
            opc.transform.SetParent(canvasGO.transform, false);
            opc.AddComponent<RectTransform>();
            var opcImg = opc.AddComponent<UnityEngine.UI.Image>();
            opcImg.color = ColGris;
            Center(opc, xs[i], ys[i], 450f, 210f);

            var btn    = opc.AddComponent<Button>();
            var colors = btn.colors;
            colors.highlightedColor = new Color(0.88f, 0.95f, 0.88f);
            colors.pressedColor     = new Color(0.75f, 0.90f, 0.75f);
            btn.colors = colors;

            // Letter badge (left)
            var letterBadge = Img(opc, "LetterBadge" + letras[i], ColReading);
            Center(letterBadge, -158f, 0f, 88f, 88f);

            var txtLetra = TMP(letterBadge, "Letra" + letras[i], letras[i], 46, ColBlanco);
            Stretch(txtLetra); TMP_C(txtLetra, TextAlignmentOptions.Center, true);

            // Answer text (right of badge)
            var txtOpcion = TMP(opc, "TextoOpcion" + letras[i], "Option " + letras[i], 36, ColTextoOsc);
            var rtTO      = RT(txtOpcion);
            rtTO.anchorMin = Vector2.zero; rtTO.anchorMax = Vector2.one;
            rtTO.offsetMin = new Vector2(108, 10); rtTO.offsetMax = new Vector2(-14, -10);
            var tmpTO = txtOpcion.GetComponent<TextMeshProUGUI>();
            tmpTO.alignment = TextAlignmentOptions.MidlineLeft;
            tmpTO.enableWordWrapping = true;

            // Persistent listener with stored int arg
            int capturedI = i;
            UnityEventTools.AddIntPersistentListener(btn.onClick, qm.Responder, capturedI);
            EditorUtility.SetDirty(btn);

            botonesOpcion[i] = btn;
            textosOpcion[i]  = tmpTO;
            letrasOpcion[i]  = txtLetra.GetComponent<TextMeshProUGUI>();
            bgOpcion[i]      = opcImg;
        }

        // ── Feedback banner (hidden initially) ───────────────────────────
        var feedbackPanel = Img(canvasGO, "FeedbackPanel", ColVerde);
        Center(feedbackPanel, 0f, -575f, 1080f, 115f);
        feedbackPanel.SetActive(false);

        var txtFeedback = TMP(feedbackPanel, "TextoFeedback", "¡Correcto! +10 EcoPoints", 40, ColBlanco);
        Stretch(txtFeedback); TMP_C(txtFeedback, TextAlignmentOptions.Center, true);

        // ── Result panel (hidden initially, full-screen overlay) ─────────
        var resultPanel = Container(canvasGO, "ResultPanel");
        Stretch(resultPanel);
        var resultPanelImg = resultPanel.AddComponent<UnityEngine.UI.Image>();
        resultPanelImg.color = new Color(0, 0, 0, 0.72f);
        resultPanel.SetActive(false);

        // Result card
        var resultCard = Img(resultPanel, "ResultCard", ColBlanco);
        Center(resultCard, 0f, 60f, 880f, 920f);

        // Top green section (trophy/stars area)
        var resTopGreen = Img(resultCard, "ResTop", ColVerde);
        var rtRTG = RT(resTopGreen);
        rtRTG.anchorMin = new Vector2(0, 0.52f); rtRTG.anchorMax = Vector2.one;
        rtRTG.offsetMin = Vector2.zero; rtRTG.offsetMax = Vector2.zero;

        // Trophy star decoration
        var txtStars = TMP(resTopGreen, "Stars", "* * *", 64, new Color(1f, 0.92f, 0.22f));
        var rtSt     = RT(txtStars);
        rtSt.anchorMin = new Vector2(0.05f, 0.02f); rtSt.anchorMax = new Vector2(0.95f, 0.48f);
        rtSt.offsetMin = Vector2.zero; rtSt.offsetMax = Vector2.zero;
        TMP_C(txtStars, TextAlignmentOptions.Center, true);

        // Title ("¡Excelente!")
        var txtResTitle = TMP(resTopGreen, "TituloResultado", "¡Excelente!", 68, ColBlanco);
        var rtRT = RT(txtResTitle);
        rtRT.anchorMin = new Vector2(0.05f, 0.48f); rtRT.anchorMax = new Vector2(0.95f, 1f);
        rtRT.offsetMin = Vector2.zero; rtRT.offsetMax = Vector2.zero;
        TMP_C(txtResTitle, TextAlignmentOptions.Center, true);

        // Score detail ("4 / 5 correctas")
        var txtResDetalle = TMP(resultCard, "DetalleResultado", "4 / 5 correctas", 48, ColTextoMed);
        var rtRD = RT(txtResDetalle);
        rtRD.anchorMin = new Vector2(0.05f, 0.30f); rtRD.anchorMax = new Vector2(0.95f, 0.52f);
        rtRD.offsetMin = Vector2.zero; rtRD.offsetMax = Vector2.zero;
        TMP_C(txtResDetalle, TextAlignmentOptions.Center, false);

        // EcoPoints gained
        var txtResPoints = TMP(resultCard, "PuntosGanados", "+40 EcoPoints", 46, ColVerde);
        var rtRPts = RT(txtResPoints);
        rtRPts.anchorMin = new Vector2(0.1f, 0.17f); rtRPts.anchorMax = new Vector2(0.9f, 0.30f);
        rtRPts.offsetMin = Vector2.zero; rtRPts.offsetMax = Vector2.zero;
        TMP_C(txtResPoints, TextAlignmentOptions.Center, true);

        // Button: Volver a Mascota
        var btnVolverGO  = Btn(resultCard, "BtnVolverMascota");
        var rtBVM = RT(btnVolverGO);
        rtBVM.anchorMin = new Vector2(0.06f, 0.04f); rtBVM.anchorMax = new Vector2(0.94f, 0.16f);
        rtBVM.offsetMin = Vector2.zero; rtBVM.offsetMax = Vector2.zero;
        btnVolverGO.GetComponent<UnityEngine.UI.Image>().color = ColVerde;
        var txtBtnV = TMP(btnVolverGO, "TxtBtnVolver", "Volver a Mascota", 42, ColBlanco);
        Stretch(txtBtnV); TMP_C(txtBtnV, TextAlignmentOptions.Center, true);
        var btnVolverComp = btnVolverGO.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(btnVolverComp.onClick, qm.VolverAMascota);
        EditorUtility.SetDirty(btnVolverComp);

        // ── Wire back button ─────────────────────────────────────────────
        var btnBackComp = btnBackGO.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(btnBackComp.onClick, qm.VolverAMascota);
        EditorUtility.SetDirty(btnBackComp);

        // ── Wire QuizManager references ──────────────────────────────────
        qm.textoEcoPoints        = txtEP_Num.GetComponent<TextMeshProUGUI>();
        qm.textoProgreso         = txtProg.GetComponent<TextMeshProUGUI>();
        qm.fillProgreso          = imgFill;
        qm.textoCategoria        = txtCat.GetComponent<TextMeshProUGUI>();
        qm.iconoCategoria        = catLine.GetComponent<UnityEngine.UI.Image>();
        qm.textoPregunta         = tmpQ;
        qm.botonesOpcion         = botonesOpcion;
        qm.textosOpcion          = textosOpcion;
        qm.letrasOpcion          = letrasOpcion;
        qm.bgOpcion              = bgOpcion;
        qm.panelFeedback         = feedbackPanel;
        qm.textoFeedback         = txtFeedback.GetComponent<TextMeshProUGUI>();
        qm.bgFeedback            = feedbackPanel.GetComponent<UnityEngine.UI.Image>();
        qm.panelResultado        = resultPanel;
        qm.textoResultadoTitulo  = txtResTitle.GetComponent<TextMeshProUGUI>();
        qm.textoResultadoDetalle = txtResDetalle.GetComponent<TextMeshProUGUI>();
        qm.textoGanados          = txtResPoints.GetComponent<TextMeshProUGUI>();

        // ── Quiz questions ────────────────────────────────────────────────
        qm.preguntas = new Pregunta[]
        {
            new Pregunta
            {
                categoria      = "Reading",
                enunciado      = "What does 'deforestation' mean?\n(¿Qué significa 'deforestación'?)",
                opciones       = new[]{ "Planting more trees", "Removing trees from forests", "Protecting forests", "Growing city gardens" },
                indiceCorrecto = 1
            },
            new Pregunta
            {
                categoria      = "Listening",
                enunciado      = "Which action helps REDUCE pollution?\n(¿Qué acción ayuda a REDUCIR la contaminación?)",
                opciones       = new[]{ "Burning garbage", "Using plastic bags", "Riding a bicycle", "Leaving lights on" },
                indiceCorrecto = 2
            },
            new Pregunta
            {
                categoria      = "Writing",
                enunciado      = "Complete: 'We must _____ water to protect our planet.'\n('Debemos _____ el agua para proteger nuestro planeta.')",
                opciones       = new[]{ "waste", "pollute", "ignore", "save" },
                indiceCorrecto = 3
            },
            new Pregunta
            {
                categoria      = "Reading",
                enunciado      = "What is 'climate change'?\n(¿Qué es el 'cambio climático'?)",
                opciones       = new[]{ "Changes in weather over long periods", "Daily temperature shifts", "The four seasons changing", "Rain during summer" },
                indiceCorrecto = 0
            },
            new Pregunta
            {
                categoria      = "Writing",
                enunciado      = "Which word means 'to use something again'?\n(¿Cuál palabra significa 'volver a usar algo'?)",
                opciones       = new[]{ "Dispose", "Purchase", "Recycle", "Waste" },
                indiceCorrecto = 2
            },
        };

        EditorUtility.SetDirty(qm);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("[EcoPet] QuizScene rebuilt successfully!");
    }

    // ─── Layout helpers ───────────────────────────────────────────────────

    static RectTransform RT(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        return rt != null ? rt : go.AddComponent<RectTransform>();
    }

    static GameObject Img(GameObject parent, string name, Color col)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<UnityEngine.UI.Image>().color = col;
        return go;
    }

    static GameObject Btn(GameObject parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<UnityEngine.UI.Image>().color = Color.white;
        go.AddComponent<Button>();
        return go;
    }

    static GameObject TMP(GameObject parent, string name, string text, float size, Color col)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text  = text;
        tmp.fontSize = size;
        tmp.color = col;
        tmp.enableWordWrapping = true;
        return go;
    }

    static GameObject Container(GameObject parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        return go;
    }

    static void TMP_C(GameObject go, TextAlignmentOptions align, bool bold)
    {
        var tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp == null) return;
        tmp.alignment = align;
        if (bold) tmp.fontStyle = FontStyles.Bold;
    }

    static void Center(GameObject go, float x, float y, float w, float h)
    {
        var rt = RT(go);
        rt.anchorMin        = new Vector2(0.5f, 0.5f);
        rt.anchorMax        = new Vector2(0.5f, 0.5f);
        rt.pivot            = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(x, y);
        rt.sizeDelta        = new Vector2(w, h);
    }

    static void Stretch(GameObject go)
    {
        var rt = RT(go);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
