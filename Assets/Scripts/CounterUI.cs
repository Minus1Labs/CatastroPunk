using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class CounterUI : MonoBehaviour
{
    // reference to player:
    public string CounterName = "Ammo";

    public Slider slider;
    public Player player;
    public bool displayMax = false;
    public int maxValue = 0;

    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        //if player is null, find it via tag "Player":
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.GetComponent<Player>();
                slider.maxValue = player.GetBatteryCapacity();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //update text to show current ammo count:
        if (text != null && player != null)
        {
            float value = player.GetCurrentCharge();
            text.text = value.ToString().Truncate(4, "") + " / " + player.GetBatteryCapacity();
            slider.value = value;
        }
    }
}
