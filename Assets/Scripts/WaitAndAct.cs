using UnityEngine;
using UnityEngine.Events;

public class WaitAndAct : MonoBehaviour
{
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private bool startOnStart = true;
    [SerializeField] private UnityEvent onWaitComplete;

    private Coroutine runningCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (startOnStart)
        {
            StartTimer();
        }
    }

    public void StartTimer()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
        }

        runningCoroutine = StartCoroutine(WaitAndInvoke());
    }

    public void CancelTimer()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
    }

    private System.Collections.IEnumerator WaitAndInvoke()
    {
        yield return new WaitForSeconds(waitTime);
        onWaitComplete?.Invoke();
        runningCoroutine = null;
    }
}
