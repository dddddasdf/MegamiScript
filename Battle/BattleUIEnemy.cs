using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleUIManager : MonoBehaviour
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

    private EnemyCell TargetedEnemy;        //타게팅된 적 저장용

    public static EnemyCell ClickedEnemyCell = new();      //클릭시 정보 받아오기용

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

        TargetedEnemy = EnemyCellArray[0];      //맨 처음 타게팅 되는 적은 0번째 인덱스의 적
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

    /// <summary>
    /// 현재 적 전체에게 상성 표시 마크 감추기
    /// </summary>
    public void HideAffinityMarkAll()
    {
        for (int i = 0; i < EnemyCellArray.Length; i++)
        {
            EnemyCellArray[i].TurnOffAffinityMark();
        }
    }

    /// <summary>
    /// 현재 타게팅 된 단일에게만 상성 표시 보여주기
    /// </summary>
    public void ShowAffinityMarkSingle()
    {
        TargetedEnemy.TurnOnAffinityMark(NowSelectedSkillData.ReturnSkillDataRec());
    }

    /// <summary>
    /// 타게팅 변경시 or 공격시 타게팅 된 단일에게 표시된 상성 표시 숨기기
    /// </summary>
    public void HideAffinityMarkSingle()
    {
        TargetedEnemy.TurnOffAffinityMark();
    }


    /// <summary>
    /// 사용할 스킬을 고르고 어떤 적을 타게팅할 건지 고르는 단계에서 타게팅 된 적의 정보 보여주기
    /// </summary>
    public void ShowEnemyInformation()
    {
        TargetedEnemy.ShowInformation();
    }

    /// <summary>
    /// 타게팅 된 적의 정보 감추기
    /// </summary>
    public void HideEnemyInformation()
    {
        TargetedEnemy.HideInformation();
    }

    /// <summary>
    /// 적 게임 오브젝트 클릭
    /// </summary>
    public void ClickEnemyGameobject()
    {
        if (OnMenuStack.Count == 0 || OnMenuStack.Peek() != NowOnMenu.SkillSelected)
            return;     //적 타게팅하는 단계가 아니면 반응하지 않는다

        if (TargetedEnemy == ClickedEnemyCell)
            return;     //타게팅이 변함 없으면 내용을 변경할 필요 없다

        if (NowSelectedSkillData.ReturnSkillDataRec().ReturnNumberOfTarget() == NumberOfTarget.Single)
        {
            //단일 공격 스킬
            TargetedEnemy.HideInformation();        //원래 타게팅 되어있던 적의 정보를 감춘다
            HideAffinityMarkSingle();               //상성 표시 마크를 감춘다
            TargetedEnemy = ClickedEnemyCell;       //타게팅 된 적을 교체한다
            TargetedEnemy.ShowInformation();        //새로 타게팅 된 적의 정보를 띄운다
            ShowAffinityMarkSingle();               //새로 타게팅 된 적에게 상성 표시를 띄운다
        }
        else
        {
            //아닐 경우
            TargetedEnemy.HideInformation();        //원래 타게팅 되어있던 적의 정보를 감춘다
            TargetedEnemy = ClickedEnemyCell;       //타게팅 된 적을 교체한다
            TargetedEnemy.ShowInformation();        //새로 타게팅 된 적의 정보를 띄운다
        }
    }


    /// <summary>
    /// 타게팅된 적이 죽으면 타게팅을 살아있는 적 중 가장 순번이 빠른 녀석으로 옮긴다
    /// </summary>
    public void ResetTarget(List<OnBattleEnemyObject?> NowOnBattleObject)
    {
        for (int i = 0; i < NowOnBattleObject.Count; i++)
        {
            if (NowOnBattleObject[i] != null)
            {
                TargetedEnemy = EnemyCellArray[i];      //null이 아닐 경우 살아있으니 해당 적으로 타게팅 미리 변경시켜두고 종료
                break;
            }
        }
    }

    /// <summary>
    /// 적이 공격으로 죽었을 경우 사라지게 한다
    /// </summary>
    public void DisappearEnemy()
    {

    }
}
