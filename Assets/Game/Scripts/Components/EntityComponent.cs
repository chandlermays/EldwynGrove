/*-------------------------
File: EntityComponent.cs
Author: Chandler Mays
-------------------------*/
using UnityEngine;

namespace EldwynGrove.Components
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class EntityComponent : MonoBehaviour
    {
        protected GameObject Owner { get; private set; }
        protected Transform Transform { get; private set; }
        protected Animator Animator { get; private set; }
        protected Rigidbody2D Rigidbody2D { get; private set; }
        protected BoxCollider2D BoxCollider2D { get; private set; }

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        protected virtual void Awake()
        {
            Owner = gameObject;
            Transform = transform;
            Animator = GetComponent<Animator>();
            Rigidbody2D = GetComponent<Rigidbody2D>();
            BoxCollider2D = GetComponent<BoxCollider2D>();
        }
    }
}