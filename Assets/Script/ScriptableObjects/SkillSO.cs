using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "skillData", menuName = "Scriptable Object/Skill Data")]
public class SkillSO : ScriptableObject
{
    public int id;
    public string name;
    public string description;
    public float coolTime;
    public UnityEvent skillEvent;
}
