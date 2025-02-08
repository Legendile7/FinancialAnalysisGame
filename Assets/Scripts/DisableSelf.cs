using UnityEngine;

public class DisableSelf : MonoBehaviour
{
    public GameObject objectToDisable;
    public void DisableMe()
    {
        objectToDisable.SetActive(false);
    }
}
