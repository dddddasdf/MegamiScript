using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObjectGroup : MonoBehaviour
{
    [SerializeField] private EnemyCell[] enemyObject;

    private void Awake()
    {
        
    }

    /// <summary>
    /// 적 스프라이트 담당 오브젝트들 활성화
    /// </summary>
    private void ActivateObject()
    {
        for (int i = 0; i < enemyObject.Length; i++)
        {
            //enemyObject[i].SetActive(true);
        }
    }
}
