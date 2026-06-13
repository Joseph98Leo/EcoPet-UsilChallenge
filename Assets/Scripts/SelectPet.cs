using UnityEngine;

public class SelectPet : MonoBehaviour
{
    public void Seleccionar(int id)
    {
        PlayerPrefs.SetInt("ActualPet", id);
        PlayerPrefs.Save();
    }
}
