using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Text;
using System.IO;

#region Observer

/// <summary>
/// ��Ƽ �Ŵ����� ������ ����-������ �������̽�
/// </summary>
public interface IPartySubject
{
    void AddObserver(IPartyObserver NewObserver);
    void RemoveObserver(IPartyObserver Observer);
    void NotifyObserver();
}


/// <summary>
/// ��Ƽ �Ŵ����� ������ ����-������ �������̽�
/// </summary>
public interface IPartyObserver
{
    void UpdateEntry();
}

#endregion

/// <summary>
/// �Ӽ��� ���� ����~����
/// </summary>
public enum SkillAffinities
{
    Weak,   //���� �
    Resist, //���� ұ
    Void,   //��ȿ ��
    Reflect,    //�ݻ� ��
    Drain   //��� ��
}

[Flags]
public enum AilmentType
{
    None = 0   //�����̻� ����

    , Poison = 1 << 0    //�� - �������� �ൿ�� ü�� ����, �ʵ忡�� �̵��� ü�� ����/�ڵ� ȸ��X
    , Sick = 1 << 1      //���� - �������� ���ݷ� 25% �϶�, ȸ���� 0%�� �϶�, �� ���Ḷ�� 5%�� Ȯ���� Ÿ �Ʊ� ĳ���Ϳ��� ����/�ڵ� ȸ��X
    , Panic = 1 << 2     //ȥ�� - ���� �Ұ���, ������� �ൿ�� / ���� �� �� ���� ����� �ڵ� ȸ��
    , Bind = 1 << 3      //��� - �ൿ �Ұ���, ��� ������ �Ǹ����� ���� ��ȭ�� �÷��� ȿ�� / ���� �� �� ���� ����� �ڵ� ȸ��
    , Sleep = 1 << 4      //���� - �ൿ �Ұ���, �ǰݽ� 25% �߰� �����, �ǰݽ� ȸ������ ���� / ���� �� �� ���� ����� �ڵ� ȸ��

    , All = int.MaxValue    //ġ��� ����
}

#region ClassField
[System.Serializable][JsonObject(MemberSerialization.OptIn)]
public class PartyMemberData
{
    public PartyMemberData()
    {

    }
    
    #region Field
    [JsonProperty] private string Name = "tst"; // { get; init; }     //�̸�
    [JsonProperty] private int MaxHP = 0;       //�ִ� ü��
    [JsonProperty] private int NowHP = 0;       //���� ü��
    [JsonProperty] private int MaxMP = 0;       //�ִ� ����
    [JsonProperty] private int NowMP = 0;       //���� ����
    [JsonProperty] private bool IsDead = false;     //��� ����

    [JsonProperty] private int St = 0;          //��-Į �⺻ ���� ���� ���, ���� ��ų ���� ���� ���
    [JsonProperty] private int Dx = 0;          //��-�� �⺻ ���� ���� ���, ���� ��ų ���� ���� ���
    [JsonProperty] private int Ma = 0;          //��-���� ��ų ���� ���
    [JsonProperty] private int Ag = 0;          //��-������ Ȯ��, �ൿ����, ���߷�, ȸ���� ���
    [JsonProperty] private int Lu = 0;          //��-ũ��Ƽ��, ȸ����, �����̻� ������, �����̻� ȸ���ӵ�, ������ ����� ���

    [JsonProperty] private AilmentType NowAilment = AilmentType.None;  //���� �ɷ��ִ� �����̻�

    [JsonProperty] private int BeginningLevel = 0;     //�ʱ� ����<-�Ǹ� �ʿ��� �Űܵ� ��
    [JsonProperty] private int Level = 0;       //���� ����
    [JsonProperty] private int EXP = 0;         //���� ����ġ

    [JsonIgnore] private List<SkillDataRec> LearnedSkillList = new List<SkillDataRec>();     //������ ��ų ����Ʈ

