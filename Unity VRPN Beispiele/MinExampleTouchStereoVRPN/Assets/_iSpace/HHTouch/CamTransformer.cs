﻿/*
 * @author Valentin Simonov / http://va.lent.in/
 */

using System;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;

namespace TouchScript.Behaviors
{
    /// <summary>
    /// Component which transforms an object according to events from transform gestures: <see cref="TransformGesture"/>, <see cref="ScreenTransformGesture"/>, <see cref="PinnedTransformGesture"/> and others.
    /// </summary>
    public class CamTransformer : MonoBehaviour
    {
        #region Private variables

        [SerializeField] public List<GameObject> objectsToListenTo = new List<GameObject>();
        private Transform cachedTransform;
        private List<ITransformGesture> gestures = new List<ITransformGesture>();

        #endregion

        #region Unity methods

        private void Awake()
        {
            cachedTransform = transform;
        }

        private void OnEnable()
        {
            foreach( GameObject go in objectsToListenTo)
            {
                var g = go.GetComponents<Gesture>();
                for (var i = 0; i < g.Length; i++)
                {
                    var transformGesture = g[i] as ITransformGesture;
                    if (transformGesture == null) continue;

                    gestures.Add(transformGesture);
                    transformGesture.Transformed += transformHandler;
                }
            }

        }

        private void OnDisable()
        {
            for (var i = 0; i < gestures.Count; i++)
            {
                var transformGesture = gestures[i];
                transformGesture.Transformed -= transformHandler;
            }
            gestures.Clear();
        }

        #endregion

        #region Event handlers

        private void transformHandler(object sender, EventArgs e)
        {
            var gesture = sender as ITransformGesture;
            gesture.ApplyTransform(cachedTransform);            
        }

        #endregion
    }
}