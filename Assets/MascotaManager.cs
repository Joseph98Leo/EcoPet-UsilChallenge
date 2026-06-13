using UnityEngine;
using UnityEngine.UI;

public class MascotaManager : MonoBehaviour
{
    [Header("Imágenes de los Animales")]
    public Sprite[] spritesAnimales; 

    [Header("Componente Visual")]
    public Image imagenMascota;

    void Start()
    {
        int idSeleccionado = PlayerPrefs.GetInt("PersonajeSeleccionado", 0);

        if (idSeleccionado >= 0 && idSeleccionado < spritesAnimales.Length)
        {
            imagenMascota.sprite = spritesAnimales[idSeleccionado];
        }
    }
}
