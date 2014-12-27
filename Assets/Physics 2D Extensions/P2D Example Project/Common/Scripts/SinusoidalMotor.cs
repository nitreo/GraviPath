//
// SinusoidalMotor.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2014 Thinksquirrel Software, LLC
//
using UnityEngine;
using System.Collections;
using Thinksquirrel.Phys2D;

namespace Thinksquirre.Phys2DExamples
{
    [AddComponentMenu("Physics 2D Examples/Sinusoidal Motor")]
    public sealed class SinusoidalMotor : MonoBehaviour
    {
        [SerializeField] HingeOrSliderJoint2DExt m_Joint;
        [SerializeField] float m_MotorSpeed = 0.5f;
        [SerializeField] float m_TimeScale = 1.0f;

        void FixedUpdate()
        {
            var hinge = m_Joint as HingeJoint2DExt;
            var slider = m_Joint as SliderJoint2DExt;

            if (hinge)
            {
                var motor = hinge.motor;
                motor.motorSpeed = m_MotorSpeed * Mathf.Sin(Time.time * m_TimeScale);
                hinge.motor = motor;
            }
            else if (slider)
            {
                var motor = slider.motor;
                motor.motorSpeed = m_MotorSpeed * Mathf.Sin(Time.time * m_TimeScale);
                slider.motor = motor;
            }
        }
    }
}
