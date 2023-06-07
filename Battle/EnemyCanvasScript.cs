using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCanvasScript : MonoBehaviour
{
    [SerializeField] private GameObject EnemySingle;    //단일 등장
    [SerializeField] private GameObject EnemyDuo;       //2체 등장
    [SerializeField] private GameObject EnemyTrio;      //3체 등장
    [SerializeField] private GameObject EnemyQuartet;   //4체 등장

    [SerializeField] private EnemyCell[] SingleCellArray;
    [SerializeField] private EnemyCell[] DuoCellArray;
    [SerializeField] private EnemyCell[] TrioCellArray;
    [SerializeField] private EnemyCell[] QuartetCellArray;
    /// <summary>
    /// 전투하는 적의 숫자에 따라 최종적으로 활용할 에너미셀을 받는 배열
    /// </summary>
    private EnemyCell[] EnemyCellArray;

    private void Awake()
    {
        
    }

#nullable enable
    /// <summary>
    /// 전투 돌입 후 적 파티 리스트 완료 되면 정보를 가져와서 UI를 활성화한다
    /// </summary>
    /// <param name="NowOnBattleObject"></param>
    public void InitEnemyisplay(List<OnBattleEnemyObject?> NowOnBattleObject)
    {
        int EnemyCount = NowOnBattleObject.Count;

        SelectEnemyCellArray(EnemyCount);
        
        for (int i = 0; i < EnemyCount; i++)
        {
            EnemyCellArray[i].SetEnemyCell(NowOnBattleObject[i]);
        }
    }
#nullable disable

    /// <summary>
    /// 전투에 등장하는 적의 수를 받아와서 사용할 배열 확정
    /// </summary>
    private void SelectEnemyCellArray(int EnemyCount)
    {
        switch (EnemyCount)
        {
            case 1:
                EnemyCellArray = SingleCellArray;
                EnemySingle.SetActive(true);
                break;
            case 2:
                EnemyCellArray = DuoCellArray;
                EnemyDuo.SetActive(true);
                break;
            case 3:
                EnemyCellArray = TrioCellArray;
                EnemyTrio.SetActive(true);
                break;
            case 4:
                EnemyCellArray = QuartetCellArray;
                EnemyQuartet.SetActive(true);
                break;
            default:
                //에러
                break;
        }
    }

    /// <summary>
    /// 현재 적 전체에게 상성 표시 마크 출력
    /// </summary>
    /// <param name="TargetSkill"></param>
    public void ShowAffinityMarkAll(SkillDataRec TargetSkill)
    {
        for (int i = 0; i < EnemyCellArray.Length; i++)
        {
            EnemyCellArray[i].TurnOnAffinityMark(TargetSkill);
        }
    }


    public void HideAffinityMarkAll()
    {
        for (int i = 0; i < EnemyCellArray.Length; i++)
        {
            EnemyCellArray[i].TurnOffAffinityMark();
        }
    }
}
