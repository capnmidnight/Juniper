using Juniper.Input;

using UnityEngine;

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

        private MeshRenderer startRend;
        private MeshRenderer endRend;
        private MaterialPropertyBlock startProps;
        private MaterialPropertyBlock endProps;

        private Vector3 EndPosition
        {
            get
            {
                return master.systemUserInterface.position;
            }
        }

        private Quaternion EndRotation
        {
            get
            {
                return master.systemUserInterface.rotation;
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

        private MeshRenderer CreateChild(Material mat, out MaterialPropertyBlock props)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
            obj.Remove<MeshCollider>();
            var rend = obj.GetComponent<MeshRenderer>();
            rend.SetMaterial(mat);
            obj.transform.SetParent(transform, false);
            props = new MaterialPropertyBlock();
            rend.SetPropertyBlock(props);
            return rend;
        }



        public void Awake()
        {
            Find.Any(out master);

            startMaterial = Instantiate(startMaterial);


            startRend = CreateChild(startMaterial, out startProps);
            startRend.transform.localRotation = Quaternion.Euler(0, 180, 0);
            endRend = CreateChild(endMaterial, out endProps);

            SetAlpha(0);

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
                if (value == 0)
                {
                    startPosition = transform.position;
                    startRotation = transform.rotation;
                }
                state = Direction.Forward;
            }
        }

        public void Update()
        {
            var nextState = state;
            if (state == Direction.Forward
                && value < 1)
            {
                value += Time.smoothDeltaTime / length;
                if (value > 1)
                {
                    value = 1;
                }
            }
            else if (state == Direction.Reverse
                && value > 0)
            {
                value -= Time.smoothDeltaTime / length;
                if (value < 0)
                {
                    value = 0;
                    nextState = Direction.Stopped;
                }
            }

            var tweenFunc = Tween.Functions[tween];
            var tweenValue = tweenFunc(value, tweenK, state);
            RenderValue(tweenValue);
            state = nextState;
        }

        private void RenderValue(float value)
        {
            if (state != Direction.Stopped)
            {
                transform.rotation = Quaternion.SlerpUnclamped(startRotation, EndRotation, value);
                transform.position = Vector3.LerpUnclamped(startPosition, EndPosition, value);
                SetAlpha(value);
            }
        }

        private void SetAlpha(float value)
        {
            endProps.SetFloat("_Alpha", value);
            endRend.SetPropertyBlock(endProps);
            startProps.SetFloat("_Alpha", 1 - value);
            startRend.SetPropertyBlock(startProps);
        }
    }
}
