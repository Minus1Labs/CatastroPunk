using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestructibleObject : MonoBehaviour, IReversible
{
    private string objectName;
    private int timesHit;
    private ParticleSystem rubbleParticles;
    public int maxHits;


    private SpriteRenderer parentSprite;
    private SpriteRenderer destructiveSprite;

    private Collider2D parentCollider;
    public Collider2D destructiveCollider;
    public enum SpriteSize
    {
        Size_1x1,
        Size_1x2,
        Size_1x3,
        Size_2x1,
        Size_3x1
    }

    public enum SpriteType
    {
        Normal,
        Electronics
    }

    [System.Serializable]
    public struct SpriteMapping
    {
        public SpriteSize size;
        public SpriteType type;
        public Sprite sprite;
    }

    public SpriteSize mySize; 
    public SpriteType myType; 

    public List<SpriteMapping> spriteDatabase;

    private static Dictionary<(SpriteSize, SpriteType), Sprite> spriteMap;

    void Awake()
    {
        if (spriteMap == null)
        {
            spriteMap = new Dictionary<(SpriteSize, SpriteType), Sprite>();

            foreach (var mapping in spriteDatabase)
            {
                var combinedKey = (mapping.size, mapping.type);

                if (!spriteMap.ContainsKey(combinedKey))
                {
                    spriteMap.Add(combinedKey, mapping.sprite);
                }
                else
                {
                    Debug.LogWarning("Duplicate database entry for: " + mapping.size + " / " + mapping.type);
                }
            }
        }

    }

    void Start()
    {
        objectName = transform.parent.name;

        rubbleParticles = GetComponentInChildren<ParticleSystem>();

        Transform spriteColliderObj = transform.parent.Find("SpriteCollider");
        if (spriteColliderObj != null)
        {
            parentSprite = spriteColliderObj.GetComponent<SpriteRenderer>();
            parentCollider = spriteColliderObj.GetComponentInParent<Collider2D>();

        }
        else if(parentSprite == null)
        {
            parentSprite = transform.parent.GetComponentInParent<SpriteRenderer>();
            parentCollider = transform.parent.GetComponentInParent<Collider2D>();
        }
        parentSprite.enabled = true;

        destructiveSprite = GetComponent<SpriteRenderer>();
        //destructiveCollider = GetComponent<Collider2D>();

        destructiveSprite.flipX = parentSprite.flipX;
        destructiveSprite.enabled = false;
        destructiveCollider.enabled = false;



        var lookupKey = (mySize, myType);

        if (spriteMap.ContainsKey(lookupKey))
        {
            destructiveSprite.sprite = spriteMap[lookupKey];
            //Debug.Log("Assigned sprite: " + destructiveSprite.name + " based on size: " + mySize + " and type: " + myType);

        }
        else
        {
           // Debug.LogError("Sprite for " + mySize + " / " + myType + " not found in database!", this.gameObject);
        }
    }


    public void Update()
    {
        

    }
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Bullet" && timesHit < maxHits)
        {
            timesHit++;
            //play shader
            StartCoroutine(particlesPlay());

            if (timesHit >= maxHits)
            {
                //disable shader
                parentSprite.enabled = false;
                StartCoroutine(particlesPlay());
                destructiveSprite.enabled = true;
                destructiveCollider.enabled = true;


                //disable old collider and enable new one
            }
        }
    }

    public IEnumerator particlesPlay()
    {
        rubbleParticles.Play();

        yield return new WaitForSeconds(1f);
    }

    public void Reverse(int strength)
    {
        //reverse the destruction:
        if (timesHit >= maxHits)
        {
            destructiveSprite.enabled = false;
            destructiveCollider.enabled = false;
            parentSprite.enabled = true;
        }
        timesHit = Mathf.Max(0, timesHit - strength);
    }

}


