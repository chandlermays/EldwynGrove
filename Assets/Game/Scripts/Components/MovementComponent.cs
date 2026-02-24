using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//---------------------------------
using EldwynGrove.Core;

namespace EldwynGrove.Navigation
{
    public class MovementComponent : EntityComponent
    {
        [SerializeField] private float m_moveSpeed = 5f;

        private static readonly int s_animMoveX = Animator.StringToHash("MoveX");
        private static readonly int s_animMoveY = Animator.StringToHash("MoveY");
        private static readonly int s_animIsMoving = Animator.StringToHash("IsMoving");

        private Vector2 m_lastDirection = Vector2.down;
        private Coroutine m_moveCoroutine;

        public bool IsMoving { get; private set; }

        protected override void Awake()
        {
            base.Awake();
        }

        public void MoveTo(Vector3 targetPos, Action onComplete = null)
        {
            List<Vector3> path = Pathfinder.FindPath(Transform.position, targetPos);
            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("No path found to target position.");
                return;
            }

            if (m_moveCoroutine != null)
            {
                StopCoroutine(m_moveCoroutine);
            }
            m_moveCoroutine = StartCoroutine(MoveAlongPath(path, onComplete));
        }

        public void Stop()
        {
            if (m_moveCoroutine != null)
            {
                StopCoroutine(m_moveCoroutine);
                m_moveCoroutine = null;
            }
            SetMoving(false, Vector2.zero);
        }

        private IEnumerator MoveAlongPath(List<Vector3> path, Action onComplete)
        {
            SetMoving(true, Vector2.zero);

            foreach (Vector3 waypoint in path)
            {
                Vector3 target = new Vector3(waypoint.x, waypoint.y, Transform.position.z);
                Vector2 direction = (target - Transform.position).normalized;

                m_lastDirection = direction;
                SetMoving(true, direction);

                while (Vector3.Distance(Transform.position, target) > 0.01f)
                {
                    Transform.position = Vector3.MoveTowards(Transform.position, target, m_moveSpeed * Time.deltaTime);
                    yield return null;
                }

                Transform.position = target;
            }

            SetMoving(false, m_lastDirection);
            m_moveCoroutine = null;

            onComplete?.Invoke();
        }

        private void SetMoving(bool isMoving, Vector2 direction)
        {
            IsMoving = isMoving;
            Animator.SetBool(s_animIsMoving, isMoving);
            Animator.SetFloat(s_animMoveX, direction.x);
            Animator.SetFloat(s_animMoveY, direction.y);
        }
    }
}