using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class FadeController : MonoBehaviour {

    public Image fadePanel;
    public float fadeTime = 0.1f;

    private void Start()
    {
        FadeOut();
    }

    public void FadeIn(TweenCallback tweenCallback = null)
    {
        fadePanel.DOFade(1, fadeTime).OnComplete(tweenCallback);
    }

    public void FadeOut(TweenCallback tweenCallback = null)
    {
        fadePanel.DOFade(0, fadeTime).OnComplete(tweenCallback);
    }
}
