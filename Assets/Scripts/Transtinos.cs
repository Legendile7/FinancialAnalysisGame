using TransitionsPlus;
using UnityEngine;

public class Transtinos : MonoBehaviour
{
    public float timer = 0;
    public TransitionAnimator animator;
    public TransitionAnimator normal;
    public GameObject gameobject;

    public void Delay()
    {
        CancelInvoke(nameof(doShit));
        Invoke(nameof(doShit), timer);

    }

    private void doShit()
    {
        animator.gameObject.SetActive(true);
        normal.gameObject.SetActive(false);
        gameobject.SetActive(true);
        animator.Play();
    }
}
