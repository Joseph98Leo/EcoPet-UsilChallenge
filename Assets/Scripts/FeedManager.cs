using UnityEngine;

public class FeedManager : MonoBehaviour
{
    [Header("Referencias")]
    public PetManager petManager;
    public GameObject prefabComida;
    public RectTransform puntoDeAparicion;
    public RectTransform contenedor;

    [Header("Audio")]
    public AudioClip sonidoComer;

    /// <summary>Llamado por el botón 1 (alimentar).</summary>
    public void SpawnFood()
    {
        if (petManager == null) { Debug.LogWarning("FeedManager: falta asignar 'petManager'."); return; }
        if (prefabComida == null) { Debug.LogWarning("FeedManager: falta asignar 'prefabComida'."); return; }
        if (puntoDeAparicion == null) { Debug.LogWarning("FeedManager: falta asignar 'puntoDeAparicion'."); return; }

        Transform padre = contenedor != null ? contenedor : puntoDeAparicion.parent;
        GameObject comida = Instantiate(prefabComida, padre);

        RectTransform rect = comida.transform as RectTransform;
        if (rect != null) rect.position = puntoDeAparicion.position;

        if (comida.GetComponent<DraggableUI>() == null)
            comida.AddComponent<DraggableUI>();

        FoodItem item = comida.GetComponent<FoodItem>();
        if (item == null) item = comida.AddComponent<FoodItem>();

        item.Configurar(petManager, petManager.PetRect, sonidoComer);
    }
}
