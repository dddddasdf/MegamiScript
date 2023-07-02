using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleUIManager : MonoBehaviour
{
    [SerializeField] private GameObject EnemySingle;    //���� ����
    [SerializeField] private GameObject EnemyDuo;       //2ü ����
    [SerializeField] private GameObject EnemyTrio;      //3ü ����
    [SerializeField] private GameObject EnemyQuartet;   //4ü ����

    [SerializeField] private EnemyCell[] SingleCellArray;
    [SerializeField] private EnemyCell[] DuoCellArray;
    [SerializeField] private EnemyCell[] TrioCellArray;
    [SerializeField] private EnemyCell[] QuartetCellArray;
    /// <summary>
    /// �����ϴ� ���� ���ڿ� ���� ���������� Ȱ���� ���ʹ̼��� �޴� �迭
    /// </summary>
    private EnemyCell[] EnemyCellArray;

    private EnemyCell TargetedEnemy;        //Ÿ���õ� �� �����

    public static EnemyCell ClickedEnemyCell = new();      //Ŭ���� ���� �޾ƿ����

#nullable enable
    /// <summary>
    /// ���� ���� �� �� ��Ƽ ����Ʈ �Ϸ� �Ǹ� ������ �����ͼ� UI�� Ȱ��ȭ�Ѵ�
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

        TargetedEnemy = EnemyCellArray[0];      //�� ó�� Ÿ���� �Ǵ� ���� 0��° �ε����� ��
    }
#nullable disable

    /// <summary>
    /// ������ �����ϴ� ���� ���� �޾ƿͼ� ����� �迭 Ȯ��
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
                //����
                break;
        }
    }

    /// <summary>
    /// ���� �� ��ü���� �� ǥ�� ��ũ ���
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
    /// ���� �� ��ü���� �� ǥ�� ��ũ ���߱�
    /// </summary>
    public void HideAffinityMarkAll()
    {
        for (int i = 0; i < EnemyCellArray.Length; i++)
        {
            EnemyCellArray[i].TurnOffAffinityMark();
        }
    }

    /// <summary>
    /// ���� Ÿ���� �� ���Ͽ��Ը� �� ǥ�� �����ֱ�
    /// </summary>
    public void ShowAffinityMarkSingle()
    {
        TargetedEnemy.TurnOnAffinityMark(NowSelectedSkillData.ReturnSkillDataRec());
    }

    /// <summary>
    /// Ÿ���� ����� or ���ݽ� Ÿ���� �� ���Ͽ��� ǥ�õ� �� ǥ�� �����
    /// </summary>
    public void HideAffinityMarkSingle()
    {
        TargetedEnemy.TurnOffAffinityMark();
    }


    /// <summary>
    /// ����� ��ų�� ���� � ���� Ÿ������ ���� ���� �ܰ迡�� Ÿ���� �� ���� ���� �����ֱ�
    /// </summary>
    public void ShowEnemyInformation()
    {
        TargetedEnemy.ShowInformation();
    }

    /// <summary>
    /// Ÿ���� �� ���� ���� ���߱�
    /// </summary>
    public void HideEnemyInformation()
    {
        TargetedEnemy.HideInformation();
    }

    /// <summary>
    /// �� ���� ������Ʈ Ŭ��
    /// </summary>
    public void ClickEnemyGameobject()
    {
        if (OnMenuStack.Count == 0 || OnMenuStack.Peek() != NowOnMenu.SkillSelected)
            return;     //�� Ÿ�����ϴ� �ܰ谡 �ƴϸ� �������� �ʴ´�

        if (TargetedEnemy == ClickedEnemyCell)
            return;     //Ÿ������ ���� ������ ������ ������ �ʿ� ����

        if (NowSelectedSkillData.ReturnSkillDataRec().ReturnNumberOfTarget() == NumberOfTarget.Single)
        {
            //���� ���� ��ų
            TargetedEnemy.HideInformation();        //���� Ÿ���� �Ǿ��ִ� ���� ������ �����
            HideAffinityMarkSingle();               //�� ǥ�� ��ũ�� �����
            TargetedEnemy = ClickedEnemyCell;       //Ÿ���� �� ���� ��ü�Ѵ�
            TargetedEnemy.ShowInformation();        //���� Ÿ���� �� ���� ������ ����
            ShowAffinityMarkSingle();               //���� Ÿ���� �� ������ �� ǥ�ø� ����
        }
        else
        {
            //�ƴ� ���
            TargetedEnemy.HideInformation();        //���� Ÿ���� �Ǿ��ִ� ���� ������ �����
            TargetedEnemy = ClickedEnemyCell;       //Ÿ���� �� ���� ��ü�Ѵ�
            TargetedEnemy.ShowInformation();        //���� Ÿ���� �� ���� ������ ����
        }
    }


    /// <summary>
    /// Ÿ���õ� ���� ������ Ÿ������ ����ִ� �� �� ���� ������ ���� �༮���� �ű��
    /// </summary>
    public void ResetTarget(List<OnBattleEnemyObject?> NowOnBattleObject)
    {
        for (int i = 0; i < NowOnBattleObject.Count; i++)
        {
            if (NowOnBattleObject[i] != null)
            {
                TargetedEnemy = EnemyCellArray[i];      //null�� �ƴ� ��� ��������� �ش� ������ Ÿ���� �̸� ������ѵΰ� ����
                break;
            }
        }
    }

    /// <summary>
    /// ���� �������� �׾��� ��� ������� �Ѵ�
    /// </summary>
    public void DisappearEnemy()
    {

    }
}
