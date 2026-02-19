using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using EldwynGrove.Input;

namespace EldwynGrove.Navigation
{
    public enum MovementDirection
    {
        kUp,
        kDown,
        kLeft,
        kRight
    };

    public class MovementComponent : EntityComponent
    {
        [Header("Movement Settings")]
        [SerializeField] private float m_moveSpeed = 3.0f;
        [SerializeField] private Vector2 m_gridSize = new Vector2(1f, 1f);
        [SerializeField] private ObstacleTilemap m_obstacleTilemap;
        [SerializeField] private TileSelection m_tileSelection;

        private Vector2 m_targetPosition;
        private bool m_isMoving;
        private MovementDirection m_currentDirection;
        private MovementDirection m_lastDirection;

        private EGInputActions m_inputActions;

        public float MoveSpeed => m_moveSpeed;

        private const string kMoveX = "MoveX";
        private const string kMoveY = "MoveY";
        private const string kIsWalking = "IsWalking";

        protected override void Awake()
        {
            base.Awake();
            m_currentDirection = MovementDirection.kDown;
            m_inputActions = new EGInputActions();
        }

        private void OnEnable()
        {
            m_inputActions.Enable();
            m_inputActions.Gameplay.Click.performed += OnClick;
        }

        private void OnDisable()
        {
            m_inputActions.Gameplay.Click.performed -= OnClick;
            m_inputActions.Disable();
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            if (m_isMoving) return;

            m_targetPosition = m_tileSelection.GetHighlightedTilePosition();
            Vector2Int clickedTile = GridUtils.WorldToGrid(m_targetPosition);

            if (m_obstacleTilemap.IsTileObstacle(clickedTile))
            {
                Vector2Int? nearestWalkableTile = FindNearestWalkableTile(clickedTile);

                if (nearestWalkableTile.HasValue)
                {
                    m_targetPosition = GridUtils.GridToWorld(nearestWalkableTile.Value) + m_gridSize / 2;
                    FindPathToTargetPosition();
                }
            }
            else
            {
                if (m_targetPosition != Vector2.zero)
                {
                    FindPathToTargetPosition();
                }
            }

            m_lastDirection = m_currentDirection;
        }

        private void Update()
        {
            if (m_isMoving)
            {
                MoveTowardsTarget();
            }
        }

        private void FindPathToTargetPosition()
        {
            Vector2 startPosition = GridUtils.GridToWorld(GridUtils.WorldToGrid(transform.position));
            List<Vector2> path = AStar.FindPath(startPosition, m_targetPosition, m_gridSize, m_obstacleTilemap.IsTileObstacle);

            if (path != null && path.Count > 0)
            {
                StartCoroutine(MoveAlongPath(path));
            }
        }

        private IEnumerator MoveAlongPath(List<Vector2> path)
        {
            m_isMoving = true;
            int currentWaypointIndex = 0;

            while (currentWaypointIndex < path.Count)
            {
                m_targetPosition = path[currentWaypointIndex] + m_gridSize / 2;

                while ((Vector2)transform.position != m_targetPosition)
                {
                    float step = m_moveSpeed * Time.fixedDeltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, step);

                    yield return new WaitForFixedUpdate();
                }

                ++currentWaypointIndex;
            }

            m_isMoving = false;
        }

        private void MoveTowardsTarget()
        {
            Vector2 direction = (m_targetPosition - (Vector2)transform.position).normalized;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                    m_currentDirection = MovementDirection.kRight;
                else
                    m_currentDirection = MovementDirection.kLeft;
            }
            else
            {
                if (direction.y > 0)
                    m_currentDirection = MovementDirection.kUp;
                else
                    m_currentDirection = MovementDirection.kDown;
            }
        }

        private Vector2Int? FindNearestWalkableTile(Vector2Int startTile)
        {
            Queue<Vector2Int> queue = new();
            HashSet<Vector2Int> visited = new();

            queue.Enqueue(startTile);
            visited.Add(startTile);

            while (queue.Count > 0)
            {
                Vector2Int currentTile = queue.Dequeue();

                if (!m_obstacleTilemap.IsTileObstacle(currentTile))
                {
                    return currentTile;
                }

                foreach (Vector2Int neighbor in GetAdjacentTiles(currentTile))
                {
                    if (!visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }

            return null;
        }

        private List<Vector2Int> GetAdjacentTiles(Vector2Int position)
        {
            return new List<Vector2Int>
            {
                position + Vector2Int.up,
                position + Vector2Int.down,
                position + Vector2Int.left,
                position + Vector2Int.right
            };
        }
    }
}