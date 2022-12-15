using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattlePartyMember : MonoBehaviour
{
    #region SetVariables

    [SerializeField] private GameObject MemberInfo;  //�� ����� �ƴ� ��� ����ϴ� ���������� ����
    [SerializeField] private Image Frame;  //��Ƽ ������
    [SerializeField] private Slider HPBar;  //ü�� �� 
    [SerializeField] private Slider MPBar;  //���� ��
    [SerializeField] private TextMeshProUGUI NameTMP;
    [SerializeField] private TextMeshProUGUI HPText;
    [SerializeField] private TextMeshProUGUI MPText;
    [SerializeField] private GameObject EmptyText; //�� ����� ��� ������� ǥ��

    #endregion

    /// <summary>
    /// ����ִ� ������� Ȯ��
    /// </summary>
    /// <param name="IsEmpty"> ��������� true</param>
    public void SetEmptyText()
    {
        MemberInfo.SetActive(false);
        EmptyText.SetActive(true);
    }

    /// <summary>
    /// ǥ��� �̸� ���ڿ� �޾ƿ�
    /// </summary>
    /// <param name="Name"></param>
    public void GetName(string Name)
    {
        NameTMP.text = Name;
    }

    /// <summary>
    /// �ش� ��ȣ ����� �ִ� ü�� �� ���� �� ����
    /// </summary>
    /// <param name="thisMaxHP"></param>
    /// <param name="thisMaxMP"></param>
    public void GetMaxHPMP(int MaxHP, int MaxMP)
    {
        HPBar.maxValue = MaxHP;
        MPBar.maxValue = MaxMP;
    }

    /// <summary>
    /// ü�� �� ������ ����
    /// </summary>
    /// <param name="RemainHP"></param>
    public void GetHPValue(int RemainHP)
    {
        HPBar.value = RemainHP;
        HPText.text = RemainHP.ToString();
    }
    
    /// <summary>
    /// ���� �� ������ ����
    /// </summary>
    /// <param name="RemainMP"></param>
    public void GetMPValue(int RemainMP)
    {
        MPBar.value = RemainMP;
        MPText.text = RemainMP.ToString();
    }
    
    /// <summary>
    /// ���� �������� ��� �������� ������� ����
    /// </summary>
    public void NowTurn()
    {
        Frame.color = Color.green;
    }

    /// <summary>
    /// �������� ����Ǹ� �������� ������� ����
    /// </summary>
    public void EndTurn()
    {
        Frame.color = Color.white;
    }
}
