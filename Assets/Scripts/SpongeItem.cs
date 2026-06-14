using UnityEngine;

public class SpongeItem : MonoBehaviour
{
    private BathManager bathManager;
    private RectTransform petRect;
    private RectTransform rectTransform;
    private bool tocoMascota;

    public void Configurar(BathManager manager, RectTransform pet)
    {
        bathManager = manager;
        petRect = pet;
        rectTransform = transform as RectTransform;
    }

    private void Update()
    {
        if (tocoMascota || bathManager == null || petRect == null || rectTransform == null) return;

        if (RectOverlapUtil.Overlaps(rectTransform, petRect))
        {
            tocoMascota = true;
            bathManager.OnSpongeTouchedPet();
        }
    }
}
