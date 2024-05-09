using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollbarController : MonoBehaviour
{
    public ScrollRect currentScrollRect;
    public float scrollSpeed = 0.5f;

    public ScrollRect pagesCollectionScrollRect;
    public ScrollRect pageInspectionScrollRect;
    private Coroutine smoothScrollCoroutine;

    void Start()
    {
        currentScrollRect = pagesCollectionScrollRect;
    }
    void Update()
    {
        if (InventoryManager.Instance.pagesCollection.activeSelf)
        {
            currentScrollRect = pagesCollectionScrollRect;
        }
        if (InventoryManager.Instance.pageInspection.activeSelf)
        {
            currentScrollRect = pageInspectionScrollRect;
        }

        // Scroll with up/down arrow keys
        if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && (smoothScrollCoroutine == null))
        {
            smoothScrollCoroutine = StartCoroutine(SmoothScroll(currentScrollRect.verticalNormalizedPosition + scrollSpeed));
        }
        else if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && (smoothScrollCoroutine == null))
        {
            smoothScrollCoroutine = StartCoroutine(SmoothScroll(currentScrollRect.verticalNormalizedPosition - scrollSpeed));
        }
        else if (smoothScrollCoroutine != null)
        {
            StopCoroutine(smoothScrollCoroutine);
            smoothScrollCoroutine = null;
        }
    }

    IEnumerator SmoothScroll(float target)
    {
        float time = 0;
        float start = currentScrollRect.verticalNormalizedPosition;

        // Clamp the target value within the range of 0 to 1
        target = Mathf.Clamp(target, 0f, 1f);

        while (time < 0.01f)
        {
            time += Time.deltaTime / scrollSpeed;
            currentScrollRect.verticalNormalizedPosition = Mathf.Lerp(start, target, time);
            yield return null;
        }

        smoothScrollCoroutine = null;
    }
}