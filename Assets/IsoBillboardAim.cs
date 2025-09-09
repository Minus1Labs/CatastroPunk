using UnityEngine;

/// Attach this to the CHILD (sprite) of your bullet.
/// The parent handles Z-rotation/physics; the child faces camera and spins in screen space to match travel dir.
public class IsoBillboardAim : MonoBehaviour
{
    [Tooltip("The parent transform that rotates in Z (or the rigidbody holder).")]
    public Transform parentTransform;

    [Tooltip("Optional: if your sprite's 'forward' isn't exactly to the right (+X), tweak this.")]
    public float spriteArtZOffsetDeg = 0f;

    [Tooltip("Use rigidbody velocity instead of parent.right (leave null to use parent.right).")]
    public Rigidbody2D rb2D;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        if (!parentTransform) parentTransform = transform.parent;
    }

    void LateUpdate()
    {
        if (!cam || !parentTransform) return;

        // 1) Face the camera (billboard) so the sprite lies in the screen plane.
        //    This removes the weird tilt you get from an isometric camera.
        Quaternion faceCam = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);

        // 2) Compute bullet travel direction in WORLD space.
        Vector3 dirWorld;
        if (rb2D && rb2D.velocity.sqrMagnitude > 1e-6f)
            dirWorld = rb2D.velocity.normalized;           // trust actual velocity if available
        else
            dirWorld = parentTransform.right.normalized;   // or assume you move along +X of the parent

        // 3) Project that direction onto the CAMERA PLANE (screen plane).
        Vector3 dirScreen = Vector3.ProjectOnPlane(dirWorld, cam.transform.forward);
        if (dirScreen.sqrMagnitude < 1e-6f) return;
        dirScreen.Normalize();

        // 4) Find how much to rotate around the screen's Z (camera.forward) so our sprite's +X matches dirScreen.
        //    Compare to camera.right (the screen's +X).
        float spinAroundScreenZ =
            Vector3.SignedAngle(cam.transform.right, dirScreen, cam.transform.forward);

        // 5) Final rotation: face camera, then spin within that plane, then any small art offset.
        transform.rotation = faceCam
                           * Quaternion.AngleAxis(spinAroundScreenZ + spriteArtZOffsetDeg, Vector3.forward);
    }
}
