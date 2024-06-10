using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HpBar : MonoBehaviour
{
    //　体力バーのUI
    private Slider hpSlider;

    private void Awake()
    {
        hpSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }

    public void Initialize(int value)
    {
        hpSlider.maxValue = value;
        hpSlider.value = value;
    }

    public void ChangeValue(int value)
    {
        //　体力バーの値を変化させる
        DOTween.To(() => hpSlider.value, x => hpSlider.value = x, value, 0.1f);
    }
}
