using UnityEngine;

public class FmodEvents : MonoBehaviour
{
    public static FmodEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("found more than one FmodEvents in the scene");
        }
        instance = this;
    }    
}
