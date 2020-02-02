using Juniper.Input;

using UnityEngine;

using Yarrow;

namespace Juniper.Animation
{
    [DisallowMultipleComponent]
    public class PresentationTransition : MonoBehaviour
    {
        [ReadOnly]
        public Vector3 startPosition;

        [ReadOnly]
        public Quaternion startRotation;

        public Material startMaterial;

        [ReadOnly]
        public Material endMaterial;

        private MasterSceneController master;

        /// <summary>
        /// The method to use to pre-compute the transition value.
        /// </summary>
        public TweenType tween;

        /// <summary>
        /// The constant value to provide to the tweening function. Currently, only the Bump tween
        /// function uses this value.
        /// </summary>
        [Tooltip("The constant value to provide to the tweening function. Currently, only the Bump tween function uses this value.")]
        public float tweenK;

        private float value = 0;

        /// <summary>
        /// The amount of time it takes to complete the transition.
        /// </summary>
        public float length = 0.25f;

        private Direction state = Direction.Stopped;

        private Vector3 EndPosition
        {
            get
            {
                if (master is null
                    || master.presentationPoint is null)
                {
                    return startPosition;
                }
                else
                {
                    return master.presentationPoint.position;
                }
            }
        }

        private Quaternion EndRotation
        {
            get
            {
                if (master is null
                    || master.presentationPoint is null)
                {
                    return startRotation;
                }
                else
                {
                    return master.presentationPoint.rotation;
                }
            }
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            var renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                endMaterial = renderer.GetMaterial();
            }
        }
#endif

        private Transform CreateChild(Material mat)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
            obj.Remove<MeshCollider>();
            var rend = obj.GetComponent<MeshRenderer>();
            rend.SetMaterial(mat);
            obj.transform.SetParent(transform, false);
            obj.transform.localScale = transform.localScale;
            return obj.transform;
        }

        public void Awake()
        {
            Find.Any(out master);

            var startChild = CreateChild(startMaterial);
            startChild.transform.localRotation = Quaternion.Euler(0, 180, 0);
            CreateChild(endMaterial);

            startMaterial.SetFloat("_Alpha", 1);
            endMaterial.SetFloat("_Alpha", 0);

            transform.localScale = Vector3.one;

            this.Remove<MeshRenderer>();
        }

        public void Toggle()
        {
            if (state == Direction.Forward)
            {
                state = Direction.Reverse;
            }
            else
            {
                if(value == 0)
                {
                    startPosition = transform.position;
                    startRotation = transform.rotation;
                }
                state = Direction.Forward;
            }
        }

        public void Update()
        {
            if (state == Direction.Forward
                && value < 1)
            {
                value += Time.smoothDeltaTime;
                if (value > 1)
                {
                    value = 1;
                }
            }
            else if (state == Direction.Reverse
                && value > 0)
            {
                value -= Time.smoothDeltaTime;
                if (value < 0)
                {
                    value = 0;
                }
            }

            var tweenFunc = Tween.Functions[tween];
            var tweenValue = tweenFunc(value, tweenK, state);
            RenderValue(tweenValue);
        }

        private void RenderValue(float value)
        {
            if (state != Direction.Stopped)
            {
                transform.rotation = Quaternion.Slerp(startRotation, EndRotation, value);
                transform.position = Vector3.Lerp(startPosition, EndPosition, value);
                startMaterial.SetFloat("_Alpha", 1 - value);
                endMaterial.SetFloat("_Alpha", value);
            }
        }
    }
}
