using UnityEngine;

public class Deployable : MonoBehaviour
{
    public bool deployable;
    private Material mat;
    private static readonly int CanPlace = Shader.PropertyToID("_CanPlace");

    private void Start()
    {
        mat = gameObject.GetComponentInChildren<Renderer>().sharedMaterial;
    }

    public void IsDeployableChange(int litIT)
    {
        mat.SetFloat(CanPlace,litIT);
    }
}
