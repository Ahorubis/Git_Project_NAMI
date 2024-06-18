using UnityEngine;

public class ButtonList : MonoBehaviour
{
    public void Number()
    {
        GameObject.Find("Player").GetComponent<Player>().PressedButton = gameObject;
    }
}
