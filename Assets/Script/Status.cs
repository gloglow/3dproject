using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Status
{
    public int HP;　//　最大hp
    public int atkPhy;　//　物理攻撃力
    public int atkMag;　//　魔法攻撃力
    public int defPhy;　//　物理防衛力
    public int defMag;　//　魔法防衛力
    public float criRate;　//　クリティカル率
    public float criPer;　//　クリティカル倍率
    public float spdMov;　//　移動速度
    public float spdAtk;　//　攻撃速度
}
