using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    private Player player;

    public Image reverseCooldownSlider;
    public Image[] batteryFill;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        //set the reverseCooldownSlider:
        if (player != null && reverseCooldownSlider != null)
        {
            //set image fill amount to player's reverse cooldown value:
            reverseCooldownSlider.fillAmount = player.GetReverseCooldownValue();
        } 
        //based on the proportion from player.GetCurrentCharge() and player.GetBatteryCapacity():
        if (player != null && batteryFill != null)
        {
            float chargeProportion = player.GetCurrentCharge() / player.GetBatteryCapacity();
            int totalBatteries = batteryFill.Length;
            float batteriesToFill = chargeProportion * totalBatteries;

            for (int i = 0; i < totalBatteries; i++)
            {
                if (batteriesToFill >= i + 1)
                {
                    //full battery
                    batteryFill[i].fillAmount = 1f;
                }
                else if (batteriesToFill > i && batteriesToFill < i + 1)
                {
                    //partially filled battery
                    batteryFill[i].fillAmount = batteriesToFill - i;
                }
                else
                {
                    //empty battery
                    batteryFill[i].fillAmount = 0f;
                }
            }
        }
    }
}
