using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class PlayerTeleporter : MonoBehaviour
{
    public Vector2 targetLocation;

    [SerializeField] private CinemachineVirtualCamera vcam; // optional drag in inspector

    public bool loadScene = false;
    public string sceneName = "BossAiTestScene";

    private void Start()
    {
        // If not assigned in inspector, grab the first vcam it finds
        if (vcam == null)
        {
            vcam = FindObjectOfType<CinemachineVirtualCamera>();
            if (vcam == null)
            {
                Debug.LogWarning("No CinemachineVirtualCamera found in scene!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player teleported!");
            if (loadScene)
            {
                SceneManager.LoadScene(sceneName);
                return; // don't do the rest of the teleport logic
            }

            Transform root = collision.transform.parent != null
                ? collision.transform.parent
                : collision.transform;

            root.position = targetLocation;

            if (vcam != null)
            {
                // simplest: reset Cinemachine so it snaps
                vcam.PreviousStateIsValid = false;

                // or, if you want the delta approach instead:
                // Vector3 delta = targetLocation - root.position;
                // vcam.OnTargetObjectWarped(root, delta);
            }
            Player p = collision.GetComponent<Player>();
            if (p != null)
            {
                p.SetCurrentAmmo(p.getAmmoPerRoom());
            }
        }
    }
}
