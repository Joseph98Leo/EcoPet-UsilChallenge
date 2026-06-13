using System.Collections;
using UnityEngine;

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

        GameSceneManager.LoadScene("SeleccionScene");
    }
}