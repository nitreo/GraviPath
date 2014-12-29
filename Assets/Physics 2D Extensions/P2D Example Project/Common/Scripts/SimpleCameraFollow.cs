//
// SimpleCameraFollow.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2014 Thinksquirrel Software, LLC
//
using UnityEngine;

namespace Thinksquirrel.Phys2DExamples
{
    [AddComponentMenu("Physics 2D Examples/Simple Camera Follow")]
    public sealed class SimpleCameraFollow : MonoBehaviour
    {
        [SerializeField] Transform m_Target;
        [SerializeField] Vector2 m_Offset = new Vector2(0, 3);
        [SerializeField] float m_CameraSpeed = 4;
        Transform m_CachedTransform;
        public bool VelocityBasedSize;
        public float MinimumSize;
        public float SizeVelocityFactor;
        public float SizeChangeSpeed;
        private Camera m_CachedCamera;
        private Rigidbody2D m_CachedTargetRigidbody;

        void Awake()
        {
            m_CachedCamera = GetComponent<Camera>();
            m_CachedTransform = transform;
            m_CachedTargetRigidbody = m_Target.GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (!m_Target)
                return;

            Vector3 pos = m_CachedTransform.position;
            Vector3 targetPos = m_Target.position;
            m_CachedTransform.position = Vector3.Lerp(pos, new Vector3(targetPos.x + m_Offset.x, targetPos.y + m_Offset.y, pos.z), Time.deltaTime * m_CameraSpeed);
            m_CachedCamera.orthographicSize = Mathf.Lerp(m_CachedCamera.orthographicSize, MinimumSize + m_CachedTargetRigidbody.velocity.magnitude * SizeVelocityFactor, Time.deltaTime * SizeChangeSpeed);


        }
    }
}
