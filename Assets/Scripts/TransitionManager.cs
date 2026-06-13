using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum TransitionEffectType
{
    WaterWave,
    Leaf,
    Wind,
    Stripes
}

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [Header("Referencias")]
    [Tooltip("Image que cubre toda la pantalla, usada para mostrar la transición")]
    [SerializeField] private Image transitionImage;

    [Header("Materiales por efecto")]
    [SerializeField] private Material waterWaveMaterial;
    [SerializeField] private Material leafMaterial;
    [SerializeField] private Material windMaterial;
    [SerializeField] private Material stripesMaterial;

    private static readonly int ProgressID = Shader.PropertyToID("_Progress");
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    private Material runtimeMaterial;
    private bool isTransitioning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (transitionImage != null)
        {
            transitionImage.raycastTarget = false;
            transitionImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Wires up references created at runtime by GameSceneManager when no TransitionManager
    /// has been placed in the scene. Existing inspector-assigned references take priority.
    /// </summary>
    public void Configure(Image image, Material waterWave, Material leaf, Material wind, Material stripes)
    {
        if (transitionImage == null)
        {
            transitionImage = image;
            transitionImage.raycastTarget = false;
            transitionImage.gameObject.SetActive(false);
        }

        if (waterWaveMaterial == null) waterWaveMaterial = waterWave;
        if (leafMaterial == null) leafMaterial = leaf;
        if (windMaterial == null) windMaterial = wind;
        if (stripesMaterial == null) stripesMaterial = stripes;
    }

    public void TransitionToScene(string sceneName, float duration, Color color, TransitionEffectType effect)
    {
        if (isTransitioning)
        {
            return;
        }

        StartCoroutine(DoTransition(sceneName, duration, color, effect));
    }

    private IEnumerator DoTransition(string sceneName, float duration, Color color, TransitionEffectType effect)
    {
        isTransitioning = true;

        Material source = GetMaterialForEffect(effect);
        runtimeMaterial = new Material(source);
        runtimeMaterial.SetColor(ColorID, color);
        runtimeMaterial.SetFloat(ProgressID, 0f);

        transitionImage.material = runtimeMaterial;
        transitionImage.color = Color.white;
        transitionImage.gameObject.SetActive(true);

        float half = duration * 0.5f;

        yield return AnimateProgress(0f, 1f, half);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return AnimateProgress(1f, 0f, half);

        transitionImage.gameObject.SetActive(false);
        Destroy(runtimeMaterial);
        runtimeMaterial = null;
        isTransitioning = false;
    }

    private IEnumerator AnimateProgress(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            runtimeMaterial.SetFloat(ProgressID, to);
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = t * t * (3f - 2f * t);
            runtimeMaterial.SetFloat(ProgressID, Mathf.Lerp(from, to, eased));
            yield return null;
        }

        runtimeMaterial.SetFloat(ProgressID, to);
    }

    private Material GetMaterialForEffect(TransitionEffectType effect)
    {
        switch (effect)
        {
            case TransitionEffectType.WaterWave: return waterWaveMaterial;
            case TransitionEffectType.Leaf: return leafMaterial;
            case TransitionEffectType.Wind: return windMaterial;
            case TransitionEffectType.Stripes: return stripesMaterial;
            default: return waterWaveMaterial;
        }
    }
}
