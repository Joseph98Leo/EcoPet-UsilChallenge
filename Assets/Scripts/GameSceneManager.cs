using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Drop this on any GameObject (e.g. a Button) to navigate to a scene with a transition.
/// Configure the target scene, effect, duration and color in the inspector and wire
/// the button's OnClick to Play(). Also exposes a static LoadScene API for code-driven
/// navigation. A persistent TransitionManager is bootstrapped automatically
/// (via RuntimeInitializeOnLoadMethod), so nothing needs to be placed in any scene
/// and Play mode can be entered from any scene directly in the editor.
/// </summary>
public class GameSceneManager : MonoBehaviour
{
    private const string TransitionsResourcesPath = "Transitions/";
    private const int OverlaySortingOrder = 1000;

    [Header("Transición")]
    [Tooltip("Nombre de la escena a cargar")]
    [SerializeField] private string sceneName;
    [SerializeField] private TransitionEffectType effect = TransitionEffectType.WaterWave;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Color color = Color.white;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        EnsureTransitionManager();
    }

    /// <summary>Wire this to a Button's OnClick to navigate using the inspector settings above.</summary>
    public void Play()
    {
        LoadScene(sceneName, effect, duration, color);
    }

    public static void LoadScene(string sceneName)
    {
        LoadScene(sceneName, TransitionEffectType.WaterWave, 1f, Color.white);
    }

    public static void LoadScene(string sceneName, TransitionEffectType effect, float duration, Color color)
    {
        EnsureTransitionManager();

        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.TransitionToScene(sceneName, duration, color, effect);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private static void EnsureTransitionManager()
    {
        if (TransitionManager.Instance != null)
        {
            return;
        }

        GameObject root = new GameObject("SceneTransitionManager");
        Object.DontDestroyOnLoad(root);

        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = OverlaySortingOrder;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0f;

        root.AddComponent<GraphicRaycaster>();

        GameObject imageGO = new GameObject("TransitionImage");
        imageGO.transform.SetParent(root.transform, false);

        Image image = imageGO.AddComponent<Image>();
        RectTransform rect = image.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        TransitionManager transitionManager = root.AddComponent<TransitionManager>();
        transitionManager.Configure(
            image,
            Resources.Load<Material>(TransitionsResourcesPath + "WaterWave"),
            Resources.Load<Material>(TransitionsResourcesPath + "Leaf"),
            Resources.Load<Material>(TransitionsResourcesPath + "Wind"),
            Resources.Load<Material>(TransitionsResourcesPath + "Stripes"));
    }
}
