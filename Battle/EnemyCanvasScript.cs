using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCanvasScript : MonoBehaviour
{
    [SerializeField] private GameObject EnemySingle;    //���� ����
    [SerializeField] private GameObject EnemyDuo;       //2ü ����
    [SerializeField] private GameObject EnemyTrio;      //3ü ����
    [SerializeField] private GameObject EnemyQuartet;   //4ü ����

    private void Awake()
    {
        GetEnemyNumber();
    }

    /// <summary>
    /// ������ �����ϴ� ���� ���� �޾ƿͼ� �˸��� ���� ������Ʈ Ȱ��ȭ
    /// </summary>
    private void GetEnemyNumber()
    {
        int Number = 1;


        switch (Number)
        {
            case 1:
                EnemySingle.SetActive(true);
                break;
            case 2:
                EnemyDuo.SetActive(true);
                break;
            case 3:
                EnemyTrio.SetActive(true);
                break;
            case 4:
                EnemyQuartet.SetActive(true);
                break;
            default:
                //����
                break;
        }
    }
}
