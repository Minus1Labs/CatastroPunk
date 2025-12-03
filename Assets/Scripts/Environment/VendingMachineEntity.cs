using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

/*
A vending machine is a prop with "bouncy" collision resembling a vending machine.
It has a clearly defined screen cycling through the current offered items. The price of the item
is decided by the time in the run, and enemies drop cash according to their difficulty, run time,
the player's cashMultipier, and any other factors.

When the player "bounces" into the vending machine and has the necessary cash, we:
- subtract the cash required
- subtract from the uses of the vending machine
- take the current displayed item and give it to the player
- play animations of the vending machine dispensing the item
- update the UI items and cash

*/
public class VendingMachineEntity : MonoBehaviour
{
    [Header("Vending Machine Options")]
    [Range(1, 5)] public int options = 2;
    [Range(0.1f, 3f)] public float seconds_per_option = 1.0f;
    [Range(0, 5)] public int uses_remaining = 1;
    public int cost = 15;
    

    [Header("Required components")]
    public SpriteRenderer VendingMachineSprite;
    public SpriteRenderer DisplayedCard;
    [SerializeField] private CardDatabase cardDb;
    [SerializeField] private Collider2D triggerArea;
    [SerializeField] private Rigidbody2D rb;
    [Tooltip("Don't assign")] public List<Card> heldCards;

    /*[Header("Sounds")]
    public string idleNoise = "";
    public string purchaseNoise = "";
    public string noMoneyNoise = "";*/

    public AudioSource vendingSFX;

    private bool instantiated = false;
    private float timeLastDisplayed = 0.0f;
    private int displayedIndex = -1;

    private float lastInteractTime = 0f;
    private float interactLockoutTime = 1.5f;


    public GameObject FloppyDisk;
    Animator floppyDiskAnimator;
    public GameObject eatCardPosition;

    public bool classicVendingMachine = false;


    public void Awake()
    {
        heldCards = cardDb.GetTBCard(options);
        if (heldCards != null)
        {
            instantiated = true;
        }
        floppyDiskAnimator = FloppyDisk.GetComponent<Animator>();
    }

    public void FixedUpdate()
    {
        if (instantiated && uses_remaining > 0)
        {
            if (Time.time - timeLastDisplayed > seconds_per_option)
            {
                timeLastDisplayed = Time.time;
                ChangeDisplayed();
            }
        }
    }

    private void ChangeDisplayed()
    {
        displayedIndex++;
        if (displayedIndex >= heldCards.Count)
        {
            displayedIndex = 0;
        }

        DisplayedCard.sprite = heldCards[displayedIndex].UISprite;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (uses_remaining > 0 && other.CompareTag("Player"))
        {
            if (Time.time - lastInteractTime > interactLockoutTime)
            {
                Player p = other.GetComponent<Player>() ?? other.GetComponentInChildren<Player>();
                if (p == null)
                {
                    Debug.LogError("Tagged object Player doesn't have player component?");
                    return;
                }
                if(classicVendingMachine)
                {
                    //do the original way where u get a choice
                    CardUIManager cardUIManager = FindObjectOfType<CardUIManager>();
                    cardUIManager.EnableCardUI();
                    
                } else 
                {
                    GiveCard(p);
                }
                lastInteractTime = Time.time;
            }
        }
    }

    private void GiveCard(Player p)
    {
        if (p.cash >= cost)
        {
            p.cash -= cost;
            p.AddCard(heldCards[displayedIndex]);
            uses_remaining--;
            Debug.Log("Gave player the card: " + heldCards[displayedIndex].cardName);


            //TODO: play purchase animation and noise
            Vector3 cardPosition = eatCardPosition.GetComponent<Transform>().position;
            p.transform.localPosition = cardPosition;
            vendingSFX.Play();
            FloppyDisk.SetActive(true);
            floppyDiskAnimator.SetTrigger("eatCard");


            if (uses_remaining > 0)
            {
                heldCards.RemoveRange(displayedIndex, 1);
            }
        }
        else
        {
            Debug.Log("No money to buy: " + heldCards[displayedIndex].cardName);
            //TODO: play denial animation and noise
        }
    }
    
    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //bounce player away
            other.attachedRigidbody.AddRelativeForce(other.attachedRigidbody.velocity * -1.5f);
        }
    }
}