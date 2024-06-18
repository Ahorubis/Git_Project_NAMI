using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Story : MonoBehaviour
{
    [SerializeField] private Sprite character;
    [SerializeField] private Sprite enemy;

    [SerializeField] private string[] openingStories;
    [SerializeField] private string[] endingStories;

    [SerializeField] private Image fish;
    [SerializeField] private Button startButton;

    [SerializeField] private TMP_Text buttonComent;
    [SerializeField] private TMP_Text fishName;
    [SerializeField] private TMP_Text coment;

    [HideInInspector] public bool previewStory;

    private SystemCon system;

    private int storyIndex;

    private void Awake()
    {
        system = GameObject.Find("SystemCon").GetComponent<SystemCon>();

        previewStory = true;
        storyIndex = 0;
    }

    private void Start()
    {
        startButton.interactable = false;

        if (openingStories[storyIndex].Contains("(적)")) fish.sprite = enemy;
        else fish.sprite = character;

        StoryComent();
    }

    public void StoryComent()
    {
        if (previewStory)
        {
            if (storyIndex >= openingStories.Length - 1) buttonComent.text = "게임시작";
            else buttonComent.text = "다음대화";

            if (storyIndex < openingStories.Length) StartCoroutine(TypingComent(openingStories[storyIndex]));

            else
            {
                storyIndex = 0;
                previewStory = false;
                system.PauseMode(false);
                gameObject.SetActive(false);
            }
        }

        else
        {
            if (storyIndex >= endingStories.Length - 1) buttonComent.text = "다음으로";
            else buttonComent.text = "다음대화";

            if (storyIndex < endingStories.Length) StartCoroutine(TypingComent(endingStories[storyIndex]));

            else
            {
                int index = SceneManager.GetActiveScene().buildIndex + 1;
                string sceneName = "Stage" + index.ToString();

                system.PauseMode(false);

                if (!Application.CanStreamedLevelBeLoaded(sceneName)) SceneCon.Scene.LoadScene("MainScene");
                else SceneCon.Scene.LoadScene(sceneName);
            }
        }
    }

    public void StoryButton()
    {
        storyIndex++;
        startButton.interactable = false;
        StoryComent();
    }

    private IEnumerator TypingComent(string typingText)
    {
        int i = 0;

        system.PauseMode(true);

        if (typingText.Contains("(적) "))
        {
            fish.sprite = enemy;
            typingText = typingText.Replace("(적) ", "");
        }

        else fish.sprite = character;

        coment.text = null;

        while (i < typingText.Length)
        {
            coment.text += typingText[i].ToString();
            yield return new WaitForSecondsRealtime(0.05f);
            i++;

            if (i >= typingText.Length)
            {
                startButton.interactable = true;
                yield break;
            }
        }
    }
}
