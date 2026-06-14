using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BathManager : MonoBehaviour
{
    [Header("Referencias")]
    public PetManager petManager;
    public RectTransform contenedor;

    [Header("Esponja")]
    public GameObject prefabEsponja;
    public RectTransform puntoAparicionEsponja;

    [Header("Botón Listo")]
    public GameObject prefabBotonListo;
    public RectTransform puntoAparicionBotonListo;

    [Header("Espuma")]
    public GameObject prefabEspuma;
    [Range(1, 30)] public int cantidadEspuma = 10;
    public float radioEspuma = 150f;

    [Header("Manguera")]
    public GameObject prefabManguera;
    public RectTransform puntoAparicionManguera;

    [Header("Configuración")]
    [Range(0f, 1f)] public float porcentajeParaTerminar = 0.9f;

    private GameObject esponjaActual;
    private GameObject botonListoActual;
    private GameObject mangueraActual;
    private readonly List<GameObject> espumaActiva = new List<GameObject>();
    private int totalEspuma;
    private bool banoTerminado;

    /// <summary>Llamado por el botón 2 (bañar): aparece la esponja y el botón "listo".</summary>
    public void IniciarBano()
    {
        if (petManager == null) { Debug.LogWarning("BathManager: falta asignar 'petManager'."); return; }
        if (prefabEsponja == null) { Debug.LogWarning("BathManager: falta asignar 'prefabEsponja'."); return; }
        if (puntoAparicionEsponja == null) { Debug.LogWarning("BathManager: falta asignar 'puntoAparicionEsponja'."); return; }

        banoTerminado = false;
        espumaActiva.Clear();
        totalEspuma = 0;

        Transform padre = contenedor != null ? (Transform)contenedor : puntoAparicionEsponja.parent;

        esponjaActual = Instantiate(prefabEsponja, padre);
        PosicionarEn(esponjaActual, puntoAparicionEsponja);

        if (esponjaActual.GetComponent<DraggableUI>() == null)
            esponjaActual.AddComponent<DraggableUI>();

        SpongeItem spongeItem = esponjaActual.GetComponent<SpongeItem>();
        if (spongeItem == null) spongeItem = esponjaActual.AddComponent<SpongeItem>();
        spongeItem.Configurar(this, petManager.PetRect);

        if (prefabBotonListo != null && puntoAparicionBotonListo != null)
        {
            botonListoActual = Instantiate(prefabBotonListo, padre);
            PosicionarEn(botonListoActual, puntoAparicionBotonListo);

            Button boton = botonListoActual.GetComponent<Button>();
            if (boton != null) boton.onClick.AddListener(TerminarEspuma);
        }
    }

    /// <summary>Llamado por la esponja al tocar a la mascota: aparece la espuma.</summary>
    public void OnSpongeTouchedPet()
    {
        if (prefabEspuma == null) { Debug.LogWarning("BathManager: falta asignar 'prefabEspuma'."); return; }
        if (petManager.PetRect == null) { Debug.LogWarning("BathManager: 'petManager.imagenMascota' no está asignado."); return; }

        Transform padre = contenedor != null ? (Transform)contenedor : petManager.PetRect.parent;
        Vector2 centro = petManager.PetRect.anchoredPosition;

        for (int i = 0; i < cantidadEspuma; i++)
        {
            GameObject pieza = Instantiate(prefabEspuma, padre);
            RectTransform rect = pieza.transform as RectTransform;
            if (rect != null)
            {
                Vector2 offset = Random.insideUnitCircle * radioEspuma;
                rect.anchoredPosition = centro + offset;
            }

            espumaActiva.Add(pieza);
        }

        totalEspuma = espumaActiva.Count;
    }

    /// <summary>Llamado por el botón "listo": quita la esponja y trae la manguera.</summary>
    public void TerminarEspuma()
    {
        if (esponjaActual != null) Destroy(esponjaActual);
        if (botonListoActual != null) Destroy(botonListoActual);

        if (prefabManguera == null) { Debug.LogWarning("BathManager: falta asignar 'prefabManguera'."); return; }
        if (puntoAparicionManguera == null) { Debug.LogWarning("BathManager: falta asignar 'puntoAparicionManguera'."); return; }

        Transform padre = contenedor != null ? (Transform)contenedor : puntoAparicionManguera.parent;

        mangueraActual = Instantiate(prefabManguera, padre);
        PosicionarEn(mangueraActual, puntoAparicionManguera);

        if (mangueraActual.GetComponent<DraggableUI>() == null)
            mangueraActual.AddComponent<DraggableUI>();

        HoseItem hoseItem = mangueraActual.GetComponent<HoseItem>();
        if (hoseItem == null) hoseItem = mangueraActual.AddComponent<HoseItem>();
        hoseItem.Configurar(this, espumaActiva);
    }

    /// <summary>Llamado por la manguera al tocar una pieza de espuma.</summary>
    public void RemoverEspuma(GameObject pieza)
    {
        if (pieza == null || banoTerminado) return;
        if (!espumaActiva.Remove(pieza)) return;

        Destroy(pieza);

        if (totalEspuma <= 0) return;

        float eliminado = (float)(totalEspuma - espumaActiva.Count) / totalEspuma;
        if (eliminado >= porcentajeParaTerminar)
        {
            banoTerminado = true;
            TerminarBano();
        }
    }

    private void TerminarBano()
    {
        if (mangueraActual != null) Destroy(mangueraActual);
        petManager.Limpiar();
    }

    private static void PosicionarEn(GameObject objeto, RectTransform punto)
    {
        RectTransform rect = objeto.transform as RectTransform;
        if (rect != null) rect.position = punto.position;
    }
}