    [JsonProperty] private SkillAffinities? AffinityPhysical { get; init; }
    [JsonProperty] private SkillAffinities? AffinityGun { get; init; }
    [JsonProperty] private SkillAffinities? AffinityFire { get; init; }
    [JsonProperty] private SkillAffinities? AffinityIce { get; init; }
    [JsonProperty] private SkillAffinities? AffinityElectric { get; init; }
    [JsonProperty] private SkillAffinities? AffinityForce { get; init; }
    [JsonProperty] private SkillAffinities? AffinityLight { get; init; }
    [JsonProperty] private SkillAffinities? AffinityDark { get; init; }

    [JsonProperty] private bool IsNormalGun = false; // { get; init; }        //�Ϲ� ������ �ѼӼ��� ��� true, �� �ܿ��� false
    [JsonProperty] private int NormalMinHit = 0; //{ get; init; }        //�Ϲ� ������ �ּ� Ÿ��: �Ϻ� �Ǹ��� �����ϸ� ��������δ� 1ȸ
    [JsonProperty] private int NormalMaxHit = 0; //{ get; init; }        //�Ϲ� ������ �ִ� Ÿ��: �Ϻ� �Ǹ��� �����ϸ� ��������δ� 1ȸ


    #endregion

    /// <summary>
    /// ü�� ���� �Լ�
    /// </summary>
    /// <param name="Value">����� ��ġ</param>
    public virtual void LossHP(int Value, out bool IsTargetDead)
    {
        NowHP -= Value;

        //���� ü���� 0 ���ϰ� �Ǹ� ��� ó��
        if (NowHP <= 0)
        {
            NowHP = 0;
            IsDead = true;
        }
        IsTargetDead = IsDead;
    }

    /// <summary>
    /// ��Ȱ
    /// </summary>
    /// <param name="Value">��Ȱ�� ������ ��Ȱ�� ü��</param>
    public void Revival(int Value)
    {
        IsDead = false;
        NowHP = Value;
    }

    /// <summary>
    /// ���� ����ߴ��� ��ȯ
    /// </summary>
    /// <returns></returns>
    public bool ReturnIsDead()
    {
        return IsDead;
    }

    /// <summary>
    /// �� ��ų, ���� ������ ü�� ȸ��
    /// </summary>
    /// <param name="Value">ȸ�� ��ġ</param>
    public void RecoverHP(int? Value, out int RecovredHPValue)
    {
        if (Value == null)
        {
            //Ǯü�� ȸ������ ��� �ణ�� ��길 ��ġ�� �ٷ� ����
            RecovredHPValue = MaxHP - NowHP;
            NowHP = MaxHP;
        }
        
        int Tmp = (int)Value;   //����ȯ�� ����

        if ((int)Value < MaxHP - NowHP)
        {
            RecovredHPValue = (int)Tmp;
            NowHP += Tmp;
        }
        else
        {
            RecovredHPValue = MaxHP - NowHP;
            NowHP = MaxHP;
        }
        
    }

    /// <summary>
    /// �� ��ų, ���� ������ ���� ȸ��
    /// </summary>
    /// <param name="Value">ȸ�� ��ġ</param>
    public void RecoverMP(int? Value, out int RecovredMPValue)
    {
        if (Value == null)
        {
            //Ǯ���� ȸ������ ��� �ణ�� ��길 ��ġ�� �ٷ� ����
            RecovredMPValue = MaxMP - NowMP;
            NowMP = MaxMP;
        }

        int Tmp = (int)Value;   //����ȯ�� ����

        if ((int)Value < MaxMP - NowMP)
        {
            RecovredMPValue = (int)Tmp;
            NowMP += Tmp;
        }
        else
        {
            RecovredMPValue = MaxMP - NowMP;
            NowMP = MaxMP;
        }

    }

    /// <summary>
    /// �����̻� ����
    /// </summary>
    /// <param name="newAil">�߰��� �����̻�</param>
    public void SetAil(AilmentType newAil)
    {
        NowAilment |= newAil;
    }

