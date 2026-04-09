/*-------------------------
File: Portal.cs
Author: Chandler Mays
-------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//---------------------------------
using EldwynGrove.Components;
using EldwynGrove.Player;
using EldwynGrove.Saving;

namespace EldwynGrove.SceneManagement
{
    public enum PortalID
    {
        A,
        B,
        C,
        D,
        E
    }

    public class Portal : MonoBehaviour
    {
        [Header("Portal Settings")]
        [SerializeField] private SceneField m_sceneField;
        [SerializeField] private Transform m_spawnPoint;
        [SerializeField] private PortalID m_portalID;

        private static readonly List<Portal> s_portals = new();

        private Transform m_player;
        private const string kPlayerTag = "Player";
        private Vector2 m_playerDirection;

        /*----------------------------------------------------------------
        | --- Awake: Called when the script instance is being loaded --- |
        ----------------------------------------------------------------*/
        private void Awake()
        {
            Utilities.CheckForNull(m_sceneField, nameof(m_sceneField));
            Utilities.CheckForNull(m_spawnPoint, nameof(m_spawnPoint));
        }

        /*---------------------------------------------------------------------
        | --- OnEnable: Called when the object becomes enabled and active --- |
        ---------------------------------------------------------------------*/
        private void OnEnable()
        {
            s_portals.Add(this);
        }

        /*--------------------------------------------------------
        | --- OnDisable: Called when the object is destroyed --- |
        --------------------------------------------------------*/
        private void OnDestroy()
        {
            s_portals.Remove(this);
        }

        /*------------------------------------------------------------------------------
        | --- OnTriggerEnter: Called when the Collider 'other' enters this trigger --- |
        ------------------------------------------------------------------------------*/
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(kPlayerTag))
            {
                m_player = other.transform;

                if (m_player.TryGetComponent<MovementComponent>(out var movementComponent))
                {
     //               m_playerDirection = movementComponent.LastDirection;
                }

                StartCoroutine(SceneTransition());
            }
        }

        /*-------------------------------------------------------------
        | --- SceneTransition: Coroutine to Transition the Scenes --- |
        -------------------------------------------------------------*/
        private IEnumerator SceneTransition()
        {
            // DontDestroyOnLoad only works for root GameObjects or components on root GameObjects
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);

            PlayerController playerController = m_player.GetComponent<PlayerController>();
            playerController.enabled = false;

            yield return TransitionFade.Instance.FadeOut();

            // Checkpoint before scene transition
            SaveManager.Instance.Save();

            yield return SceneManager.LoadSceneAsync(m_sceneField.SceneName);

            // Cannot use cached player, have to find it again in the new scene
            PlayerController newPlayerController = GameObject.FindWithTag(kPlayerTag).GetComponent<PlayerController>();
            newPlayerController.enabled = false;

            SaveManager.Instance.Load();

            Portal destination = GetDestination();
            UpdatePlayer(destination);

            // Checkpoint after scene transition
            SaveManager.Instance.Save();

            yield return TransitionFade.Instance.Wait();
            yield return TransitionFade.Instance.FadeIn();

            newPlayerController.enabled = true;
            Destroy(gameObject);
        }

        /*--------------------------------------------------------------------
        | --- GetDestination: Returns the Portal the Player is headed to --- |
        --------------------------------------------------------------------*/
        private Portal GetDestination()
        {
            foreach (Portal portal in s_portals)
            {
                if (portal == this) continue;
                if (portal.m_portalID == m_portalID)
                {
                    return portal;
                }
            }
            return null;
        }

        /*-------------------------------------------------------------------------------------
        | --- UpdatePlayer: Update the Player's Position to the Destination's Spawn Point --- |
        -------------------------------------------------------------------------------------*/
        private void UpdatePlayer(Portal destination)
        {
            // Retrieve the player object again in the new scene
            m_player = GameObject.FindWithTag(kPlayerTag).transform;
            if (m_player == null)
            {
                Debug.LogError("Player object not found in the new scene!");
                return;
            }
            m_player.position = destination.m_spawnPoint.position;

            if (m_player.TryGetComponent<MovementComponent>(out var movementComponent))
            {
             //   movementComponent.SetDirection(m_playerDirection);
             //   movementComponent.Stop();
            }
        }
    }
}