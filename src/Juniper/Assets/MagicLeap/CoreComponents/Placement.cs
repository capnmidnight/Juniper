// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System;

#pragma warning disable 472

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// The type of surface that will be acceptable for placement.
    /// </summary>
    public enum SurfaceType
    {
        Unknown,
        Horizontal,
        Vertical,
        Slanted
    }

    /// <summary>
    /// The fit type that is determined upon positioning the content.
    /// </summary>
    public enum FitType
    {
        Unknown,
        Fits,
        NoSurface,
        Uneven,
        Overhang,
        VolumeIntersection,
        WrongOrientation
    }

    /// <summary>
    /// Handles the overall content validation and confirmation for placement objects.
    /// A valid fit is determined based on the desired orientation and clearance of the volume.
    /// </summary>
    public class Placement : MonoBehaviour
    {
        #region Private Variables
        [SerializeField, Tooltip("Tilts the placement's cast lower or higher than the source's forward.")]
        private float _tilt;

        [SerializeField, Tooltip("When on a wall should the volume's y axis match gravity?")]
        private bool _matchGravityOnVerticals = true;

        [SerializeField, Tooltip("How far to detect a surface.")]
        private float _maxDistance = 3.048f;

        [SerializeField, Tooltip("Beyond this value a surface will be determined to be uneven.")]
        private float _maxUneven = 0.0508f;

        private Transform _source;
        private bool _allowHorizontal;
        private bool _allowVertical;
        private System.Action<Vector3, Quaternion> _callback;

        private RaycastHit _surfaceHit;
        private FitType _fitStatus;

        private RaycastHit _cornerAHit = new RaycastHit();
        private RaycastHit _cornerBHit = new RaycastHit();
        private RaycastHit _cornerCHit = new RaycastHit();
        private RaycastHit _cornerDHit = new RaycastHit();

        private const float MIN_CAST_DISTANCE = 0.05f;
        #endregion

        #region Events
        public event Action<FitType> OnPlacementEvent;
        #endregion

        #region Public Properties
        /// <summary>
        /// The active status of the placement script.
        /// </summary>
        public bool IsRunning
        {
            get;
            private set;
        }

        /// <summary>
        /// The current fit status based on placement and collisions.
        /// </summary>
        public FitType Fit
        {
            get;
            private set;
        }

        /// <summary>
        /// The determined YAxis based on placement orientation.
        /// </summary>
        public Vector3 YAxis
        {
            get;
            private set;
        }

        /// <summary>
        /// The position of the placement object.
        /// </summary>
        public Vector3 Position
        {
            get;
            private set;
        }

        /// <summary>
        /// The rotation of the placement object.
        /// </summary>
        public Quaternion Rotation
        {
            get;
            private set;
        }

        /// <summary>
        /// The SurfaceType of the placement object.
        /// </summary>
        public SurfaceType Surface
        {
            get;
            private set;
        }

        /// <summary>
        /// The Scale of the placement object.
        /// </summary>
        public Vector3 Scale
        {
            get;
            private set;
        }

        /// <summary>
        /// If the placement object should orientate to match gravity on vertical surfaces.
        /// </summary>
        public bool MatchGravityOnVerticals
        {
            get { return _matchGravityOnVerticals; }
        }
        #endregion

        #region Unity Methods
        void Update()
        {
            if (!IsRunning)
            {
                return;
            }

            // Adjust the cast by the tilt value.
            Quaternion adjustmentQuaternion = Quaternion.AngleAxis(_tilt, _source.right);
            Vector3 castVector = adjustmentQuaternion * _source.forward;

            if (Physics.Raycast(_source.position, castVector, out _surfaceHit, _maxDistance))
            {
                // Set the initial values.
                Position = _surfaceHit.point;
                Surface = GetSurfaceType(_surfaceHit.normal);
                _fitStatus = GetFitType(Surface);

                ValidateCorners(_surfaceHit);

                ValidateVolume();
            }
            else
            {
                _fitStatus = FitType.NoSurface;
                Surface = SurfaceType.Unknown;

                Position = _source.position + (castVector * _maxDistance);
                Rotation = Quaternion.LookRotation(-_source.forward);
            }

            // Event Notification
            if (Fit != _fitStatus)
            {
                Fit = _fitStatus;
                if (OnPlacementEvent != null)
                {
                    OnPlacementEvent.Invoke(_fitStatus);
                }
            }
        }
        #endregion

        #region Public Methods
        public void Confirm()
        {
            if (IsRunning && Fit == FitType.Fits)
            {
                IsRunning = false;
                _callback(Position, Rotation);
            }
        }

        /// <summary>
        /// Begins placement session. Callback provides position and volume rotation.
        /// </summary>
        public void Place(Transform placementSource, Vector3 scale, bool allowHorizontal, bool allowVertical, System.Action<Vector3, Quaternion> callback)
        {
            if (IsRunning) return;

            Scale = scale;
            _allowHorizontal = allowHorizontal;
            _allowVertical = allowVertical;

            //cache:
            _source = placementSource;
            _callback = callback;

            //start placement loop:
            IsRunning = true;
        }

        /// <summary>
        /// Resumes placement and starts updating.
        /// </summary>
        public void Resume()
        {
            IsRunning = true;
        }

        /// <summary>
        /// Cancels placement and stops updating.
        /// </summary>
        public void Cancel()
        {
            IsRunning = false;
        }
        #endregion

        #region Private Methods
        private void ValidateCorners(RaycastHit surfaceHit)
        {
            if (_fitStatus == FitType.Fits || _fitStatus == FitType.WrongOrientation)
            {
                // Obtain a "perfect" normal since meshed surfaces are not smooth.
                YAxis = GetPerfectNormal(Surface, surfaceHit.normal);

                // Find the axis.
                Vector3 xAxis = GetCrossAxis(Surface, YAxis);
                Vector3 zAxis = Vector3.Cross(YAxis, xAxis);

                // Set the rotation.
                if (Surface == SurfaceType.Vertical && _matchGravityOnVerticals)
                {
                    Rotation = Quaternion.LookRotation(YAxis, Vector3.up);
                }
                else
                {
                    Rotation = Quaternion.LookRotation(zAxis, YAxis);
                }

                GetCornersHitPoints(surfaceHit, xAxis, zAxis, out _cornerAHit, out _cornerBHit, out _cornerCHit, out _cornerDHit);

                UpdateFitStatus(surfaceHit, xAxis, zAxis);

                // Push out if we want to align with gravity while on a wall.
                if (_matchGravityOnVerticals && Surface == SurfaceType.Vertical)
                {
                    Position += YAxis * (Scale.z * .5f);
                }
            }
        }

        private void GetCornersHitPoints(RaycastHit surfaceHit, Vector3 xAxis, Vector3 zAxis,
            out RaycastHit cornerAHit, out RaycastHit cornerBHit, out RaycastHit cornerCHit, out RaycastHit cornerDHit)
        {
            // Locate each surface-proximity corner.
            Vector3 halfVolume = Scale * .5f;
            Vector3 cornerA = Vector3.zero;
            Vector3 cornerB = Vector3.zero;
            Vector3 cornerC = Vector3.zero;
            Vector3 cornerD = Vector3.zero;

            if (_matchGravityOnVerticals && Surface == SurfaceType.Vertical)
            {
                cornerA = surfaceHit.point + (zAxis * -halfVolume.y) + (xAxis * -halfVolume.x);
                cornerB = surfaceHit.point + (zAxis * -halfVolume.y) + (xAxis * halfVolume.x);
                cornerC = surfaceHit.point + (zAxis * halfVolume.y) + (xAxis * halfVolume.x);
                cornerD = surfaceHit.point + (zAxis * halfVolume.y) + (xAxis * -halfVolume.x);

                // Find the corner-to-surface points.
                cornerAHit = PerformCornerCast(YAxis, cornerA, Scale.z);
                cornerBHit = PerformCornerCast(YAxis, cornerB, Scale.z);
                cornerCHit = PerformCornerCast(YAxis, cornerC, Scale.z);
                cornerDHit = PerformCornerCast(YAxis, cornerD, Scale.z);
            }
            else
            {
                cornerA = surfaceHit.point + (zAxis * halfVolume.z) + (xAxis * halfVolume.x);
                cornerB = surfaceHit.point + (zAxis * halfVolume.z) + (xAxis * -halfVolume.x);
                cornerC = surfaceHit.point + (zAxis * -halfVolume.z) + (xAxis * -halfVolume.x);
                cornerD = surfaceHit.point + (zAxis * -halfVolume.z) + (xAxis * halfVolume.x);

                // Find the corner-to-surface points.
                cornerAHit = PerformCornerCast(YAxis, cornerA, Scale.y);
                cornerBHit = PerformCornerCast(YAxis, cornerB, Scale.y);
                cornerCHit = PerformCornerCast(YAxis, cornerC, Scale.y);
                cornerDHit = PerformCornerCast(YAxis, cornerD, Scale.y);
            }
        }

        private void UpdateFitStatus(RaycastHit surfaceHit, Vector3 xAxis, Vector3 zAxis)
        {
            // All corners have hit something.
            if (_cornerAHit.collider != null && _cornerBHit.collider != null && _cornerCHit.collider != null && _cornerDHit.collider != null)
            {
                // Obtain the even-ness values.
                float cornerAEvenness = CheckForEvenSurface(surfaceHit.point, _cornerAHit.point, xAxis, zAxis);
                float cornerBEvenness = CheckForEvenSurface(surfaceHit.point, _cornerBHit.point, xAxis, zAxis);
                float cornerCEvenness = CheckForEvenSurface(surfaceHit.point, _cornerCHit.point, xAxis, zAxis);
                float cornerDEvenness = CheckForEvenSurface(surfaceHit.point, _cornerDHit.point, xAxis, zAxis);

                // Are we within the maxUneven threshold?
                float largestBump = Mathf.Max(cornerAEvenness, cornerBEvenness, cornerCEvenness, cornerDEvenness);

                // Determine if we passed even-ness testing.
                if (largestBump > _maxUneven)
                {
                    _fitStatus = FitType.Uneven;
                }
                else
                {
                    // Only set as fits if we are in the correct orientation.
                    if (_fitStatus != FitType.WrongOrientation)
                    {
                        _fitStatus = FitType.Fits;
                        Position = surfaceHit.point + (YAxis * largestBump);
                    }
                }
            }
            else
            {
                // We are likely hanging over a physical edge.
                _fitStatus = FitType.Overhang;
            }
        }

        private void ValidateVolume()
        {
            if (_fitStatus == FitType.Fits)
            {
                // Locate the center of the volume.
                Vector3 centerPoint = Position + (YAxis * (Scale.y * .5f));

                // Determine if the volume is smaller than our actual volume by uneven allowance as a buffer.
                Vector3 collisionVolumeSize = Scale * (.5f - _maxUneven);

                // Check for any volume collisions.
                Collider[] volumeCollisions = Physics.OverlapBox(centerPoint, collisionVolumeSize, Rotation);

                if (volumeCollisions.Length > 0)
                {
                    _fitStatus = FitType.VolumeIntersection;
                }
                else
                {
                    _fitStatus = FitType.Fits;
                }
            }
        }

        private float CheckForEvenSurface(Vector3 rootPoint, Vector3 cornerPoint, Vector3 normalA, Vector3 normalB)
        {
            Vector3 gapVector = rootPoint - cornerPoint;

            // Collapse vector:
            gapVector = Vector3.ProjectOnPlane(gapVector, normalA);

            // Collapse vector again:
            gapVector = Vector3.ProjectOnPlane(gapVector, normalB);

            return gapVector.magnitude;
        }

        private RaycastHit PerformCornerCast(Vector3 normal, Vector3 corner, float volumeAxis)
        {
            RaycastHit hit;
            // Translate corner to the top of our volume to cast "down".
            corner = corner + (normal * volumeAxis);

            // Over cast to ensure we can hit a surface.
            float castDistance = volumeAxis * 1.5f;

            // Fix cast distance for objects that are flat or very thin.
            if (castDistance < MIN_CAST_DISTANCE)
            {
                castDistance = MIN_CAST_DISTANCE;
            }

            Physics.Raycast(corner, -normal, out hit, castDistance);

            return hit;
        }

        private Vector3 GetCrossAxis(SurfaceType surfaceType, Vector3 normal)
        {
            if (surfaceType == SurfaceType.Horizontal)
            {
                if (normal == Vector3.up)
                {
                    // table or floor
                    return Vector3.Cross(normal, _source.forward).normalized;
                }
                else
                {
                    // ceiling
                    return Vector3.Cross(normal, _source.forward).normalized;
                }
            }

            if (surfaceType == SurfaceType.Vertical)
            {
                return Vector3.Cross(normal, Vector3.up);
            }

            return Vector3.zero;
        }

        private Vector3 GetPerfectNormal(SurfaceType surfaceType, Vector3 normal)
        {
            if (surfaceType == SurfaceType.Horizontal)
            {
                // Collapse the normal to be straight.
                Vector3 phase1 = Vector3.ProjectOnPlane(normal, Vector3.right).normalized;
                Vector3 phase2 = Vector3.ProjectOnPlane(phase1, Vector3.forward).normalized;

                return phase2;
            }

            if (surfaceType == SurfaceType.Vertical)
            {
                return Vector3.ProjectOnPlane(normal, Vector3.up).normalized;
            }

            return Vector3.zero;
        }

        private FitType GetFitType(SurfaceType surfaceType)
        {
            if (surfaceType == SurfaceType.Slanted)
            {
                return FitType.Uneven;
            }

            if (surfaceType == SurfaceType.Horizontal && !_allowHorizontal)
            {
                return FitType.WrongOrientation;
            }

            if (surfaceType == SurfaceType.Vertical && !_allowVertical)
            {
                return FitType.WrongOrientation;
            }

            if (surfaceType == SurfaceType.Horizontal && _allowHorizontal)
            {
                return FitType.Fits;
            }

            if (surfaceType == SurfaceType.Vertical && _allowVertical)
            {
                return FitType.Fits;
            }

            return FitType.Unknown;
        }

        private SurfaceType GetSurfaceType(Vector3 surfaceNormal)
        {
            float dot = Vector3.Dot(surfaceNormal, Vector3.up);

            // Determine the surface orientation.
            if (dot >= .97f || dot <= -.97f)
            {
                return SurfaceType.Horizontal;
            }

            if (dot >= -.3f && dot <= .3f)
            {
                return SurfaceType.Vertical;
            }

            return SurfaceType.Slanted;
        }
        #endregion
    }
}
