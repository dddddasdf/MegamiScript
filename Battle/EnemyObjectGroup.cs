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
    /// �� ��������Ʈ ��� ������Ʈ�� Ȱ��ȭ
    /// </summary>
    private void ActivateObject()
    {
        for (int i = 0; i < enemyObject.Length; i++)
        {
            //enemyObject[i].SetActive(true);
        }
    }
}
