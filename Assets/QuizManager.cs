using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class Pregunta
{
    public string categoria;
    public string enunciado;
    public string[] opciones;
    public int     indiceCorrecto;
}

public class QuizManager : MonoBehaviour
{
    // ─── Preguntas ─────────────────────────────────────────────────────────
    [Header("Banco de preguntas")]
    public Pregunta[] preguntas;

    // ─── UI ───────────────────────────────────────────────────────────────
    [Header("UI - Cabecera")]
    public TextMeshProUGUI textoEcoPoints;
    public TextMeshProUGUI textoProgreso;

    [Header("UI - Barra de progreso")]
    public UnityEngine.UI.Image fillProgreso;

    [Header("UI - Pregunta")]
    public TextMeshProUGUI textoCategoria;
    public UnityEngine.UI.Image  iconoCategoria;
    public TextMeshProUGUI textoPregunta;

    [Header("UI - Opciones (4 botones)")]
    public Button[]             botonesOpcion;
    public TextMeshProUGUI[]    textosOpcion;
    public TextMeshProUGUI[]    letrasOpcion;
    public UnityEngine.UI.Image[] bgOpcion;

    [Header("UI - Feedback")]
    public GameObject panelFeedback;
    public TextMeshProUGUI textoFeedback;
    public UnityEngine.UI.Image  bgFeedback;

    [Header("UI - Resultado final")]
    public GameObject panelResultado;
    public TextMeshProUGUI textoResultadoTitulo;
    public TextMeshProUGUI textoResultadoDetalle;
    public TextMeshProUGUI textoGanados;

    // ─── Colores ──────────────────────────────────────────────────────────
    static readonly Color ColNormal     = new Color(0.97f, 0.97f, 0.97f);
    static readonly Color ColCorrecto   = new Color(0.20f, 0.72f, 0.30f);
    static readonly Color ColIncorrecto = new Color(0.88f, 0.22f, 0.22f);
    static readonly Color ColLetraNorm  = new Color(0.25f, 0.25f, 0.25f);
    static readonly Color ColLetraAct   = Color.white;

    static readonly Color ColReading   = new Color(0.20f, 0.60f, 0.25f);
    static readonly Color ColListening = new Color(0.18f, 0.48f, 0.88f);
    static readonly Color ColWriting   = new Color(0.92f, 0.55f, 0.12f);

    // ─── Estado ───────────────────────────────────────────────────────────
    int  idx          = 0;
    int  correctas    = 0;
    int  puntosGanados = 0;
    bool respondido   = false;

    // ─── Ciclo de vida ────────────────────────────────────────────────────

    void Start()
    {
        if (panelResultado != null) panelResultado.SetActive(false);
        if (panelFeedback  != null) panelFeedback.SetActive(false);
        ActualizarEcoPoints();
        MostrarPregunta();
    }

    // ─── Lógica principal ─────────────────────────────────────────────────

    void MostrarPregunta()
    {
        if (idx >= preguntas.Length) { MostrarResultado(); return; }

        respondido = false;
        var p = preguntas[idx];

        // Progreso
        if (textoProgreso  != null) textoProgreso.text = (idx + 1) + " / " + preguntas.Length;
        if (fillProgreso   != null) fillProgreso.fillAmount = (float)(idx) / preguntas.Length;

        // Categoría
        if (textoCategoria != null)
        {
            textoCategoria.text  = p.categoria.ToUpper();
            textoCategoria.color = Color.white;
        }
        if (iconoCategoria != null) iconoCategoria.color = ColorCategoria(p.categoria);

        // Enunciado
        if (textoPregunta != null) textoPregunta.text = p.enunciado;

        // Opciones
        for (int i = 0; i < botonesOpcion.Length && i < 4; i++)
        {
            if (textosOpcion[i] != null) textosOpcion[i].text = p.opciones[i];
            if (bgOpcion[i]     != null) bgOpcion[i].color = ColNormal;
            if (letrasOpcion[i] != null) letrasOpcion[i].color = ColLetraNorm;
            if (textosOpcion[i] != null) textosOpcion[i].color = ColLetraNorm;
            botonesOpcion[i].interactable = true;
        }

        if (panelFeedback != null) panelFeedback.SetActive(false);
    }

