using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WholeMarkAnimationScript : MonoBehaviour
{
    [SerializeField] private Image MarkImage;
    [SerializeField] private Animation MarkAnimation;
    [SerializeField] private AnimationClip WholeMarkAppearAnimation;
    [SerializeField] private AnimationClip WholeMarkDisappearAnimation;

    private readonly int AnimationIndexAppear = 0;
    private readonly int AnimationIndexDisappear = 1;

    public void AppearWholeMarkAnime()
    {
        WholeMarkAppearAnimation.legacy = true;
        MarkImage.enabled = true;
        MarkAnimation.clip = WholeMarkAppearAnimation;
        MarkAnimation.Play();
    }

    public void DisappearWholeMarkAnime()
    {
        WholeMarkDisappearAnimation.legacy = true;
        MarkAnimation.clip = WholeMarkDisappearAnimation;
        MarkAnimation.Play();
    }

    public void HideWholeMark()
    {
        MarkImage.enabled = false;
    }

    public bool ReturnIsMarkAppeared()
    {
        return (MarkImage.enabled);
    }
}
