using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Range(0, 2.8f)][SerializeField] private float slingDistance = 2.8f;

    [SerializeField] private GameObject pivot;
    [SerializeField] private GameObject score;
    [SerializeField] private GameObject jinju;
    [SerializeField] private GameObject story;
    [SerializeField] private GameObject dust;

    [SerializeField] private CanvasGroup buttonGroup;
    [SerializeField] private Button pause;
    [SerializeField] private Slider scoreSlider;

    [SerializeField] private float scoreTimer;
    [SerializeField] private float forceAmount;

    [SerializeField] private int goalScore;

    [SerializeField] private RuntimeAnimatorController[] fishes;

    public Sprite[] fishList;

    [Header("Fire Audio Clips")]
    public AudioClip selectFish;
    [SerializeField] private AudioClip drag;
    [SerializeField] private AudioClip fire;

    [Header("Destroy Audio Clips")]
    [SerializeField] private AudioClip dustClip;
    [SerializeField] private AudioClip pointClip;

    [Header("Skill Audio Clips")]
    [SerializeField] private AudioClip clamJinju;
    [SerializeField] private AudioClip blowfishExpansion;
    [SerializeField] private AudioClip sharkWave;

    [HideInInspector] public GameObject PressedButton;

    [HideInInspector] public Vector3 DamageObjectPos;

    [HideInInspector] public bool StatusPause;
    [HideInInspector] public bool StoryMode;

    [HideInInspector] public int ButtonIndex;

    private Camera mainCamera;
    private Rigidbody2D rigid;
    private SpringJoint2D sprintJoint;
    private LineRenderer lineRender;
    private SpriteRenderer spriteRen;
    private AudioSource audioSource;
    private Animator anime;

    private Story storyScript;
    private SystemCon system;

    private GameObject[] whitestoneTag;
    private GameObject[] blackstoneTag;
    private GameObject[] cornerstoneTag;
    private GameObject[] blobfishTag;
    private GameObject[] anglerfishTag;
    private GameObject[] giantSquidTag;

    private Vector3 playerResetPosition;
    private Vector2 playerResetScale;

    private Color invisibleColor = new Color(255, 255, 255, 0);

    private bool swimmingPlayer = false;
    private bool useSkill = false;
    private bool playingScreen = false;
    private bool sharkSkill = false;

    private float scoreToNext;

    private int totalScore = 0;
    private int playerScore = 0;
    private int scorePlus = 0;

    private void Awake()
    {
        scoreSlider.value = 0;

        StatusPause = false;
        StoryMode = false;
        PressedButton = null;

        mainCamera = Camera.main;
        rigid = GetComponent<Rigidbody2D>();
        sprintJoint = GetComponent<SpringJoint2D>();
        lineRender = GetComponent<LineRenderer>();
        spriteRen = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        anime = GetComponent<Animator>();

        storyScript = story.GetComponent<Story>();
        system = GameObject.Find("SystemCon").GetComponent<SystemCon>();

        playerResetPosition = transform.position;
        playerResetScale = transform.localScale;

        whitestoneTag = GameObject.FindGameObjectsWithTag("Whitestone");
        blackstoneTag = GameObject.FindGameObjectsWithTag("Blackstone");
        cornerstoneTag = GameObject.FindGameObjectsWithTag("Cornerstone");
        blobfishTag = GameObject.FindGameObjectsWithTag("Blobfish");
        anglerfishTag = GameObject.FindGameObjectsWithTag("Anglerfish");
        giantSquidTag = GameObject.FindGameObjectsWithTag("GiantSquid");

        ButtonIndex = 0;
    }

    private void Start()
    {
        TotalScoreCalculate(whitestoneTag, 80);
        TotalScoreCalculate(blackstoneTag, 350);
        TotalScoreCalculate(cornerstoneTag, 30);
        TotalScoreCalculate(blobfishTag, 150);
        TotalScoreCalculate(anglerfishTag, 280);
        TotalScoreCalculate(giantSquidTag, 350);

        if (spriteRen.sprite == fishList[1]) transform.localScale = new Vector2(0.7f, 0.7f);
        else transform.localScale = new Vector2(-0.7f, 0.7f);

        spriteRen.color = invisibleColor;
        anime.enabled = false;

        scoreToNext = goalScore / (float)totalScore;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && !useSkill && swimmingPlayer)
        {
            if (ButtonIndex == 2) Clam();
            else if (ButtonIndex == 4) DolphinOrShark("Dolphin");
        }
    }

    private void OnMouseDown()
    {
        if (ButtonIndex == 0 || playingScreen) return;

        lineRender.positionCount = 2;
        lineRender.SetPosition(0, pivot.transform.position);
        lineRender.SetPosition(1, transform.position);
        lineRender.startWidth = lineRender.endWidth = 0.1f;

        rigid.isKinematic = true;

        buttonGroup.interactable = false;
        pause.interactable = false;

        audioSource.PlayOneShot(drag);
    }

    private void OnMouseDrag()
    {
        if (ButtonIndex == 0 || playingScreen) return;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.WorldToScreenPoint(transform.position).z);
        Vector3 screenMousePos = mainCamera.ScreenToWorldPoint(mousePos);

        float fireDistance = Vector3.Distance(screenMousePos, pivot.transform.position);

        lineRender.SetPosition(1, transform.position);

        if (pivot.transform.position.x <= transform.position.x)
        {
            if (ButtonIndex == 2) transform.localScale = new Vector2(-0.7f, 0.7f);
            else transform.localScale = new Vector2(0.7f, 0.7f);
        }

        else 
        {
            if (ButtonIndex == 2) transform.localScale = new Vector2(0.7f, 0.7f);
            else transform.localScale = new Vector2(-0.7f, 0.7f);
        }

        if (fireDistance < slingDistance) transform.position = screenMousePos;

        else
        {
            float distanceCal = slingDistance / fireDistance;
            transform.position = Vector3.Lerp(pivot.transform.position, screenMousePos, distanceCal);
        }
    }

    private void OnMouseUp()
    {
        if (ButtonIndex == 0 || playingScreen) return;

        if (pivot.transform.position.x <= transform.position.x) StartCoroutine(FireOn(pivot, gameObject));
        else StartCoroutine(FireOn(gameObject, pivot));

        lineRender.positionCount = 0;

        rigid.isKinematic = false;

        PressedButton.SetActive(false);
        PressedButton = null;

        audioSource.PlayOneShot(fire);
    }

    private void SkillFire()
    {
        useSkill = true;
        switch (ButtonIndex)
        {
            case 3:
                Blowfish();
                audioSource.PlayOneShot(blowfishExpansion);
                break;
            case 5:
                DolphinOrShark("Shark");
                break;
            default:
                break;
        }
    }

    private void Clam()
    {
        GameObject pearl = Instantiate(jinju);
        pearl.transform.position = transform.position;
        audioSource.PlayOneShot(clamJinju);
    }

    private void Blowfish()
    {
        anime.enabled = true;
        anime.runtimeAnimatorController = fishes[0];
        anime.SetTrigger("Blowfish");

        transform.localScale = new Vector2(transform.localScale.x * 1.5f, transform.localScale.y * 1.5f);
        audioSource.PlayOneShot(blowfishExpansion);
    }

    private void DolphinOrShark(string fish)
    {
        if (fish == "Dolphin") rigid.velocity = Vector2.one * 25;
        else if (fish == "Shark")
        {
            audioSource.PlayOneShot(sharkWave);
            sharkSkill = true;
            anime.enabled = true;

            anime.runtimeAnimatorController = fishes[1];
            anime.SetTrigger("Shark");
        }
    }

    private IEnumerator Shark()
    {
        Vector2 origin = transform.position;

        float timer = 1;

        sharkSkill = false;
        audioSource.PlayOneShot(sharkWave);

        while (timer > 0)
        {
            float power = 0.5f;

            if (Time.timeScale == 1) timer -= Time.deltaTime;
            else if (Time.timeScale == 0) timer -= Time.deltaTime * 0;

            transform.position = origin + (Vector2)Random.insideUnitCircle * power * timer;
            yield return null;
        }

        transform.position = origin;
        rigid.velocity = Vector2.zero;
    }

    private bool GameObjectOnScreen()
    {
        Vector2 point = mainCamera.WorldToViewportPoint(transform.position);

        bool onScreen = point.x > 0 && point.x < 1 && point.y > 0 && point.y < 1;

        return onScreen;
    }

    private int TotalScoreCalculate(GameObject[] array, int score)
    {
        int total = 0;

        for (int i = 0; i < array.Length; i++) total += score;

        return totalScore += total;
    }

    public void CrushObject(Collision2D other, string tag, int score)
    {
        scorePlus += score;
        DamageObjectPos = other.transform.position;
        this.score.gameObject.GetComponent<Score>().ObjectCode = tag;

        if (ButtonIndex == 5 && sharkSkill) StartCoroutine("Shark");
        Instantiate(this.score);

        if (tag != "Whitestone" && tag != "Blackstone" && tag != "Cornerstone")
        {
            audioSource.PlayOneShot(pointClip);
            Destroy(other.gameObject);
        }

        else
        {
            GameObject temp = Instantiate(dust);
            temp.transform.position = transform.position;

            StartCoroutine(Dust(temp));
            Destroy(other.gameObject);
        }
    }

    public void SetWindow()
    {
        if (!StatusPause) return;
        
        Transform window = GameObject.Find("MainCanvas/PauseWindow").transform;
        window.SetAsLastSibling();
    }

    private IEnumerator Dust(GameObject dustEffect)
    {
        yield return null;
        audioSource.PlayOneShot(dustClip);

        yield return new WaitForSeconds(0.5f);
        Destroy(dustEffect);
    }

    private IEnumerator FireOn(GameObject left, GameObject right)
    {
        float calculate, x, y;

        playingScreen = true;
        
        while (true)
        {
            calculate = right.transform.position.x - left.transform.position.x;

            x = Mathf.Abs(rigid.velocity.x);
            y = Mathf.Abs(rigid.velocity.y);

            if (calculate <= 0)
            {
                swimmingPlayer = true;
                sprintJoint.enabled = false;
            }

            if (!sprintJoint.enabled && x < 0.2f && y < 0.2f || !GameObjectOnScreen())
            {
                StartCoroutine(ResetFish(2.5f));
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator ResetFish(float timer)
    {
        yield return new WaitForSeconds(timer);

        spriteRen.color = invisibleColor;
        if (anime.enabled) anime.enabled = false;

        if (ButtonIndex == 5) StopCoroutine("Shark");

        Destroy(GetComponent<PolygonCollider2D>());

        StartCoroutine(SliderAnime(scorePlus));

        sprintJoint.enabled = true;

        transform.localScale = playerResetScale;
        transform.position = playerResetPosition;
        transform.rotation = Quaternion.Euler(0, 0, 0);

        rigid.constraints = RigidbodyConstraints2D.FreezePosition;
        rigid.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        ButtonIndex = 0;

        swimmingPlayer = false;
        useSkill = false;
        playingScreen = false;

        rigid.constraints = ~RigidbodyConstraints2D.FreezePosition;
    }

    private IEnumerator SliderAnime(int plus)
    {
        float before = playerScore / (float)totalScore;
        float after = before + plus / (float)totalScore;
        float duration = 0;

        while (true)
        {
            scoreSlider.value = Mathf.Lerp(before, after, duration / scoreTimer);

            if (scoreSlider.value == after)
            {
                playerScore += plus;
                scorePlus = 0;

                if (scoreSlider.value >= scoreToNext)
                {
                    story.SetActive(true);
                    storyScript.previewStory = false;
                    system.PauseMode(true);
                    storyScript.StoryComent();
                    yield break;
                }

                buttonGroup.interactable = true;
                pause.interactable = true;

                yield break;
            }

            yield return null;
            duration += Time.unscaledDeltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject && !useSkill && swimmingPlayer) SkillFire();

        if (other.gameObject.CompareTag("Whitestone")) CrushObject(other, "Whitestone", 80);
        else if (other.gameObject.CompareTag("Blackstone")) CrushObject(other, "Blackstone", 350);
        else if (other.gameObject.CompareTag("Cornerstone")) CrushObject(other, "Cornerstone", 30);
        else if (other.gameObject.CompareTag("Blobfish")) CrushObject(other, "Blobfish", 150);
        else if (other.gameObject.CompareTag("Anglerfish")) CrushObject(other, "Anglerfish", 280);
        else if (other.gameObject.CompareTag("GiantSquid")) CrushObject(other, "GiantSquid", 350);
    }
}
