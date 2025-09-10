using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    // reference to player:
    public Player player;
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
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //update text to show current ammo count:
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        if (text != null && player != null)
        {
            text.text = "Ammo: " + player.GetCurrentAmmoCount().ToString();
        }
    }
}
