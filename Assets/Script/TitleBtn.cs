using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using System.Threading;
using UnityEngine.Events;

public class TitleBtn : MonoBehaviour
{
    [SerializeField] private string whatFunc;
    [SerializeField] private int HP;
    [SerializeField] private Vector3 initialPos;

    private Material material;
    public UnityEvent boxDestroyed;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    protected void OnTriggerEnter(Collider other)
    {
        //　プレイヤーに攻撃されたら
        if (other.tag == "PlayerAtk")
        {
            HP--;
            StartCoroutine(OnDamage());
        }
    }

    IEnumerator OnDamage()
    {
        //　まだ生きている
        if (HP > 0)
        {
            StartCoroutine(DamagedEffect());
        }
        //　死んだ
        else
        {
            BrokenEffect();
            boxDestroyed?.Invoke();
            Invoke("Respawn", 3f);
        }

        yield return null;
    }

    IEnumerator DamagedEffect()
    {
        //　体を一瞬赤くする
        material.color = new Color(1f, 0f, 0f);
        transform.DOShakePosition(0.1f);

        yield return new WaitForSeconds(0.1f);

        material.color = new Color(1f, 1f, 1f);
    }

    private void BrokenEffect()
    {
        transform.DOPause();
        gameObject.SetActive(false);
    }

    private void Respawn()
    {
        HP = 10;
        transform.position = initialPos;
        material.color = new Color(1f, 1f, 1f);
        gameObject.SetActive(true);
    }
}
