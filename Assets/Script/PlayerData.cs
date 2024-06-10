using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : Status
{
    public string name;　//　プレイヤーの名前
    public int level;　//　レベル
    public int exp;　//　経験値
    public int crtStage;　//　クリアしたステージ
    public Vector2[] skillList; 　//　スキル育成状態（スキルid、スキルレベル）
    public Vector3[] monsterList;　//　保有モンスターリスト（モンスターid、モンスターレベル、モンスター経験値）

    public PlayerData()
    {
        level = 1;
        crtStage = 0;
        HP = 50;
        atkPhy = 1;
        atkMag = 1;
        defPhy = 0;
        defMag = 0;
        criRate = 10;
        criPer = 120;
        spdMov = 6;
        spdAtk = 1;
    }
}
