using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json;

public class HelpTextData : MonoBehaviour
{
    private static List<HelpText> HelpTextDataList;   //���� �ؽ�Ʈ ���� ����Ʈ,  ���� �Ŵ����� ������ ���� �� �ʱ�ȭ

    /// <summary>
    /// ȣ���
    /// </summary>
    public HelpTextData()
    {

    }

    /// <summary>
    /// ���� �ؽ�Ʈ ������ ���� �ʱ�ȭ
    /// </summary>
    public static void InitHelpTextData()
    {
        HelpTextDataList = new List<HelpText>();

        AsyncOperationHandle<TextAsset> LoadHandle = Addressables.LoadAssetAsync<TextAsset>("HelpTextData");

#if UNITY_EDITOR
        if (LoadHandle.Status == AsyncOperationStatus.Failed)
        {

        }

#endif
        LoadHandle.Completed += Handle =>
        {
            //���� ������ ������ ���� �о�鿴���� ������ȭ �Ͽ� ����Ʈ�� ����
            TextAsset DataStack = LoadHandle.Result;
            HelpTextDataList = JsonConvert.DeserializeObject<List<HelpText>>(DataStack.text);
            Addressables.Release(LoadHandle);   //����Ʈ�� ������ ���� �Ϸ������Ƿ� ��巹���� ����
        };
    }

    /// <summary>
    /// ���� �ؽ�Ʈ ���� �޾ƿ���
    /// </summary>
    /// <param name="HelpCode">�ҷ��;� �ϴ� ������ �ڵ��</param>
    /// <returns></returns>
    public string ReturnHelpText(string HelpCode)
    {
        HelpText DataTmp = HelpTextDataList.Find(Data => Data.Code == HelpCode);

        return DataTmp.Text;
    }

}
