using UnityEngine;

public class ButtonInteractions : MonoBehaviour
{
    public void DummyMode()
    {
        GameManager.Instance.ChangeMode("Dummy");
    }
    
    public void ModelMode()
    {
        GameManager.Instance.ChangeMode("Models");
    }
}
