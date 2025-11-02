using UnityEngine;

public class ChangeMode : MonoBehaviour
{
    public string mode;

    public void Change()
    {
        GameManager.Instance.ChangeMode(mode);
    }
}
