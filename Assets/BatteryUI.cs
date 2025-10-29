using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    private Player player;

    public Image reverseCooldownSlider;
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
            Debug.Log("KEN GOIN UP");
            //set image fill amount to player's reverse cooldown value:
            reverseCooldownSlider.fillAmount = player.GetReverseCooldownValue();
        } 
    }
}
