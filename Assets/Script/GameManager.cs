using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;　//　シングルトーン

    public int crtStage;

    public static GameManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (instance == null)
                {
                    Debug.Log("no singleton obj");
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeStageNum(int stageNum)
    {
        var existStage = DataManager.Instance.LoadStageData(stageNum);
        if (existStage != null)
            crtStage = stageNum;
    }

    public void MoveScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }

    public void GameQuit()
    {
        Application.Quit();
    }
}
