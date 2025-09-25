using UnityEngine;
using UnityEngine.UI;

public class EnemyDataIndex : MonoBehaviour
{
    public int index = 0;
    public Image sprite;
    public EnemyLayout Layout;

    public void OnClick()
    {
        Layout.OnClick(index);
    }
}
