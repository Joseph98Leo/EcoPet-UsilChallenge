using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PetManager : MonoBehaviour
{
    [Header("Sprites según limpieza")]
    public Sprite[] listaSucio;
    public Sprite[] listaLimpio;

    [Header("Mascota")]
    public Image imagenMascota;

    [Header("Stats (0-100)")]
    public float salud = 100f;
    public float felicidad = 100f;
    public float limpieza = 100f;

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

    private int idSeleccionado;

    private void Start()
    {
        idSeleccionado = PlayerPrefs.GetInt("PersonajeSeleccionado", 0);

        salud     = PlayerPrefs.GetFloat("MiSalud",     salud);
        felicidad = PlayerPrefs.GetFloat("MiFelicidad", felicidad);
        limpieza  = PlayerPrefs.GetFloat("MiLimpieza",  limpieza);

        ConfigurarSlider(sliderSalud);
        ConfigurarSlider(sliderFelicidad);
        ConfigurarSlider(sliderLimpieza);

        RefrescarUI();
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
}
