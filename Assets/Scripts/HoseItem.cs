using System.Collections.Generic;
using UnityEngine;

public class HoseItem : MonoBehaviour
{
    private BathManager bathManager;
    private List<GameObject> espuma;
    private RectTransform rectTransform;

    public void Configurar(BathManager manager, List<GameObject> espumaActiva)
    {
        bathManager = manager;
        espuma = espumaActiva;
        rectTransform = transform as RectTransform;
    }

    private void Update()
    {
        if (bathManager == null || espuma == null || rectTransform == null) return;

        for (int i = espuma.Count - 1; i >= 0; i--)
        {
            GameObject pieza = espuma[i];
            if (pieza == null) continue;

            RectTransform piezaRect = pieza.transform as RectTransform;
            if (RectOverlapUtil.Overlaps(rectTransform, piezaRect))
                bathManager.RemoverEspuma(pieza);
        }
    }
}
