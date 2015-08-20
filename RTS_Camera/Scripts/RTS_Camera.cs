using UnityEngine;
using System.Collections;

namespace RTS_Cam
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("RTS Camera")]
    public class RTS_Camera : MonoBehaviour
    {

        #region Foldouts

#if UNITY_EDITOR

        public bool movementSettingsFoldout;
        public bool zoomingSettingsFoldout;
        public bool rotationSettingsFoldout;
        public bool heightSettingsFoldout;
        public bool mapLimitSettingsFoldout;
        public bool targetingSettingsFoldout;
        public bool inputSettingsFoldout;

#endif

        #endregion

        private Transform m_Transform; //camera tranform
        public bool useFixedUpdate = false; //use FixedUpdate() or Update()

        #region Movement

        public float keyboardMovementSpeed = 5f; //speed with keyboard movement
        public float screenEdgeMovementSpeed = 3f; //spee with screen edge movement
        public float followingSpeed = 5f; //speed when following a target
        public float rotationSped = 3f; 

        #endregion

        #region Height

        public LayerMask groundMask = -1; //layermask of ground or other objects that affects height

        public float maxHeight = 10f; //maximal height
        public float minHeight = 15f; //minimnal height
        public float zoomSpeed = 5f; //zooming speed

        private float zoomPos = 0; //value in range (0, 1) used as t in Matf.Lerp

        #endregion

        #region MapLimits

        public float limitX = 50f; //x limit of map
        public float limitY = 50f; //z limit of map

        #endregion

        #region Targeting

        public Transform targetFollow; //target to follow

        /// <summary>
        /// are we following target
        /// </summary>
        public bool FollowingTarget
        {
            get
            {
                return targetFollow != null;
            }
        }

        #endregion

        #region Input

        public bool useScreenEdgeInput = true;
        public float screenEdgeBorder = 25f;
        public bool useKeyboardInput = true;
        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";
        public bool useKeyboardZooming = true;
        public KeyCode zoomInKey = KeyCode.E;
        public KeyCode zoomOutKey = KeyCode.Q;
        public string zoomingAxis = "Mouse ScrollWheel";
        public float zoomSensitivity = 2f;
        public bool useKeyboardRotation = true;
        public KeyCode rotateRightKey = KeyCode.X;
        public KeyCode rotateLeftKey = KeyCode.Z;
        public KeyCode mouseRotationKEy = KeyCode.Mouse2;

        private Vector2 KeyboardInput
        {
            get
            {
                return useKeyboardInput ? new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis)) : Vector2.zero;
            }
        }

        private Vector2 MouseInput
        {
            get
            {
                return Input.mousePosition;
            }
        }

        private float ScrollWheel
        {
            get
            {
                return Input.GetAxis(zoomingAxis);
            }
        }

        private int ZoomDirection
        {
            get
            {
                bool zoomIn = Input.GetKey(zoomInKey);
                bool zoomOut = Input.GetKey(zoomOutKey);
                if (zoomIn && zoomOut)
                    return 0;
                else if (!zoomIn && zoomOut)
                    return 1;
                else if (zoomIn && !zoomOut)
                    return -1;
                else 
                    return 0;
            }
        }

        private int RotationDirection
        {
            get
            {
                bool rotateRight = Input.GetKey(rotateRightKey);
                bool rotateLeft = Input.GetKey(rotateLeftKey);
                if(rotateLeft && rotateRight)
                    return 0;
                else if(rotateLeft && !rotateRight)
                    return -1;
                else if(!rotateLeft && rotateRight)
                    return 1;
                else 
                    return 0;
            }
        }

        #endregion

        #region Unity_Methods

        private void Start()
        {
            m_Transform = transform;
        }

        private void Update()
        {
            if (!useFixedUpdate)
                CameraUpdate();
        }

        private void FixedUpdate()
        {
            if (useFixedUpdate)
                CameraUpdate();
        }

        #endregion

        #region RTSCamera_Methods

        /// <summary>
        /// update camera movement and rotation
        /// </summary>
        private void CameraUpdate()
        {
            if (FollowingTarget)
                FollowTarget();
            else
                Move();

            HeightCalculation();
            Rotation();
            LimitPosition();
        }

        /// <summary>
        /// move camera with keyboard or with screen edge
        /// </summary>
        private void Move()
        {
            if (useKeyboardInput)
            {
                Vector2 desiredMoveKeyboard = KeyboardInput;
                desiredMoveKeyboard *= keyboardMovementSpeed;
                desiredMoveKeyboard *= Time.deltaTime;
                m_Transform.Translate(desiredMoveKeyboard);
            }

            if (useScreenEdgeInput)
            {
                Vector2 desiredMoveMouse;

                Rect leftRect = new Rect(0, 0, screenEdgeBorder, Screen.height);
                Rect rightRect = new Rect(Screen.width - screenEdgeBorder, 0, screenEdgeBorder, Screen.height);
                Rect upRect = new Rect(0, Screen.height - screenEdgeBorder, Screen.width, screenEdgeBorder);
                Rect downRect = new Rect(0, 0, Screen.width, screenEdgeBorder);

                desiredMoveMouse.x = leftRect.Contains(MouseInput) ? -1 : rightRect.Contains(MouseInput) ? 1 : 0;
                desiredMoveMouse.y = upRect.Contains(MouseInput) ? 1 : downRect.Contains(MouseInput) ? -1 : 0;

                desiredMoveMouse *= screenEdgeMovementSpeed;
                desiredMoveMouse *= Time.deltaTime;
                m_Transform.Translate(desiredMoveMouse);
            }               
        }

        /// <summary>
        /// calcualte height
        /// </summary>
        private void HeightCalculation()
        {
            float distanceToGround = DistanceToGround();
            zoomPos += ScrollWheel * Time.deltaTime * zoomSensitivity;
            if (useKeyboardZooming)
                zoomPos += ZoomDirection * Time.deltaTime * zoomSensitivity;

            zoomPos = Mathf.Clamp01(zoomPos);

            float targetHeight = Mathf.Lerp(minHeight, maxHeight, zoomPos);
            float difference = 0; 

            if(distanceToGround != targetHeight)
                difference = targetHeight - distanceToGround;

            m_Transform.position = Vector3.Lerp(m_Transform.position, 
                new Vector3(m_Transform.position.x, targetHeight + difference, m_Transform.position.z), Time.deltaTime);
        }

        /// <summary>
        /// rotate camera
        /// </summary>
        private void Rotation()
        {
            transform.Rotate(Vector3.up, RotationDirection * Time.deltaTime * rotationSped, Space.World);
        }

        /// <summary>
        /// follow targetif target != null
        /// </summary>
        private void FollowTarget()
        {
            Vector3 targetPos = new Vector3(targetFollow.position.x, m_Transform.position.y, targetFollow.position.z);
            m_Transform.position = Vector3.MoveTowards(m_Transform.position, targetPos, Time.deltaTime * followingSpeed);
        }

        /// <summary>
        /// limit camera position
        /// </summary>
        private void LimitPosition()
        {
            m_Transform.position = new Vector3(Mathf.Clamp(m_Transform.position.x, -limitX, limitX),
                m_Transform.position.y,
                Mathf.Clamp(m_Transform.position.z, -limitY, limitY));
        }

        /// <summary>
        /// set the target
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Transform target)
        {
            targetFollow = target;
        }

        /// <summary>
        /// reset the target (target is set to null)
        /// </summary>
        public void ResetTarget()
        {
            targetFollow = null;
        }

        /// <summary>
        /// calculate distance to ground
        /// </summary>
        /// <returns></returns>
        private float DistanceToGround()
        {
            Ray ray = new Ray(m_Transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, groundMask.value))
                return (hit.point - m_Transform.position).magnitude;

            return 0f;
        }

        #endregion
    }
}