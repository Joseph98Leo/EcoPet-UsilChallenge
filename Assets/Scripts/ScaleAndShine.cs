using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScaleAndShine : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float scaleTarget = 1.1f;
    [SerializeField] private float duration = 0.5f;

    private Vector3 baseScale;
    private Color baseColor;

    private void Awake()
    {
        if (image == null) image = GetComponent<Image>();
        baseScale = transform.localScale;
        baseColor = image.color;
    }

    private void OnEnable()
    {
        transform.localScale = baseScale;
        image.color = baseColor;

        transform.DOScale(baseScale * scaleTarget, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        image.DOColor(new Color(1f, 1f, 1f, baseColor.a), duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void OnDisable()
    {
        transform.DOKill();
        image.DOKill();
        transform.localScale = baseScale;
        image.color = baseColor;
    }
}
