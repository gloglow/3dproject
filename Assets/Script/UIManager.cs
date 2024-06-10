using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpTxt;
    [SerializeField] private GameObject[] skillUsedImages;

    public void Initialize()
    {
        hpSlider.maxValue = DataManager.Instance.playerData.HP;
        HpUpdate((int)hpSlider.maxValue);

        for(int i = 0; i< skillUsedImages.Length; i++)
        {
            SkillAble(i);
        }
    }

    public void HpUpdate(int value)
    {
        hpSlider.value = value <= 0 ? 0 : value;
        hpTxt.text = value.ToString() + " / " + ((int)hpSlider.maxValue).ToString();
    }

    public void StageChange(int value)
    {
        GameManager.Instance.crtStage = value;
    }

    public void SkillUnable(int index)
    {
        skillUsedImages[index].SetActive(true);
    }

    public void SkillAble(int index)
    {
        skillUsedImages[index].SetActive(false);
    }
}
