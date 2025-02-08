using UnityEngine;

public class DisableSelf : MonoBehaviour
{
    public void DisableMe()
    {
        gameObject.SetActive(false);
    }
}
