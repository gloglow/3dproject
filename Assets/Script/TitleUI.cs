using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleUI : MonoBehaviour
{
    [SerializeField] Image titlePanel;
    [SerializeField] TextMeshProUGUI titleTxt;
    [SerializeField] GameObject[] floorBtns;
    [SerializeField] TextMeshProUGUI floorInfoPanelTxt;

    private bool onFadeOut;
    private float timer = 0;

    private void Update()
    {
        //　フェードアウト
        if (Input.anyKeyDown && !onFadeOut)
        {
            timer = 0;
            StartCoroutine(FadeOut());
        }
        //　フェードイン
        else if(!Input.anyKey && !onFadeOut)
        {
            timer += Time.deltaTime;
            if (timer >= 5f)
            {
                StartCoroutine(FadeIn());
            }
        }
    }

    private IEnumerator FadeOut()
    {
        onFadeOut = true;
        for(float f = titlePanel.color.a; f >= 0; f -= 0.02f)
        {
            Color color = titlePanel.color;
            color.a = f;
            titlePanel.color = color;
            titleTxt.color = color;
            yield return null;
        }
        titlePanel.gameObject.SetActive(false);
        onFadeOut = false;
    }

    private IEnumerator FadeIn()
    {
        titlePanel.gameObject.SetActive(true);
        for (float f = titlePanel.color.a; f <= 1; f += 0.005f)
        {
            Color color = titlePanel.color;
            color.a = f;
            titlePanel.color = color;
            titleTxt.color = color;
            yield return null;
        }
    }

    public void StageSelecting(int stageNum)
    {
        floorInfoPanelTxt.text = stageNum.ToString() + "F";
        GameManager.Instance.ChangeStageNum(stageNum);
    }
}