    /// <summary>
    /// �ɷ� �ִ� �����̻� ����
    /// </summary>
    /// <param name="CureAilmentType">���� ��� �����̻�</param>
    public void CureAil(AilmentType CureAilmentType)
    {
        //���� ������ �ִ� �����̻��� ���� ��� �� �̻��� ó�� ���� ����
        if (NowAilment == AilmentType.None)
            return;             
        
        //���� ġ���Ⱑ ��� �����̻� ġ������ ��� ������ �ִ� �����̻��� ��������� ����
        if (CureAilmentType.HasFlag(AilmentType.All))
        {
            NowAilment = AilmentType.None;
            return;
        }

        /*
        HasFlag�� ���� ��� �����̻� ����Ʈ�� �ش� �����̻��� �����ϴ��� Ȯ���ϰ�, ���� ��� ĳ���Ͱ� �ش� �����̻��� ������ �ִ��� Ȯ�� �� �´ٸ� �ش� �����̻��� ���� 
        */
        //�� ġ��
        if (CureAilmentType.HasFlag(AilmentType.Poison) && NowAilment.HasFlag(AilmentType.Poison))
        {
            NowAilment &= ~AilmentType.Poison;
        }

        //���� ġ��
        if (CureAilmentType.HasFlag(AilmentType.Sick) && NowAilment.HasFlag(AilmentType.Sick))
        {
            NowAilment &= ~AilmentType.Sick;
        }

        //�ӹ� ġ��
        if (CureAilmentType.HasFlag(AilmentType.Bind) && NowAilment.HasFlag(AilmentType.Bind))
        {
            NowAilment &= ~AilmentType.Bind;
        }

        //ȥ�� ġ��
        if (CureAilmentType.HasFlag(AilmentType.Panic) && NowAilment.HasFlag(AilmentType.Panic))
        {
            NowAilment &= ~AilmentType.Panic;
        }

        //���� ġ��
        if (CureAilmentType.HasFlag(AilmentType.Sleep) && NowAilment.HasFlag(AilmentType.Sleep))
        {
            NowAilment &= ~AilmentType.Sleep;
        }
    }

    /// <summary>
    /// �޽� ��� �Ǵ� �������� �̿��ؼ� ��� ���� ȸ�� 
    /// </summary>
    public void RecoverAllByRest()
    {
        NowHP = MaxHP;
        NowMP = MaxMP;
        CureAil(AilmentType.All);
    }


    #region ReturnStatus

    /// <summary>
    /// �̸� ��ȯ
    /// </summary>
    /// <returns></returns>
    public string ReturnName()
    {
        return Name;
    }

    /// <summary>
    /// �� ���� ��ġ ��ȯ
    /// </summary>
    /// <returns>�� ����</returns>
    public int ReturnSt()
    {
        return St;
    }

    /// <summary>
    /// �� ���� ��ġ ��ȯ
    /// </summary>
    /// <returns>�� ����</returns>
    public int ReturnDx()
    {
        return Dx;
    }

    /// <summary>
    /// �� ���� ��ġ ��ȯ
    /// </summary>
    /// <returns>�� ����</returns>
    public int ReturnMa()
    {
        return Ma;
    }

    /// <summary>
    /// �� ���� ��ġ ��ȯ
    /// </summary>
    /// <returns>�� ����</returns>
    public int ReturnAg()
    {
        return Ag;
    }

    /// <summary>
    /// �� ���� ��ġ ��ȯ
    /// </summary>
    /// <returns>�� ����</returns>
    public int ReturnLu()
    {
        return Lu;
    }

    /// <summary>
    /// ��ü�� ����/���� ��ȯ
    /// </summary>
    /// <param name="AttackedSkillType"></param>
    /// <returns></returns>
    public SkillAffinities? ReturnMatchingAffinity(SkillTypeSort AttackedSkillType)
    {
        switch (AttackedSkillType)
        {
            case SkillTypeSort.Physical:
                return AffinityPhysical;
            case SkillTypeSort.Gun:
                return AffinityGun;
            case SkillTypeSort.Fire:
                return AffinityFire;
            case SkillTypeSort.Ice:
                return AffinityIce;
            case SkillTypeSort.Electric:
                return AffinityElectric;
            case SkillTypeSort.Force:
                return AffinityForce;
            case SkillTypeSort.Light:
                return AffinityLight;
            case SkillTypeSort.Dark:
                return AffinityDark;
            default:
                return null;
        }
    }

