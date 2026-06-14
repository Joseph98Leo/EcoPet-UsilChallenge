using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [SerializeField] private float saludOtorgada = 20f;

    private PetManager petManager;
    private RectTransform petRect;
    private RectTransform rectTransform;
    private AudioClip sonidoComer;

    public void Configurar(PetManager manager, RectTransform pet, AudioClip sonido = null)
    {
        petManager = manager;
        petRect = pet;
        sonidoComer = sonido;
        rectTransform = transform as RectTransform;
    }

    private void Update()
    {
        if (petManager == null || petRect == null || rectTransform == null) return;

        if (RectOverlapUtil.Overlaps(rectTransform, petRect))
        {
            petManager.Alimentar(saludOtorgada);
            petManager.ReproducirSonido(sonidoComer);
            Destroy(gameObject);
        }
    }
}
