using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [HideInInspector] public string ObjectCode;

    private TextMeshProUGUI scorePlus;
    private RectTransform rectTransform;

    private Player player;

    private Color textColor;
    private Vector3 direction;

    private void Awake()
    {
        scorePlus = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();

        player = GameObject.Find("Player").GetComponent<Player>();

        transform.parent = GameObject.Find("MainCanvas").transform;
    }

    private void Start()
    {
        rectTransform.position = Camera.main.WorldToScreenPoint(player.DamageObjectPos);

        textColor = scorePlus.color;
        direction = new Vector3(0, 1).normalized;

        StartCoroutine("EffectTimer");
        PointCalculate();
    }

    private void PointCalculate()
    {
        if (ObjectCode == "Whitestone") scorePlus.text = "+80";
        else if (ObjectCode == "Blackstone") scorePlus.text = "+350";
        else if (ObjectCode == "Cornerstone") scorePlus.text = "+30";
        else if (ObjectCode == "Blobfish") scorePlus.text = "+150";
        else if (ObjectCode == "Anglerfish") scorePlus.text = "+280";
        else if (ObjectCode == "GiantSquid") scorePlus.text = "+350";
    }

    private IEnumerator EffectTimer()
    {
        float timer = 0;

        while (true)
        {
            scorePlus.color = new Color(textColor.r, textColor.g, textColor.b, 1 - timer);

            if (timer > 1)
            {
                Destroy(gameObject);
                break;
            }

            if (Time.timeScale == 1)
            {
                timer += Time.deltaTime;
                rectTransform.Translate(direction * timer);
            }

            else if (Time.timeScale == 0)
            {
                timer += Time.deltaTime * 0;
                rectTransform.Translate(direction * timer * 0);
            }

            yield return null;
        }
    }
}