    /// <summary>
    /// �Ϲ� ������ ��Ÿ������ ����
    /// </summary>
    /// <returns>��Ÿ���� ��� true</returns>
    public bool ReturnIsNormalGun()
    {
        return IsNormalGun;
    }

    /// <summary>
    /// �Ϲ� ���� �ּ� Ƚ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnNormalMinHit()
    {
        return NormalMinHit;
    }

    /// <summary>
    /// �Ϲ� ���� �ִ� Ƚ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnNormalMaxHit()
    {
        return NormalMaxHit;
    }

    /// <summary>
    /// ���⿡ �ɷȴ��� �ľǿ�: �ɷ����� ���� ������� �ɾ�� �ϹǷ�
    /// </summary>
    /// <returns></returns>
    public bool ReturnIsSick()
    {
        return (NowAilment.HasFlag(AilmentType.Sick));
    }

    /// <summary>
    /// �ʱ� ���� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnBeginningLv()
    {
        return BeginningLevel;
    }

    /// <summary>
    /// ������ ��ų ���� ��ȯ
    /// </summary>
    /// <returns>������ ��ų ����</returns>
    public int ReturnNumberOfSkill()
    {
        return LearnedSkillList.Count;
    }

    public SkillDataRec ReturnSkillByIndex(int Index)
    {
        return LearnedSkillList[Index];
    }

    /// <summary>
    /// �ִ� ü�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnMaxHP()
    {
        return MaxHP;
    }


    /// <summary>
    /// �ִ� ���� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnMaxMP()
    {
        return MaxMP;
    }

    /// <summary>
    /// ���� ü�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnNowHP()
    {
        return NowHP;
    }

    /// <summary>
    /// ���� ���� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnNowMP()
    {
        return NowMP;
    }


    #endregion

    public void AddNewSkill(SkillDataRec NewSkill)
    {
        //��ų�� �ִ� 8������ ���� ����
        if (LearnedSkillList.Count < 8)
        {
            LearnedSkillList.Add(NewSkill);
        }
    }
}

[System.Serializable][JsonObject(MemberSerialization.OptIn)]
public class PartyDemonData : PartyMemberData
{
    [JsonProperty] private int ID = 0;          //�Ǹ� �ĺ� ��ȣ
    [JsonProperty] private bool IsEntry = false;    //���� ������ Ȯ�ο�

    
    /// <summary>
    /// �Ǹ� ID ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnID()
    {
        return ID;
    }

    /// <summary>
    /// ���� ��Ʈ���� �ִ��� ��ȯ
    /// </summary>
    /// <returns></returns>
    public bool ReturnIsEntry()
    {
        return IsEntry;
    }

    /// <summary>
    /// ��Ʈ�� ��ü
    /// </summary>
    /// <param name="Value"></param>
    public void SwapEntry(bool Value)
    {
        IsEntry = Value;
    }

    /// <summary>
    /// ���� �Ǹ��� ü���� ���ϸ� ��Ʈ������ �ڵ����� �����Ƿ� �������̵��ؼ� �߰����� �κ��� ���δ�
    /// </summary>
    /// <param name="Value"></param>
    public override void LossHP(int Value, out bool IsTargetDead)
    {
        base.LossHP(Value, out IsTargetDead);

        if (base.ReturnIsDead())
        {
            //��Ƽ ��� �����ڿ��� �ڽ��� ��Ʈ������ ���޶�� ����
            GameManager.Instance.ReturnPartyManager().RemoveInEntry(this);
        }
    }

    /// <summary>
    /// ��Ʈ������ ���޶�� ��û
    /// </summary>
    public void CallRemoveThis()
    {
        GameManager.Instance.ReturnPartyManager().RemoveInEntry(this);
    }
}

