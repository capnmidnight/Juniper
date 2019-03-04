using UnityEngine;

namespace Juniper.Animation
{
    /// <summary>
    /// A flipbook-style animation. There's probably a more Unity-ish way of doing this, but I was
    /// pressed for time.
    /// </summary>
    public class Flipbook : MonoBehaviour
    {
        /// <summary>
        /// The time, in seconds, to wait between flipping pages of the flipbook.
        /// </summary>
        [Range(0.01f, 1f)]
        public float timeout;

        /// <summary>
        /// The images to flip through in the flipbook.
        /// </summary>
        public Sprite[] images;

        /// <summary>
        /// Get the renderer for the flipbook images.
        /// </summary>
        public void Awake()
        {
            img = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Update the lastT so the first frame has a full amount of time on screen.
        /// </summary>
        public void OnEnable()
        {
            lastT = Time.time - timeout;
        }

        /// <summary>
        /// If the timeout has passed, update the current frame of the flipbook.
        /// </summary>
        public void Update()
        {
            if (images != null && images.Length > 0 && Time.time > lastT + timeout)
            {
                currentIndex = (currentIndex + 1) % images.Length;
                lastT = Time.time;

                img.sprite = images[currentIndex];
            }
        }

        /// <summary>
        /// The current image we should be viewing.
        /// </summary>
        private int currentIndex = -1;

        /// <summary>
        /// The renderer for all of the images.
        /// </summary>
        private SpriteRenderer img;

        /// <summary>
        /// The timestamp, in seconds, since the last image was flipped.
        /// </summary>
        private float lastT;
    }
}