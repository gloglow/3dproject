using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public enum Type { Phy, Mag };
    public Type type;
    public enum Weapon { Melee, Bullet };
    public Weapon weapon;

    public int damage;
}
