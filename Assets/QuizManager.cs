using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private const int   CantidadPreguntas          = 3;
    private const float DuracionFeedbackIncorrecto = 1.5f;
    private const float DuracionParpadeo           = 0.5f;
    private const int   RepeticionesParpadeo       = 3;
    private const float EsperaRegreso              = 2f;

    // ─── Preguntas ─────────────────────────────────────────────────────────
    [Header("Banco de preguntas")]
    public Pregunta[] preguntas;

    // ─── UI ───────────────────────────────────────────────────────────────
    [Header("UI - Cabecera")]
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

    [Header("Mascota")]
    public UnityEngine.UI.Image imagenMascota;
    public Sprite[] listaNormal;
    public Sprite[] listaFeliz;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoIncorrecto;

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
    Pregunta[] preguntasSesion;
    int  idSeleccionado;
    int  idx        = 0;
    bool respondido = false;

    // ─── Ciclo de vida ────────────────────────────────────────────────────

    void Awake()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        idSeleccionado  = PlayerPrefs.GetInt("ActualPet", 0);
        preguntasSesion = SeleccionarPreguntasAleatorias(CantidadPreguntas);

        if (panelFeedback != null) panelFeedback.SetActive(false);

        MostrarPregunta();
    }

    // ─── Lógica principal ─────────────────────────────────────────────────

    Pregunta[] SeleccionarPreguntasAleatorias(int cantidad)
    {
        List<Pregunta> disponibles = new List<Pregunta>(preguntas);
        List<Pregunta> seleccion = new List<Pregunta>();

        cantidad = Mathf.Min(cantidad, disponibles.Count);
        for (int i = 0; i < cantidad; i++)
        {
            int elegido = Random.Range(0, disponibles.Count);
            seleccion.Add(disponibles[elegido]);
            disponibles.RemoveAt(elegido);
        }
        return seleccion.ToArray();
    }

    void MostrarPregunta()
    {
        respondido = false;
        var p = preguntasSesion[idx];

        MostrarSprite(listaNormal);

        // Progreso
        if (textoProgreso  != null) textoProgreso.text = (idx + 1) + " / " + preguntasSesion.Length;
        if (fillProgreso   != null) fillProgreso.fillAmount = (float)idx / preguntasSesion.Length;

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

        var p = preguntasSesion[idx];
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
            float felicidad = PlayerPrefs.GetFloat("MiFelicidad", 100f);
            felicidad = Mathf.Min(felicidad + 15f, 100f);
            PlayerPrefs.SetFloat("MiFelicidad", felicidad);

            ReproducirSonido(sonidoCorrecto);
            MostrarFeedback(true, "¡Correcto! +15 Felicidad");
        }
        else
        {
            ReproducirSonido(sonidoIncorrecto);
            MostrarFeedback(false, "¡Casi! La respuesta era: " + p.opciones[p.indiceCorrecto]);
        }

        StartCoroutine(ProcesarSiguiente(correcto));
    }

    void MostrarFeedback(bool ok, string msg)
    {
        if (panelFeedback == null) return;
        panelFeedback.SetActive(true);
        if (textoFeedback != null) textoFeedback.text = msg;
        if (bgFeedback    != null) bgFeedback.color   = ok ? ColCorrecto : ColIncorrecto;
    }

    IEnumerator ProcesarSiguiente(bool correcto)
    {
        if (correcto)
        {
            for (int i = 0; i < RepeticionesParpadeo; i++)
            {
                MostrarSprite(listaFeliz);
                yield return new WaitForSeconds(DuracionParpadeo);
                MostrarSprite(listaNormal);
                yield return new WaitForSeconds(DuracionParpadeo);
            }
            MostrarSprite(listaFeliz);
        }
        else
        {
            yield return new WaitForSeconds(DuracionFeedbackIncorrecto);
        }

        idx++;

        if (idx >= preguntasSesion.Length)
        {
            yield return new WaitForSeconds(EsperaRegreso);
            GameSceneManager.LoadScene("SampleScene");
        }
        else
        {
            MostrarPregunta();
        }
    }

    void MostrarSprite(Sprite[] lista)
    {
        if (imagenMascota == null || lista == null) return;
        if (idSeleccionado >= 0 && idSeleccionado < lista.Length && lista[idSeleccionado] != null)
            imagenMascota.sprite = lista[idSeleccionado];
    }

    void ReproducirSonido(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip);
    }

    // ─── Navegación ───────────────────────────────────────────────────────

    public void VolverAlMapa()   { GameSceneManager.LoadScene("MapaScene");   }
    public void VolverAMascota() { GameSceneManager.LoadScene("SampleScene"); }

    // ─── Util ─────────────────────────────────────────────────────────────

    Color ColorCategoria(string cat)
    {
        if (cat == "Listening") return ColListening;
        if (cat == "Writing")   return ColWriting;
        return ColReading;
    }
}
