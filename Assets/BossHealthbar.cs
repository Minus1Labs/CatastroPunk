using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthbar : MonoBehaviour
{

    public Slider slider;

    public Enemies.Enemy bossEnemy;
    

    // Update is called once per frame
    void Update()
    {
        slider.value = bossEnemy.GetCurrentHealth() / bossEnemy.GetMaxHP();
    }
}
