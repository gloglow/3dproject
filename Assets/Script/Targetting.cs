using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetting : MonoBehaviour
{
    [SerializeField] private CrossHair crossHair;

    //　ターゲット感知範囲内の敵
    [SerializeField] private List<Monster> enemyList;

    //　現在のターゲット
    public Monster crtTarget;

    public bool onAutoTarget = true;
    private int nextTargetIndex = 1;

    private void Awake()
    {
        enemyList = new List<Monster>();
    }

    private void Update()
    {
        if(!onAutoTarget && crtTarget == null)
        {
            onAutoTarget = true;
            nextTargetIndex = 0;
        }

        if (onAutoTarget)
        {
            //　ターゲットを決定
            DecideTarget();
        }
    }

    public void AddEnemy(Monster enemy)
    {
        if (enemyList.Contains(enemy))
            return;

        //　敵をターゲットリストに追加
        enemyList.Add(enemy);

        //　リストの唯一な敵をターゲットにする
        if(enemyList.Count == 1)
        {
            crtTarget = enemyList[0];
        }
    }

    public void RemoveEnemy(Monster enemy)
    {
        if (!enemyList.Contains(enemy))
            return;
        //　敵をターゲットリストから除去
        enemyList.Remove(enemy);
        //　リストが空いたら、ターゲットはなし
        if (enemyList.Count == 0)
        {
            crossHair.OffTarget();
            crtTarget = null;
        }
    }

    private void DecideTarget()
    {
        if(enemyList.Count == 0)
        {
            crtTarget = null;
            return;
        }
        if(enemyList.Count > 1)
        {
            enemyList.Sort(compareTargetDist);
        }
        
        crtTarget = enemyList[0];
        if (crtTarget == null)
        {
            enemyList.Clear();
            return;
        }
        
        crossHair.gameObject.SetActive(true);
        crossHair.OnTarget(crtTarget.transform);
    }

    private int compareTargetDist(Monster enemy1, Monster enemy2)
    {
        float dist1 = Vector3.Distance(transform.position, enemy1.transform.position);
        float dist2 = Vector3.Distance(transform.position, enemy2.transform.position);
        return dist1 < dist2 ? -1 : 1;
    }

    public Transform GetTarget()
    {
        //　リストに敵がなければターゲットはなし
        if(crtTarget == null)
        {
            return null;
        }

        //　現在のターゲットをリターン
        return crtTarget.transform;
    }

    public Transform ChangeTarget()
    {
        if(nextTargetIndex >= enemyList.Count)
            nextTargetIndex = 0;
        onAutoTarget = false;
        
        crtTarget = enemyList[nextTargetIndex];
        crossHair.OnTarget(crtTarget.transform);
        nextTargetIndex++;
        return crtTarget.transform;
    }
}