    public void Responder(int opcionIdx)
    {
        if (respondido) return;
        respondido = true;

        var p = preguntas[idx];
        bool correcto = opcionIdx == p.indiceCorrecto;

        // Desactivar botones
        foreach (var b in botonesOpcion) b.interactable = false;

        // Colorear feedback en botones
        for (int i = 0; i < botonesOpcion.Length && i < 4; i++)
        {
            Color col;
            if      (i == p.indiceCorrecto) col = ColCorrecto;
            else if (i == opcionIdx)        col = ColIncorrecto;
            else                            col = ColNormal;

            if (bgOpcion[i]     != null) bgOpcion[i].color = col;
            if (letrasOpcion[i] != null) letrasOpcion[i].color = (i == p.indiceCorrecto || i == opcionIdx) ? ColLetraAct : ColLetraNorm;
            if (textosOpcion[i] != null) textosOpcion[i].color  = (i == p.indiceCorrecto || i == opcionIdx) ? ColLetraAct : ColLetraNorm;
        }

        // Feedback banner
        if (correcto)
        {
            correctas++;
            puntosGanados += 10;

            float felicidad = PlayerPrefs.GetFloat("MiFelicidad", 100f);
            felicidad = Mathf.Min(felicidad + 15f, 100f);
            PlayerPrefs.SetFloat("MiFelicidad", felicidad);

            MostrarFeedback(true, "¡Correcto! +10 EP  +15 Felicidad");
        }
        else
        {
            MostrarFeedback(false, "¡Casi! La respuesta era: " + p.opciones[p.indiceCorrecto]);
        }

        StartCoroutine(EsperarYSiguiente(1.8f));
    }

    void MostrarFeedback(bool ok, string msg)
    {
        if (panelFeedback == null) return;
        panelFeedback.SetActive(true);
        if (textoFeedback != null) textoFeedback.text = msg;
        if (bgFeedback    != null) bgFeedback.color   = ok ? ColCorrecto : ColIncorrecto;
    }

    IEnumerator EsperarYSiguiente(float seg)
    {
        yield return new WaitForSeconds(seg);
        idx++;
        MostrarPregunta();
        ActualizarEcoPoints();
    }

    void MostrarResultado()
    {
        // Guardar puntos ganados
        int pts = PlayerPrefs.GetInt("MisEcoPoints", 50) + puntosGanados;
        PlayerPrefs.SetInt("MisEcoPoints", pts);

        if (correctas >= preguntas.Length / 2)
        {
            int nivel = PlayerPrefs.GetInt("NivelDesbloqueado", 1);
            if (nivel == 1)
            {
                PlayerPrefs.SetInt("NivelDesbloqueado", 2);
                PlayerPrefs.SetInt("AnimarNuevoNivel",  1);
            }
        }
        PlayerPrefs.Save();

        if (panelFeedback  != null) panelFeedback.SetActive(false);
        if (panelResultado != null) panelResultado.SetActive(true);

        string titulo  = correctas >= preguntas.Length * 0.8f ? "¡Excelente!" :
                         correctas >= preguntas.Length * 0.5f ? "¡Buen trabajo!" : "¡Sigue intentando!";
        int felicidadGanada = correctas * 15;
        if (textoResultadoTitulo  != null) textoResultadoTitulo.text  = titulo;
        if (textoResultadoDetalle != null) textoResultadoDetalle.text = correctas + " / " + preguntas.Length + " correctas";
        if (textoGanados          != null) textoGanados.text          = "+" + puntosGanados + " EcoPoints  •  +" + felicidadGanada + " Felicidad";

        ActualizarEcoPoints();
    }

    void ActualizarEcoPoints()
    {
        if (textoEcoPoints != null)
            textoEcoPoints.text = PlayerPrefs.GetInt("MisEcoPoints", 50).ToString();
    }

    // ─── Navegación ───────────────────────────────────────────────────────

    public void VolverAlMapa()     { SceneManager.LoadScene("MapaScene");    }
    public void VolverAMascota()   { SceneManager.LoadScene("SampleScene");  }
    public void ReiniciarQuiz()    { SceneManager.LoadScene("QuizScene");    }

    public void RespuestaCorrecta()   => Responder(preguntas[idx].indiceCorrecto);
    public void RespuestaIncorrecta() => Responder((preguntas[idx].indiceCorrecto + 1) % 4);

    // ─── Util ─────────────────────────────────────────────────────────────

    Color ColorCategoria(string cat)
    {
        if (cat == "Listening") return ColListening;
        if (cat == "Writing")   return ColWriting;
        return ColReading;
    }
}
