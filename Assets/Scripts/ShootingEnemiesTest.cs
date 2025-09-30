using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemiesTest : MonoBehaviour
{
    public GameObject projectilePrefab, player;

    public float delayBetweenShots = 2f;

    private float dt = 0;
     

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        dt += Time.deltaTime;

        if(dt >= delayBetweenShots) {
            dt = 0;
            Vector2 from = transform.position;
            Vector2 dir = player.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float angle2 = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 45f;
            float angle3 = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 45f;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
            Quaternion rot2 = Quaternion.AngleAxis(angle2, Vector3.forward);
            Quaternion rot3 = Quaternion.AngleAxis(angle3, Vector3.forward);

            GameObject go = Instantiate(projectilePrefab, from, rot);
            GameObject go2 = Instantiate(projectilePrefab, from, rot2);
            GameObject go3 = Instantiate(projectilePrefab, from, rot3);
        }

    }
}
