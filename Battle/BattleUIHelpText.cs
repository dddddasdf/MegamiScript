using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json;

public class HelpTextData : MonoBehaviour
{
    private static List<HelpText> HelpTextDataList;   //도움말 텍스트 정적 리스트,  게임 매니저가 게임이 켜질 때 초기화

    /// <summary>
    /// 호출용
    /// </summary>
    public HelpTextData()
    {

    }

    /// <summary>
    /// 헬프 텍스트 데이터 파일 초기화
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
            //몬스터 데이터 파일을 전부 읽어들였으면 역직렬화 하여 리스트에 저장
            TextAsset DataStack = LoadHandle.Result;
            HelpTextDataList = JsonConvert.DeserializeObject<List<HelpText>>(DataStack.text);
            Addressables.Release(LoadHandle);   //리스트에 데이터 저장 완료했으므로 어드레서블 해제
        };
    }

    /// <summary>
    /// 도움말 텍스트 정보 받아오기
    /// </summary>
    /// <param name="HelpCode">불러와야 하는 도움말의 코드명</param>
    /// <returns></returns>
    public string ReturnHelpText(string HelpCode)
    {
        HelpText DataTmp = HelpTextDataList.Find(Data => Data.Code == HelpCode);

        return DataTmp.Text;
    }

}
