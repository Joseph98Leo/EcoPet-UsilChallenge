using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PetManager : MonoBehaviour
{
    private const string KeyUltimaLimpieza = "UltimaLimpiezaTicks";
    private const float HorasParaEnsuciarse = 24f;
    private const float LimpiezaAlEnsuciarse = 20f;

    [Header("Sprites según limpieza")]
    public Sprite[] listaSucio;
    public Sprite[] listaLimpio;

    [Header("Mascota")]
    public Image imagenMascota;

    [Header("Zona de interacción (opcional)")]
    [Tooltip("RectTransform vacío ajustado al tamaño real del sprite del pet, usado para detectar comida/esponja y como centro de la espuma. Si se deja vacío se usa el RectTransform completo de 'imagenMascota'.")]
    public RectTransform zonaInteraccion;

    [Header("Stats (0-100)")]
    public float salud = 20f;
    public float felicidad = 10f;
    public float limpieza = 20f;

    [Header("Sliders")]
    public Slider sliderSalud;
    public Slider sliderFelicidad;
    public Slider sliderLimpieza;

    [Header("Textos de porcentaje")]
    public TextMeshProUGUI textoPctSalud;
    public TextMeshProUGUI textoPctFelicidad;
    public TextMeshProUGUI textoPctLimpieza;

    [Header("Botones de interacción (sin lógica aún)")]
    public Button botonAccion1;
    public Button botonAccion2;
    public Button botonAccion3;
    public Button botonAccion4;

    [Header("Audio")]
    public AudioSource audioSource;

    private int idSeleccionado;

    public RectTransform PetRect => zonaInteraccion != null ? zonaInteraccion : (imagenMascota != null ? imagenMascota.rectTransform : null);

    private void Awake()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        idSeleccionado = PlayerPrefs.GetInt("ActualPet", 0);

        salud     = PlayerPrefs.GetFloat("MiSalud",     salud);
        felicidad = PlayerPrefs.GetFloat("MiFelicidad", felicidad);
        limpieza  = PlayerPrefs.GetFloat("MiLimpieza",  limpieza);

        RevisarSiSeEnsucio();

        ConfigurarSlider(sliderSalud);
        ConfigurarSlider(sliderFelicidad);
        ConfigurarSlider(sliderLimpieza);

        RefrescarUI();
    }

    /// <summary>Si pasaron 24h desde el último baño, vuelve a ensuciarse.</summary>
    private void RevisarSiSeEnsucio()
    {
        string ticksGuardados = PlayerPrefs.GetString(KeyUltimaLimpieza, string.Empty);
        if (string.IsNullOrEmpty(ticksGuardados)) return;
        if (!long.TryParse(ticksGuardados, out long ticks)) return;

        double horasTranscurridas = (DateTime.UtcNow - new DateTime(ticks, DateTimeKind.Utc)).TotalHours;
        if (horasTranscurridas >= HorasParaEnsuciarse)
        {
            limpieza = LimpiezaAlEnsuciarse;
            GuardarEstado();
        }
    }

    private void ConfigurarSlider(Slider slider)
    {
        if (slider == null) return;
        slider.minValue = 0f;
        slider.maxValue = 100f;
    }

    public void RefrescarUI()
    {
        ActualizarBarra(sliderSalud, textoPctSalud, salud);
        ActualizarBarra(sliderFelicidad, textoPctFelicidad, felicidad);
        ActualizarBarra(sliderLimpieza, textoPctLimpieza, limpieza);

        ActualizarSprite();
    }

    private void ActualizarBarra(Slider slider, TextMeshProUGUI texto, float valor)
    {
        if (slider != null) slider.value = valor;
        if (texto != null) texto.text = Mathf.RoundToInt(valor) + "%";
    }

    private void ActualizarSprite()
    {
        if (imagenMascota == null) return;

        Sprite[] lista = limpieza < 50f ? listaSucio : listaLimpio;

        if (lista != null && idSeleccionado >= 0 && idSeleccionado < lista.Length && lista[idSeleccionado] != null)
            imagenMascota.sprite = lista[idSeleccionado];
    }

    public void Alimentar(float cantidad)
    {
        salud = Mathf.Clamp(salud + cantidad, 0f, 100f);
        GuardarEstado();
        RefrescarUI();
    }

    public void ReproducirSonido(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip);
    }

    /// <summary>Llamado por el botón 3: va al quiz. La felicidad ganada ahí se aplica sola al volver.</summary>
    public void IrAQuiz()
    {
        GameSceneManager.LoadScene("QuizScene");
    }

    /// <summary>Llamado cuando termina el baño (la manguera quitó suficiente espuma).</summary>
    public void Limpiar()
    {
        limpieza = 100f;
        PlayerPrefs.SetString(KeyUltimaLimpieza, DateTime.UtcNow.Ticks.ToString());
        GuardarEstado();
        RefrescarUI();
    }

    private void GuardarEstado()
    {
        PlayerPrefs.SetFloat("MiSalud", salud);
        PlayerPrefs.SetFloat("MiFelicidad", felicidad);
        PlayerPrefs.SetFloat("MiLimpieza", limpieza);
        PlayerPrefs.Save();
    }
}
