/*-------------------------
File: VisionCone.cs
Author: Chandler Mays
-------------------------*/
using System;
using System.Collections.Generic;
using UnityEngine;
//---------------------------------
using EldwynGrove.Core;

namespace EldwynGrove.Components
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class VisionCone : MonoBehaviour
    {
        [SerializeField] private float m_range = 2f;
        [SerializeField][Range(10f, 170f)] private float m_angleDegrees = 90f;
        [SerializeField] private int m_rayCount = 8;

        private PolygonCollider2D m_collider;
        private readonly HashSet<IRaycastable> m_interactablesInRange = new();

        public event Action<IRaycastable> OnInteractableEntered;
        public event Action<IRaycastable> OnInteractableExited;

        public bool HasInteractable => m_interactablesInRange.Count > 0;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            m_collider = GetComponent<PolygonCollider2D>();
            m_collider.isTrigger = true;
            BuildConeShape();
        }

        /*-----------------------------------------------------------------------
        | --- SetDirection: Rotates the cone to face the given direction --- |
        -----------------------------------------------------------------------*/
        public void SetDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.001f)
                return;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        /*---------------------------------------------------------------------------
        | --- GetClosestInteractable: Returns the nearest interactable in range --- |
        ---------------------------------------------------------------------------*/
        public IRaycastable GetClosestInteractable()
        {
            IRaycastable closest = null;
            float bestDist = float.MaxValue;

            foreach (IRaycastable interactable in m_interactablesInRange)
            {
                // IRaycastable won't always be a MonoBehaviour but AIDialogueHandler is,
                // so we cast to get the world position for distance comparison
                if (interactable is not MonoBehaviour mb)
                    continue;

                float dist = Vector3.Distance(transform.position, mb.transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    closest = interactable;
                }
            }

            return closest;
        }

        /*---------------------------------------------------------------------------
        | --- OnTriggerEnter2D: Called when a collider enters the vision cone --- |
        ---------------------------------------------------------------------------*/
        private void OnTriggerEnter2D(Collider2D other)
        {
            IRaycastable[] raycastables = other.GetComponents<IRaycastable>();
            foreach (IRaycastable raycastable in raycastables)
            {
                if (m_interactablesInRange.Add(raycastable))
                {
                    OnInteractableEntered?.Invoke(raycastable);
                }
            }
        }

        /*--------------------------------------------------------------------------
        | --- OnTriggerExit2D: Called when a collider exits the vision cone --- |
        --------------------------------------------------------------------------*/
        private void OnTriggerExit2D(Collider2D other)
        {
            IRaycastable[] raycastables = other.GetComponents<IRaycastable>();
            foreach (IRaycastable raycastable in raycastables)
            {
                if (m_interactablesInRange.Remove(raycastable))
                {
                    OnInteractableExited?.Invoke(raycastable);
                }
            }
        }

        /*-----------------------------------------------------------------------
        | --- BuildConeShape: Constructs the cone polygon from range/angle --- |
        -----------------------------------------------------------------------*/
        private void BuildConeShape()
        {
            Vector2[] points = new Vector2[m_rayCount + 2];
            points[0] = Vector2.zero;

            float halfAngle = m_angleDegrees / 2f;

            for (int i = 0; i <= m_rayCount; i++)
            {
                float t = (float)i / m_rayCount;
                float angle = Mathf.Lerp(-halfAngle, halfAngle, t) * Mathf.Deg2Rad;
                points[i + 1] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * m_range;
            }

            m_collider.SetPath(0, points);
        }
    }
}