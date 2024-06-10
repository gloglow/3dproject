using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StageManager : MonoBehaviour
{
    [SerializeField] private Transform enemyParent;

    private List<Monster> enemyList;

    public UnityEvent initializeStage;
    public UnityEvent stageClear;

    private void Start()
    {
        initializeStage?.Invoke();
        PrepareStage(DataManager.Instance.playerData.crtStage);
    }

    public void InitializeStage(int stageNum)
    {

        PrepareStage(stageNum);
    }

    private void PrepareStage(int stageNum)
    {
        StageInfo stageInfo = DataManager.Instance.LoadStageData(stageNum);
        enemyList = new List<Monster>();

        for(int i=0; i<stageInfo.stageEnemyList.Length; i++)
        {
            StageEnemy stageEnemy = stageInfo.stageEnemyList[i];
            Monster enemy = Instantiate(DataManager.Instance.enemyPrefabList[stageEnemy.id], enemyParent).GetComponent<Monster>();
            enemyList.Add(enemy);
            enemy.monsterDead.AddListener(RemoveMonster);

            enemy.level = stageEnemy.level;
            enemy.transform.position = new Vector3(stageEnemy.pos.x, 0, stageEnemy.pos.y);
            enemy.transform.localEulerAngles = new Vector3(0, stageEnemy.yRotation, 0);
            enemy.gameObject.SetActive(true);
        }
    }

    private void RemoveMonster(Monster monster)
    {
        enemyList.Remove(monster);
        if (enemyList.Count == 0)
            StageClear();
    }

    private void StageClear()
    {
        stageClear?.Invoke();
        if(DataManager.Instance.playerData.crtStage == GameManager.Instance.crtStage)
            DataManager.Instance.playerData.crtStage++;
        DataManager.Instance.SavePlayerData();
    }

    public void NextStage()
    {
        StageMove(DataManager.Instance.playerData.crtStage);
    }

    public void StageMove()
    {
        GameManager.Instance.MoveScene(1);
    }

    public void StageMove(int stageNum)
    {
        GameManager.Instance.crtStage = stageNum;
        GameManager.Instance.MoveScene(1);
    }
}
