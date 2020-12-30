using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace URay
{
    public class URay_Dispatcher : MonoBehaviour
    {
        private static readonly Queue<DispatcherInfo<URay_Intersection>> executeQueue = new Queue<DispatcherInfo<URay_Intersection>>();

        public void Update()
        {
            
        }

        public static void Raycast(Vector3 origin, Vector3 direction, Action<URay_Intersection> hit)
        {
            lock(executeQueue)
            {

            }
        }

        public struct DispatcherInfo<T>
        {
            public readonly Action<T> callback;
            public readonly T callback_parameter;

            public DispatcherInfo(Action<T> callback, T parameter)
            {
                this.callback = callback;
                this.callback_parameter = parameter;
            }
        }
    }

}
