using UnityEngine;
using UnityEngine.SceneManagement;

public class SeleccionManager : MonoBehaviour
{
    public void ElegirLoro() { GuardarYAvanzar(0); }
    public void ElegirTigre() { GuardarYAvanzar(1); }
    public void ElegirOso() { GuardarYAvanzar(2); }
    public void ElegirPinguino() { GuardarYAvanzar(3); }

    private void GuardarYAvanzar(int id)
    {
        PlayerPrefs.SetInt("PersonajeSeleccionado", id);
        PlayerPrefs.Save();

        SceneManager.LoadScene("SampleScene"); 
    }
}