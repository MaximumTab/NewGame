using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayEnemyStuffs : MonoBehaviour
{
    public EnemyInfo SelectedInfo;
    public GameObject Holder;
    public float ScrollSensitivity;
    public Image Icon;
    public TMP_Text Name;
    public TMP_Text Type;
    public TMP_Text HP;
    public TMP_Text ATK;
    public TMP_Text Block;
    public TMP_Text Speed;
    public TMP_Text Description;

    private void Start()
    {
        Icon = Holder.GetComponentInChildren<Image>();
    }
    

    void Update()
    {
        if (SelectedInfo != null&&SelectedInfo.EnemyPref)
        {
            Holder.SetActive(true);
            if (Holder.GetComponent<RectTransform>().localPosition.y < 0)
            {
                RectTransform rect = Holder.GetComponent<RectTransform>();
                rect.localPosition.Set(rect.localPosition.x,0,rect.localPosition.z);
            }

            EnemyStats EStats= (EnemyStats)SelectedInfo.EnemyPref.GetComponent<EnemyBehaviour>().Stats;
            if (Icon&&SelectedInfo.EnemyPic)
            {
                Icon.sprite = SelectedInfo.EnemyPic;
            }

            if (Name)
            {
                Name.text = EStats.Name;
            }

            if (Type)
            {
                Type.text = EStats.Type.ToString();
            }

            if (HP)
            {
                HP.text = "Hp:\n"+ EStats.MaxHp;
            }

            if (ATK)
            {
                ATK.text = "ATK:\n"+EStats.Atk;
            }

            if (Block)
            {
                Block.text = "Block:\n" + EStats.Block;
            }

            if (Speed)
            {
                Speed.text = "Speed:\n" + EStats.Speed;
            }

            if (Description)
            {
                Description.text = SelectedInfo.Description+"\n \n";
                foreach (EntityStats.Abil AB in EStats.Abilities)
                {
                    Description.text += "[" + AB.Ability.AblName + "] : " + AB.Range + "\n" +
                                               AB.Ability.Description + "\n \n";
                }
            }
        }
        else
        {
            Holder.SetActive(false);
        }

    }
}
