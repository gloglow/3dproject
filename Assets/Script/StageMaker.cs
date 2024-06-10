using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;
using Ookii.Dialogs;

public class StageEnemy
{
    public int id;
    public int level;
    public Vector2 pos;
    public float yRotation;
}

public class StageInfo
{
    public int stageId;
    public StageEnemy[] stageEnemyList;
}

public class StageMaker : MonoBehaviour
{
    public List<GameObject> enemyPrefabList;　//　モンスターのprefabリスト
    public List<Monster> enemyList;

    private VistaOpenFileDialog openDialog;
    private VistaSaveFileDialog saveDialog;
    private Stream openStream = null;

    [SerializeField] private int stageId;

    private void Start()
    {
        openDialog = new VistaOpenFileDialog();
        saveDialog = new VistaSaveFileDialog();
    }

    public void MakeEnemy(int id)
    {
        Monster enemy;
        enemy = Instantiate(enemyPrefabList[id], transform).GetComponent<Monster>();
        enemyList.Add(enemy);
    }

    public static string JsonPrettify(string json)
    {
        using (var stringReader = new StringReader(json))
        using (var stringWriter = new StringWriter())
        {
            var jsonReader = new JsonTextReader(stringReader);
            var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
            jsonWriter.WriteToken(jsonReader);
            return stringWriter.ToString();
        }
    }

    public void SaveStage()
    {
        List<StageEnemy> enemyInfoList = new List<StageEnemy>();
        foreach(Monster enemy in enemyList)
        {
            StageEnemy enemyInfo = new StageEnemy();
            enemyInfo.id = enemy.id;
            enemyInfo.level = enemy.level;
            enemyInfo.pos = new Vector2(enemy.transform.position.x, enemy.transform.position.z);
            enemyInfo.yRotation = enemy.transform.rotation.y;
            enemyInfoList.Add(enemyInfo);
        }

        StageInfo stageInfo = new StageInfo();
        stageInfo.stageId = stageId;
        stageInfo.stageEnemyList = enemyInfoList.ToArray();

        string jsonData = JsonConvert.SerializeObject(stageInfo);

        saveDialog.Filter = "json files (*.json)|*.json";
        openDialog.FilterIndex = 1;
        openDialog.Title = "json File Dialog";
        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            File.WriteAllText(saveDialog.FileName + ".json", JsonPrettify(jsonData));
        }
    }
}
