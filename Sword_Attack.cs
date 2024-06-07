using UnityEngine;
using System.Collections;

public class Sword_Attack : MonoBehaviour
{
    private int m_nSwordPower;

    // Use this for initialization
    void Start()
    {
        m_nSwordPower = 1;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Enemy")
        {
            coll.gameObject.SendMessage("Sword_hit", m_nSwordPower);
        }
        else if (coll.gameObject.tag == "Food_Jar")
        {
            coll.gameObject.SendMessage("Food_Jar_hit");
        }
        else if (coll.gameObject.tag == "Wooden_Crate")
        {
            coll.gameObject.SendMessage("Wooden_Crate_hit");
        }
    }
}