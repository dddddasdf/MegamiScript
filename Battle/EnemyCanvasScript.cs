using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCanvasScript : MonoBehaviour
{
    [SerializeField] private GameObject EnemySingle;    //단일 등장
    [SerializeField] private GameObject EnemyDuo;       //2체 등장
    [SerializeField] private GameObject EnemyTrio;      //3체 등장
    [SerializeField] private GameObject EnemyQuartet;   //4체 등장

    private void Awake()
    {
        GetEnemyNumber();
    }

    /// <summary>
    /// 전투에 등장하는 적의 수를 받아와서 알맞은 게임 오브젝트 활성화
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
                //에러
                break;
        }
    }
}
