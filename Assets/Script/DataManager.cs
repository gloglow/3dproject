using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;　//　シングルトーン

    private static string jsonPath = Application.streamingAssetsPath;
    private static string playerDataFileName = Path.Combine(jsonPath, "PlayerData.json");

    public PlayerData playerData;
    public GameObject[] enemyPrefabList;　//　モンスターのprefabリスト

    public static DataManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(DataManager)) as DataManager;

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

    private void Start()
    {
        LoadPlayerData();
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

    public void SavePlayerData()
    {
        string jsonData = JsonConvert.SerializeObject(playerData);
        File.WriteAllText(playerDataFileName, JsonPrettify(jsonData));
    }

    private void LoadPlayerData()
    {
        if (File.Exists(playerDataFileName))
        {
            var dataFile = File.ReadAllText(playerDataFileName);
            playerData = JsonConvert.DeserializeObject<PlayerData>(dataFile);
        }
        else
        {
            playerData = new PlayerData();
            string jsonData = JsonConvert.SerializeObject(playerData);
            File.WriteAllText(playerDataFileName, JsonPrettify(jsonData));
        }
    }

    public StageInfo LoadStageData(int stageNum)
    {
        string fileName = Path.Combine(jsonPath, "Stage" + stageNum.ToString() + ".json");
        if (File.Exists(fileName))
        {
            var dataFile = File.ReadAllText(fileName);
            StageInfo stageInfo = new StageInfo();
            stageInfo = JsonConvert.DeserializeObject<StageInfo>(dataFile);
            return stageInfo;
        }
        else
        {
            Debug.Log("There is no stage data");
            return null;
        }
    }
}
