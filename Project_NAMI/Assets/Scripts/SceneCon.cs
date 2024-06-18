using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneCon : MonoBehaviour
{
    protected static SceneCon sceneCon;

    public static SceneCon Scene
    {
        get
        {
            if (sceneCon == null)
            {
                var sceneObj = FindObjectOfType<SceneCon>();

                if (sceneObj != null) sceneCon = sceneObj;
                else sceneCon = LoadingCreate();
            }

            return sceneCon;
        }

        private set { sceneCon = value; }
    }

    public static SceneCon LoadingCreate()
    {
        var SceneLoaderPrefab = Resources.Load<SceneCon>("Prefabs/LoadingCanvas");
        return Instantiate(SceneLoaderPrefab);
    }

    [SerializeField] private CanvasGroup fadeImage;

    [SerializeField] private Slider progressBar;

    [SerializeField] private float setFadeTimer = 2f;

    private SystemCon systemCon;

    private string loadSceneName;

    private void Awake()
    {
        if (sceneCon != null) Destroy(this.gameObject);

        else
        {
            sceneCon = this;
            DontDestroyOnLoad(this.gameObject);
        }

        systemCon = GameObject.Find("SystemCon").GetComponent<SystemCon>();
    }

    private void Start()
    {
        fadeImage.alpha = 0;
        progressBar.value = 0;
    }

    //로딩화면 효과 끝날 때
    private void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == loadSceneName)
        {
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= LoadSceneEnd;
        }
    }

    //씬 로딩 함수
    public void LoadScene(string sceneName)
    {
        gameObject.SetActive(true);
        SceneManager.sceneLoaded += LoadSceneEnd;
        loadSceneName = sceneName;
        StartCoroutine(Load(sceneName));
    }

    //로딩화면 로딩 진행상황
    private IEnumerator Load(string sceneName)
    {
        progressBar.value = 0f;
        yield return StartCoroutine(Fade(true));

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float timer = 0f;

        while (!op.isDone)
        {
            float percentage = progressBar.value;

            yield return null;
            timer += Time.unscaledDeltaTime;

            //로딩 진행상황이 90% 미만일 시
            if (op.progress < 0.9f)
            {
                progressBar.value = Mathf.Lerp(progressBar.value, op.progress, timer);
                if (progressBar.value >= op.progress) timer = 0f;
            }

            //로딩 진행상황이 90% 이상일 시
            else
            {
                progressBar.value = Mathf.Lerp(progressBar.value, 1f, timer);

                //로딩완료 시, 다음 씬으로 전환
                if (progressBar.value == 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    //로딩화면 페이드 관련 코루틴
    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;

        while (timer <= setFadeTimer)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 2f;
            fadeImage.alpha = Mathf.Lerp(isFadeIn ? 0 : 1, isFadeIn ? 1 : 0, timer);
        }

        if (!isFadeIn)
        {
            yield return null;
            gameObject.SetActive(false);
        }
    }
}
