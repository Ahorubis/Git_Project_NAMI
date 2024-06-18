using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pearl : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Player player;

    private float speed = 10;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    private void LateUpdate()
    {
        transform.position += Vector3.right * Time.deltaTime * speed;

        if (!PearlOnScreen()) Destroy(gameObject);
    }

    private bool PearlOnScreen()
    {
        Vector2 point = Camera.main.WorldToViewportPoint(transform.position);

        bool onScreen = point.x > 0 && point.x < 1 && point.y > 0 && point.y < 1;

        return onScreen;
    }

    private void PearlCrush(Collision2D other, string tag, int score)
    {
        player.CrushObject(other, tag, score);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Whitestone")) PearlCrush(other, "Whitestone", 80);
        else if (other.gameObject.CompareTag("Blackstone")) PearlCrush(other, "Blackstone", 350);
        else if (other.gameObject.CompareTag("Cornerstone")) PearlCrush(other, "Cornerstone", 30);
        else if (other.gameObject.CompareTag("Blobfish")) PearlCrush(other, "Blobfish", 150);
        else if (other.gameObject.CompareTag("Anglerfish")) PearlCrush(other, "Anglerfish", 280);
        else if (other.gameObject.CompareTag("GiantSquid")) PearlCrush(other, "GiantSquid", 350);
    }
}
