using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HalfMarkAnimationScript : MonoBehaviour
{
    [SerializeField] private Animator HalfMarkAnimator;
    [SerializeField] private Image HalfMarkImage;
    [SerializeField] private Animation HalfMarkAnimation;
    [SerializeField] private AnimationClip HalfMarkAppearAnimation;
    [SerializeField] private AnimationClip HalfMarkDisappearAnimation;

    private readonly int AnimationIndexAppear = 0;
    private readonly int AnimationIndexDisappear = 1;
    
    public void AppearHalfMarkAnime()
    {
        HalfMarkAppearAnimation.legacy = true;
        HalfMarkImage.enabled = true;
        HalfMarkAnimation.clip = HalfMarkAppearAnimation;
        HalfMarkAnimation.Play();
    }

    public void DisappearHalfMarkAnime()
    {
        HalfMarkDisappearAnimation.legacy = true;
        HalfMarkAnimation.clip = HalfMarkDisappearAnimation;
        HalfMarkAnimation.Play();
    }

    public void HideHalfMark()
    {
        HalfMarkImage.enabled = false;
    }

    public bool ReturnIsMarkAppeared()
    {
        return (HalfMarkImage.enabled);
    }
}

