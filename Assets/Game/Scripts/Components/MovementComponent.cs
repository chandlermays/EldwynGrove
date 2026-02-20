using UnityEngine;

namespace EldwynGrove.Navigation
{
    public class MovementComponent : EntityComponent
    {
        [SerializeField] private float m_moveSpeed = 5.0f;
    }
}