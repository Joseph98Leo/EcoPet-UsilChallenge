using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginMascota : MonoBehaviour
{
    [Header("Estadísticas")]
    public float salud     = 100f;
    public float felicidad = 100f;
    public float limpieza  = 100f;
    public int   ecoPoints = 50;

    [Header("Fill images de barras")]
    public UnityEngine.UI.Image fillSalud;
    public UnityEngine.UI.Image fillFelicidad;
    public UnityEngine.UI.Image fillLimpieza;

    [Header("Textos de porcentaje")]
    public TextMeshProUGUI textoPctSalud;
    public TextMeshProUGUI textoPctFelicidad;
    public TextMeshProUGUI textoPctLimpieza;

    [Header("Otros textos")]
    public TextMeshProUGUI textoEcoPoints;
    public TextMeshProUGUI textoEstado;

    [Header("Mascota")]
    public UnityEngine.UI.Image imagenMascota;
    public Sprite[] spritesFelices;   // orden: 0=Loro 1=Tigre 2=Oso 3=Pinguino
    public Sprite[] spritesTristes;

    [Header("Info mascota")]
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoEspecie;
    public TextMeshProUGUI textoBadge;
    public UnityEngine.UI.Image imagenBadge;

    static readonly string[] Nombres = { "Kusi", "Rumi", "Wayra", "Yaku" };
    static readonly string[] Especies = {
        "Loro de Alas Amarillas · Yellow-winged Parrot",
        "Jaguar Amazonico · Amazonian Jaguar",
        "Oso de Anteojos · Spectacled Bear",
        "Pinguino de Humboldt · Humboldt Penguin"
    };

    [Header("Velocidades de decay (por segundo)")]
    public float velocidadSalud     = 1.5f;
    public float velocidadFelicidad = 1.0f;
    public float velocidadLimpieza  = 0.8f;

    static readonly Color ColorSalud     = new Color(0.90f, 0.22f, 0.22f);
    static readonly Color ColorFelicidad = new Color(1.00f, 0.75f, 0.00f);
    static readonly Color ColorLimpieza  = new Color(0.22f, 0.60f, 0.95f);

    int idSeleccionado;

    // ─── Ciclo de vida ──────────────────────────────────────────────────────

    void Start()
    {
        salud      = PlayerPrefs.GetFloat("MiSalud",     100f);
        felicidad  = PlayerPrefs.GetFloat("MiFelicidad", 100f);
        limpieza   = PlayerPrefs.GetFloat("MiLimpieza",  100f);
        ecoPoints  = PlayerPrefs.GetInt  ("MisEcoPoints", 50);
        idSeleccionado = PlayerPrefs.GetInt("PersonajeSeleccionado", 0);

        if (textoNombre  != null) textoNombre.text  = idSeleccionado < Nombres.Length  ? Nombres[idSeleccionado]  : "";
        if (textoEspecie != null) textoEspecie.text = idSeleccionado < Especies.Length ? Especies[idSeleccionado] : "";

        RefrescarUI();
    }

    void Update()
    {
        float dt = Time.deltaTime;
        salud      = Mathf.Clamp(salud     - velocidadSalud     * dt, 0f, 100f);
        felicidad  = Mathf.Clamp(felicidad - velocidadFelicidad * dt, 0f, 100f);
        limpieza   = Mathf.Clamp(limpieza  - velocidadLimpieza  * dt, 0f, 100f);
        RefrescarUI();
    }

    void OnApplicationPause(bool pausado)
    {
        if (pausado) GuardarEstado();
    }

    // ─── UI ────────────────────────────────────────────────────────────────

    void RefrescarUI()
    {
        SetPct(textoPctSalud,     salud);
        SetPct(textoPctFelicidad, felicidad);
        SetPct(textoPctLimpieza,  limpieza);

        SetBarra(fillSalud,     salud);
        SetBarra(fillFelicidad, felicidad);
        SetBarra(fillLimpieza,  limpieza);

        if (textoEcoPoints != null)
            textoEcoPoints.text = ecoPoints.ToString();

        ActualizarSprite();
        ActualizarEstado();
    }

    void SetBarra(UnityEngine.UI.Image img, float pct)
    {
        if (img == null) return;
        var rt = img.rectTransform;
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(Mathf.Clamp01(pct / 100f), 1f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    void SetPct(TextMeshProUGUI label, float valor)
    {
        if (label != null) label.text = Mathf.RoundToInt(valor) + "%";
    }

    void ActualizarSprite()
    {
        if (imagenMascota == null) return;
        if (idSeleccionado < 0) return;

        bool triste = salud < 50f || felicidad < 50f || limpieza < 50f;
        var arr = triste ? spritesTristes : spritesFelices;

        if (arr != null && idSeleccionado < arr.Length && arr[idSeleccionado] != null)
            imagenMascota.sprite = arr[idSeleccionado];
    }

    void ActualizarEstado()
    {
        if (textoEstado != null) textoEstado.text = string.Empty;

        if (textoBadge == null && imagenBadge == null) return;

        bool muyTriste = salud < 30f || felicidad < 30f || limpieza < 30f;
        bool triste    = salud < 50f || felicidad < 50f || limpieza < 50f;
        bool feliz     = salud > 80f && felicidad > 80f && limpieza > 80f;

        string badgeTexto;
        Color  badgeBg;
        Color  badgeFg;

        if (muyTriste)
        {
            badgeTexto = "Muy triste · Very sad";
            badgeBg    = new Color(1.00f, 0.72f, 0.72f);
            badgeFg    = new Color(0.72f, 0.10f, 0.10f);
        }
        else if (triste)
        {
            badgeTexto = "Triste · Sad";
            badgeBg    = new Color(1.00f, 0.82f, 0.75f);
            badgeFg    = new Color(0.75f, 0.25f, 0.10f);
        }
        else if (feliz)
        {
            badgeTexto = "Feliz · Happy";
            badgeBg    = new Color(0.75f, 0.95f, 0.78f);
            badgeFg    = new Color(0.10f, 0.50f, 0.18f);
        }
        else
        {
            badgeTexto = "Bien · OK";
            badgeBg    = new Color(0.78f, 0.90f, 1.00f);
            badgeFg    = new Color(0.10f, 0.35f, 0.70f);
        }

        if (textoBadge   != null) { textoBadge.text  = badgeTexto; textoBadge.color = badgeFg; }
        if (imagenBadge  != null) imagenBadge.color  = badgeBg;
    }

    // ─── Acciones del jugador ──────────────────────────────────────────────

    public void Alimentar()
    {
        if (ecoPoints < 10) return;
        ecoPoints -= 10;
        salud = Mathf.Min(salud + 25f, 100f);
        PlayerPrefs.SetInt("MisEcoPoints", ecoPoints);
        RefrescarUI();
        StartCoroutine(AnimarMascota());
    }

    public void Limpiar()
    {
        limpieza = Mathf.Min(limpieza + 25f, 100f);
        RefrescarUI();
        StartCoroutine(AnimarMascota());
    }

    public void Jugar()
    {
        felicidad = Mathf.Min(felicidad + 25f, 100f);
        RefrescarUI();
        StartCoroutine(AnimarMascota());
    }

    public void IrAlMapa()
    {
        GuardarEstado();
        SceneManager.LoadScene("MapaScene");
    }

    // ─── Animación ─────────────────────────────────────────────────────────

    IEnumerator AnimarMascota()
    {
        if (imagenMascota == null) yield break;
        RectTransform rt = imagenMascota.rectTransform;
        Vector3 escalaBase   = rt.localScale;
        Vector3 escalaGrande = escalaBase * 1.18f;
        float dur = 0.1f;

        for (float t = 0f; t < dur; t += Time.deltaTime)
        {
            rt.localScale = Vector3.Lerp(escalaBase, escalaGrande, t / dur);
            yield return null;
        }
        for (float t = 0f; t < dur; t += Time.deltaTime)
        {
            rt.localScale = Vector3.Lerp(escalaGrande, escalaBase, t / dur);
            yield return null;
        }
        rt.localScale = escalaBase;
    }

    // ─── Persistencia ──────────────────────────────────────────────────────

    void GuardarEstado()
    {
        PlayerPrefs.SetFloat("MiSalud",     salud);
        PlayerPrefs.SetFloat("MiFelicidad", felicidad);
        PlayerPrefs.SetFloat("MiLimpieza",  limpieza);
        PlayerPrefs.SetInt  ("MisEcoPoints", ecoPoints);
        PlayerPrefs.Save();
    }
}