[System.Serializable]
public class PlayerCharacterData : PartyMemberData
{

}

#endregion

public class PartyMemberManager : IPartySubject
{
    #region ObserverFiled

    private List<IPartyObserver> thisObserverList = new List<IPartyObserver>();   //������ ����Ʈ

    /// <summary>
    /// ������ �߰�
    /// </summary>
    /// <param name="NewObserver"></param>
    public void AddObserver(IPartyObserver NewObserver)
    {
        thisObserverList.Add(NewObserver);
    }

    /// <summary>
    /// ������ ����Ʈ���� ����
    /// </summary>
    /// <param name="Observer"></param>
    public void RemoveObserver(IPartyObserver Observer)
    {
        if (thisObserverList.Contains(Observer))
            thisObserverList.Remove(Observer);
    }

    public void NotifyObserver()
    {
        foreach(IObserver Observer in thisObserverList)
        {
            //Observer.Update(this);
        }
    }

    #endregion

    private readonly JsonSerializerSettings serializerSetting = new()
    {
        Formatting = Formatting.Indented,
        ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
    };

    public void SaveTMP()
    {
        PartyDemonData TMP = new PartyDemonData();
        TMP.AddNewSkill(TestScript.Instance.ReturnClass().ReturnSkillData(1));

        

        StringBuilder SaveSB = new StringBuilder();
        SaveSB.Append("test.json");
        string JsonFile;
        //= JsonUtility.ToJson(TMP);
        JsonFile = JsonConvert.SerializeObject(TMP, Formatting.Indented);
        Debug.Log(JsonFile);
        string SaveDataDirectory = Application.persistentDataPath + "/SaveData/";
        File.WriteAllText(SaveDataDirectory + SaveSB.ToString(), JsonFile);
    }

    /// <summary>
    /// �׽�Ʈ�� �ӽ� �Լ�
    /// </summary>
    public void LoadTMP()
    {
        AsyncOperationHandle<TextAsset> TextAssetHandle;
        TextAssetHandle = Addressables.LoadAssetAsync<TextAsset>("TestParty");

        TextAssetHandle.Completed += Handle =>
        {
            if (TextAssetHandle.Status == AsyncOperationStatus.Failed)
            {
                int a = 100;
            }

            PartyDemonList = JsonConvert.DeserializeObject<List<PartyDemonData>>(TextAssetHandle.Result.text);
            EntryDemonList = PartyDemonList;
        };

        //AsyncOperationHandle<TextAsset> TextAssetHandle2;
        //TextAssetHandle2 = Addressables.LoadAssetAsync<TextAsset>("TestPlayer");
        //TextAssetHandle2.Completed += Handle =>
        //{
        //    if (TextAssetHandle2.Status == AsyncOperationStatus.Failed)
        //    {
        //        int b = 100;
        //    }
        //
        //    PlayerCharacter = JsonConvert.DeserializeObject<PlayerCharacterData>(TextAssetHandle2.Result.text);
        //    Addressables.Release(TextAssetHandle2);
        //};

        int i = 0;
    }

    private PlayerCharacterData PlayerCharacter;
    private List<PartyDemonData> PartyDemonList = new List<PartyDemonData>();        //��ü ���� �Ǹ� ����Ʈ
#nullable enable
    private List<PartyDemonData?> EntryDemonList = new List<PartyDemonData?>();     //���� ���� �Ǹ� ����Ʈ(�� ��), �� �Ǹ��� ���� ����� ĭ�� null ó��
#nullable disable


    private int MaxNumberOfDemon = 32;          //���ÿ� ���忡 ���� �� �ִ� �Ǹ��� �ִ� ���� ���� ����

    /// <summary>
    /// ���� ���� �� ���� ���忡 �ִ� ���� �Ǹ� ����Ʈ �ʱ�ȭ
    /// </summary>
    private void InitPartyDemonList()
    {
        //���� �Ǹ� ���� List�� �ִ� ���̴� ������ �����Ƿ� �̸� �޸𸮸� �Ҵ��ؼ� �ְ� �� �� ���Ҵ��� �̷������ �ʰ� �Ѵ�
        PartyDemonList.Capacity = MaxNumberOfDemon;
        EntryDemonList.Capacity = 3;
    }

