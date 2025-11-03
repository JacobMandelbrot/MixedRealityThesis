using UnityEngine;

public class ButtonFunctions : MonoBehaviour
{
    public void ChangeModel()
    {
        DemoGameManager.Instance.ChangeModel();
    }
    
    public void ChangeLight()
    {
        DemoGameManager.Instance.ChangeLight();
    }
    
    public void ChangeToDummy()
    {
        DemoGameManager.Instance.SpawnDummy();
    }
}
