using System.Collections;
using System.Collections.Generic;



public class PartyMemberData
{
    public string Name;     //�̸�
    public int HP;          //ü��
    public int MP;          //����
    public int St;          //��-Į �⺻ ���� ���� ���, ���� ��ų ���� ���� ���
    public int Dx;          //��-�� �⺻ ���� ���� ���, ���� ��ų ���� ���� ���
    public int Ma;          //��-���� ��ų ���� ���
    public int Ag;          //��-������ Ȯ��, �ൿ����, ���߷�, ȸ���� ���
    public int Lu;          //��-ũ��Ƽ��, ȸ����, �����̻� ������, �����̻� ȸ���ӵ�, ������ ����� ���
    public AilmentType NowAilment;  //���� �ɷ��ִ� �����̻�-���� ��� null, �����̻��� ��ø���� �ʴ´�: ������ �ɷ����� ��� �� �����̻��� �����

}

public class PartyDemonData : PartyMemberData
{
    public int ID;          //�Ǹ� �ĺ� ��ȣ

}
