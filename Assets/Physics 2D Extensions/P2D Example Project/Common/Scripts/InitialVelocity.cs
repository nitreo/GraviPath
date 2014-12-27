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
    [AddComponentMenu("Physics 2D Examples/Initial Velocity")]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class InitialVelocity : MonoBehaviour
    {
        [SerializeField] Vector2 m_Velocity;
        [SerializeField] float m_AngularVelocity;

        void Start()
        {
            rigidbody2D.velocity = m_Velocity;
            rigidbody2D.angularVelocity = m_AngularVelocity;
        }
    }
}
