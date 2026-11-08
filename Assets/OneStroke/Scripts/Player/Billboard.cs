using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    private void LateUpdate()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
            return;

        // ƒJƒƒ‰‚Æ“¯‚¶Œü‚«‚É‚µ‚ÄASprite‚Ì³–Ê‚ğ•\¦‚·‚é
        transform.rotation = targetCamera.transform.rotation;
    }
}