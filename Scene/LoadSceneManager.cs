using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager Instance { get; private set; }

    #region SetVariabels

    private int NowFieldIndex;  //전투씬에 들어가기 전에 현재 있던 필드 기록
    public event Action SceneEvent;

    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //StartCoroutine(Test());

            return;
        }
        Destroy(gameObject);
    }

    private IEnumerator Test()
    {
        AsyncOperation LoadingOperation = SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
        while (!LoadingOperation.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(2));    //배틀 씬 메인 활성화 씬 지정
    }

    /// <summary>
    /// 씬 로딩 후 함수 작동 예약 
    /// </summary>
    public void AddSceneEvent(Action EventMethod)
    {
        SceneEvent += () =>
        {
            EventMethod();
        };
    }

    #region BattleScene

    /// <summary>
    /// 전투씬 세팅
    /// </summary>
    public void SetBattleScene()
    {

        StartCoroutine(LoadBattleSceneCoroutine());
    }

    private IEnumerator LoadBattleSceneCoroutine()
    {
        NowFieldIndex = SceneManager.GetActiveScene().buildIndex;   //현재 활성화 중인 필드의 씬 빌드 인덱스 저장

        AsyncOperation LoadingOperation = SceneManager.LoadSceneAsync((int)SceneIndex.BattleScene, LoadSceneMode.Additive);
        
        while (!LoadingOperation.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)SceneIndex.BattleScene));    //배틀 씬 메인 활성화 씬 지정
        SceneManager.UnloadSceneAsync(NowFieldIndex);   //필드 씬 비활성화
    }

    /// <summary>
    /// 전투씬 종료
    /// </summary>
    public void EndBattleScene()
    {
        StartCoroutine(UnLoadBattleSceneCoroutine());
    }

    public IEnumerator UnLoadBattleSceneCoroutine()
    {
        AsyncOperation LoadingOperationHandle = SceneManager.LoadSceneAsync(NowFieldIndex, LoadSceneMode.Additive);   //이전에 있던 필드 씬 불러오기

        while (!LoadingOperationHandle.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(NowFieldIndex)); //필드 씬을 메인 활성화 씬으로 지정
        SceneManager.UnloadSceneAsync((int)SceneIndex.BattleScene); //전투 씬 언로드

        SceneEvent?.Invoke();   //씬 로딩이 다 되었으니 예약해둔 함수들 작동하라고 알림
        SceneEvent = null;
    }

    #endregion
}