    /// <summary>
    /// ���� ���� �� ���� ���� ���� ���� �Ǹ� ����Ʈ �ʱ�ȭ
    /// </summary>
    private void InitEntryDemonList()
    {

    }

    /// <summary>
    /// ���� ���� �Ǹ� ��ü
    /// </summary>
    /// <param name="PartyIndex"></param>
    /// <param name="EntryIndex"></param>
    public void SwapEntryDemon(int PartyIndex, int EntryIndex)
    {
        //������ ������ ����ִ� ����(null)���� üũ, null�̶�� �ٷ� �־������ �ȴ�
        if (EntryDemonList[EntryIndex] != null)
            EntryDemonList[EntryIndex].SwapEntry(false);        //���� �ִ� �༮�� ��Ʈ������ ���ܷ� ����


        EntryDemonList[EntryIndex] = PartyDemonList[PartyIndex];        //�ִ� �Ǹ��� ��ü�� �Ǹ� ��ȯ
        EntryDemonList[EntryIndex].SwapEntry(true);         //���� ��Ʈ���� �� �Ǹ��� ��Ʈ�� �������� ����
    }

    /// <summary>
    /// �Ǹ��� ��Ʈ������ ����
    /// </summary>
    /// <param name="CallTarget">ȣ���� ���</param>
    public void RemoveInEntry(PartyDemonData CallTarget)
    {
        int CallTargetIndex;        //ȣ���� ��ü�� ����Ʈ���� � �ε����� ���� �ִ��� ���� ����

        //ȣ���� ��ü�� ����� ����Ʈ�� �����ϴ� �� �´ٸ� �����Ѵ�
        if (EntryDemonList.Contains(item: CallTarget))
        {
            CallTargetIndex = EntryDemonList.IndexOf(item: CallTarget);
            CallTarget.SwapEntry(false);
            EntryDemonList[CallTargetIndex] = null;

            foreach (IObserver Observer in thisObserverList)
            {
                //Observer.Update(this);
            }
        }
    }

    /// <summary>
    /// ���ΰ� ĳ���� ������ ��ȯ
    /// </summary>
    /// <returns></returns>
    public PlayerCharacterData ReturnPlayerCharacterData()
    {
        return PlayerCharacter;
    }

#nullable enable
    /// <summary>
    /// ��Ʈ���� �� �ִ� ���� �Ǹ� ������ ��ȯ
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    public PartyDemonData? ReturnEntryDemonData(int Index)
    {
        return EntryDemonList[Index];
    }
#nullable disable



    /// <summary>
    /// ��ü���� ȸ�� �Լ� ȣ��
    /// </summary>
    /// <param name="Target">���� ���</param>
    /// <param name="Value">ȸ�� ��ġ</param>
    /// <param name="RecovredHPValue">���� ȸ���� ��ġ</param>
    public void CallRecoverHP(PartyDemonData Target, int? Value, out int RecovredHPValue)
    {
        Target.RecoverHP(Value, out RecovredHPValue);
    }

    /// <summary>
    /// �̸��� ��ȯ�϶�� ��ü���� ����
    /// </summary>
    /// <param name="Target">���</param>
    /// <returns>����� �̸�</returns>
    public string CallReturnName(PartyDemonData Target)
    {
        return Target.ReturnName();
    }

    /// <summary>
    /// ���ݽ� ����/���� üũ�϶�� ��ü���� ����
    /// </summary>
    /// <param name="Target">������ �޴� ��Ƽ��</param>
    /// <param name="AttackedSkillType">���� ��ų�� Ÿ��</param>
    /// <returns></returns>
    public SkillAffinities? CallReturnAffinity(PartyDemonData Target, SkillTypeSort AttackedSkillType)
    {
        return Target.ReturnMatchingAffinity(AttackedSkillType);
    }

    
}