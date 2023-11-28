using System.Collections;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public bool isOpen = false;
    public bool isOpening = false;

    //public AudioClip openSound;
    //public AudioClip closeSound;
    //private AudioSource audioSource;

    private Quaternion originalRotation;
    private Quaternion openRotation;

    public Quaternion OriginalRotation
    {
        get { return originalRotation; }
    }

    public Quaternion OpenRotation
    {
        get { return openRotation; }
    }

    private float rotationDuration = 1.0f;

    void Start()
    {
        originalRotation = transform.rotation;
        openRotation = originalRotation * Quaternion.Euler(0, 100, 0);
        //audioSource = GetComponent<AudioSource>();
        //if (audioSource == null)
        //{
        //    audioSource = gameObject.AddComponent<AudioSource>();
        //}
    }

    public IEnumerator RotateDoor(Quaternion from, Quaternion to)
    {
        float elapsed = 0f;
        isOpening = true;
        while (elapsed < rotationDuration)
        {
            //if (!isOpen)
            //{
            //    audioSource.PlayOneShot(openSound);
            //}
            //else
            //{
            //    audioSource.PlayOneShot(closeSound);
            //}
            float percentage = Mathf.Clamp01(elapsed / rotationDuration);
            transform.rotation = Quaternion.Lerp(from, to, percentage);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = to;
        isOpen = !isOpen;
        isOpening = false;
    }
}
