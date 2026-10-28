using UnityEngine;

public class ForbiddenMarker : MonoBehaviour
{
    public void Show(float radius)
    {
        transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}