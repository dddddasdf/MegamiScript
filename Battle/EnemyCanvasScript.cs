using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCanvasScript : MonoBehaviour
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

    private void Awake()
    {
        
    }

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


    public void HideAffinityMarkAll()
    {
        for (int i = 0; i < EnemyCellArray.Length; i++)
        {
            EnemyCellArray[i].TurnOffAffinityMark();
        }
    }
}
