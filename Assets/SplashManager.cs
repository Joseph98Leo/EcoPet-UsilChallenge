using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour
{
    public float tiempoDeEspera = 3f;

    void Start()
    {
        StartCoroutine(CambiarPantalla());
    }

    IEnumerator CambiarPantalla()
    {
        yield return new WaitForSeconds(tiempoDeEspera);
        
        SceneManager.LoadScene("SeleccionScene"); 
    }
}