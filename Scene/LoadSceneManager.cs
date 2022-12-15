using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager Instance { get; private set; }

    #region SetVariabels

    private int NowFieldIndex;  //�������� ���� ���� ���� �ִ� �ʵ� ���
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

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(2));    //��Ʋ �� ���� Ȱ��ȭ �� ����
    }

    /// <summary>
    /// �� �ε� �� �Լ� �۵� ���� 
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
    /// ������ ����
    /// </summary>
    public void SetBattleScene()
    {

        StartCoroutine(LoadBattleSceneCoroutine());
    }

    private IEnumerator LoadBattleSceneCoroutine()
    {
        NowFieldIndex = SceneManager.GetActiveScene().buildIndex;   //���� Ȱ��ȭ ���� �ʵ��� �� ���� �ε��� ����

        AsyncOperation LoadingOperation = SceneManager.LoadSceneAsync((int)SceneIndex.BattleScene, LoadSceneMode.Additive);
        
        while (!LoadingOperation.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)SceneIndex.BattleScene));    //��Ʋ �� ���� Ȱ��ȭ �� ����
        SceneManager.UnloadSceneAsync(NowFieldIndex);   //�ʵ� �� ��Ȱ��ȭ
    }

    /// <summary>
    /// ������ ����
    /// </summary>
    public void EndBattleScene()
    {
        StartCoroutine(UnLoadBattleSceneCoroutine());
    }

    public IEnumerator UnLoadBattleSceneCoroutine()
    {
        AsyncOperation LoadingOperationHandle = SceneManager.LoadSceneAsync(NowFieldIndex, LoadSceneMode.Additive);   //������ �ִ� �ʵ� �� �ҷ�����

        while (!LoadingOperationHandle.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(NowFieldIndex)); //�ʵ� ���� ���� Ȱ��ȭ ������ ����
        SceneManager.UnloadSceneAsync((int)SceneIndex.BattleScene); //���� �� ��ε�

        SceneEvent?.Invoke();   //�� �ε��� �� �Ǿ����� �����ص� �Լ��� �۵��϶�� �˸�
        SceneEvent = null;
    }

    #endregion
}
