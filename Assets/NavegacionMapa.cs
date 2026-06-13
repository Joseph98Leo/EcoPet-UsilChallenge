using System.Collections; // ¡Librería obligatoria para las Corrutinas!
using UnityEngine;
using UnityEngine.UI;

public class NavegacionMapa : MonoBehaviour
{
    [Header("Elementos de Progresión")]
    public GameObject botonNivel2;
    public Image linea1_2;

    void Start()
    {
        int nivelDesbloqueado = PlayerPrefs.GetInt("NivelDesbloqueado", 1);

        if (nivelDesbloqueado >= 2)
        {
            botonNivel2.SetActive(true);
            
            int debeAnimar = PlayerPrefs.GetInt("AnimarNuevoNivel", 0);
            
            if (debeAnimar == 1)
            {
                StartCoroutine(AnimarLinea());
                PlayerPrefs.SetInt("AnimarNuevoNivel", 0); 
            }
            else
            {
                linea1_2.fillAmount = 1f; 
            }
        }
        else
        {
            botonNivel2.SetActive(false);
            linea1_2.fillAmount = 0f;
        }
    }

    // --- NUEVA FUNCIÓN PARA ANIMAR ---
    IEnumerator AnimarLinea()
    {
        linea1_2.fillAmount = 0f;
        float duracionAnimacion = 1.5f;
        float tiempoPasado = 0f;

        while (tiempoPasado < duracionAnimacion)
        {
            tiempoPasado += Time.deltaTime; 
            
            linea1_2.fillAmount = tiempoPasado / duracionAnimacion;
            
            yield return null;
        }

        linea1_2.fillAmount = 1f; 
    }
    // ---------------------------------

    public void VolverAMascota()
    {
        GameSceneManager.LoadScene("SampleScene");
    }

    public void IrAlQuiz()
    {
        GameSceneManager.LoadScene("QuizScene");
    }
}