using UnityEngine;
using UnityEditor;
using System.Linq;

namespace EldwynGrove.Edit
{
    public class SpriteRenamer : EditorWindow
    {
        private string m_prefix = "CharacterBase";
        private Texture2D m_selectedTexture;

        // Animation structure: [frames, name]
        private static readonly (int frames, string name)[] s_animationStructure = new (int, string)[]
        {
            (6, "IdleDown"),
            (6, "IdleSide"),
            (6, "IdleUp"),
            (6, "WalkDown"),
            (6, "WalkSide"),
            (6, "WalkUp"),
            (4, "Attack1Down"),
            (4, "Attack2Down"),
            (4, "Attack3Down"),
            (4, "Attack1Side"),
            (4, "Attack2Side"),
            (4, "Attack3Side"),
            (4, "Attack1Up"),
            (4, "Attack2Up"),
            (4, "Attack3Up"),
            (4, "Death"),
            (6, "Climb"),
            (5, "DodgeDown"),
            (8, "DodgeSide"),
            (6, "DodgeUp"),
            (1, "HoldIdleDown"),
            (1, "HoldIdleSide"),
            (1, "HoldIdleUp"),
            (5, "HoldWalkDown"),
            (5, "HoldWalkSide"),
            (5, "HoldWalkUp"),
            (6, "JumpDown"),
            (6, "JumpSide"),
            (6, "JumpUp"),
            (6, "RangedAttackDown"),
            (6, "RangedAttackSide"),
            (6, "RangedAttackUp"),
            (6, "ToolAxeDown"),
            (6, "ToolAxeSide"),
            (6, "ToolAxeUp"),
            (6, "ToolPickaxeDown"),
            (6, "ToolPickaxeSide"),
            (6, "ToolPickaxeUp"),
            (6, "ToolHoeDown"),
            (6, "ToolHoeSide"),
            (6, "ToolHoeUp"),
            (6, "ToolWatercanDown"),
            (6, "ToolWatercanSide"),
            (6, "ToolWatercanUp"),
            (9, "FishCastDown"),
            (9, "FishCastSide"),
            (9, "FishCastUp"),
            (8, "FishReelDown"),
            (8, "FishReelSide"),
            (8, "FishReelUp"),
            (2, "MountIdleDown"),
            (2, "MountIdleSide"),
            (2, "MountIdleUp"),
            (6, "MountWalkDown"),
            (6, "MountWalkSide"),
            (6, "MountWalkUp"),
        };

        /*-------------------------------------------------------------------------
        | --- ShowWindow: Opens the Sprite Renamer window in the Unity Editor --- |
        -------------------------------------------------------------------------*/
        [MenuItem("Tools/Sprite Renamer")]
        public static void ShowWindow()
        {
            GetWindow<SpriteRenamer>("Sprite Renamer");
        }

        /*---------------------------------------------------------------------
        | --- OnGUI: Draws the GUI elements for the Sprite Renamer window --- |
        ---------------------------------------------------------------------*/
        private void OnGUI()
        {
            GUILayout.Label("Sprite Sheet Renamer", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            m_selectedTexture = (Texture2D)EditorGUILayout.ObjectField(
                "Sprite Sheet",
                m_selectedTexture,
                typeof(Texture2D),
                false
            );

            EditorGUILayout.Space();

            m_prefix = EditorGUILayout.TextField("Prefix", m_prefix);

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "Select your spritesheet texture above and enter a m_prefix (e.g., 'FarmerHat', 'FarmerShirt').\n\n" +
                "This will rename sprites to: [Prefix][AnimationName][FrameNumber]\n" +
                "Example: FarmerHatIdleDown0, FarmerHatIdleDown1, etc.\n\n" +
                "NOTE: This only works for the spritesheets in the 'Player' folder!",
                MessageType.Info
            );

            EditorGUILayout.Space();

            GUI.enabled = m_selectedTexture != null && !string.IsNullOrEmpty(m_prefix);

            if (GUILayout.Button("Rename Sprites", GUILayout.Height(30)))
            {
                RenameSprites();
            }

            GUI.enabled = true;
        }

        /*--------------------------------------------------------------------
        | --- RenameSprites: Renames sprites in the selected spritesheet --- |
        --------------------------------------------------------------------*/
        private void RenameSprites()
        {
            if (m_selectedTexture == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a sprite sheet texture.", "OK");
                return;
            }

            string path = AssetDatabase.GetAssetPath(m_selectedTexture);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer == null || importer.spritesheet == null)
            {
                EditorUtility.DisplayDialog("Error", "Could not access sprite data.", "OK");
                return;
            }

            // Get the sprite metadata
            SpriteMetaData[] spritesheet = importer.spritesheet;

            if (spritesheet.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "No sprites found in the spritesheet.", "OK");
                return;
            }

            // Sort sprites: top to bottom (higher y first), then left to right
            var sortedSprites = spritesheet.OrderByDescending(s => s.rect.y)
                                           .ThenBy(s => s.rect.x)
                                           .ToList();

            // Rename sprites based on animation structure
            int spriteIndex = 0;

            foreach (var (frames, animName) in s_animationStructure)
            {
                for (int frame = 0; frame < frames; ++frame)
                {
                    if (spriteIndex >= sortedSprites.Count)
                    {
                        Debug.LogWarning($"Ran out of sprites at {animName} frame {frame}. Expected more sprites.");
                        break;
                    }

                    string newName = $"{m_prefix}{animName}{frame}";

                    // Find this sprite in the original array and update it
                    for (int i = 0; i < spritesheet.Length; ++i)
                    {
                        if (spritesheet[i].rect == sortedSprites[spriteIndex].rect)
                        {
                            spritesheet[i].name = newName;
                            break;
                        }
                    }

                    ++spriteIndex;
                }
            }

            if (spriteIndex < sortedSprites.Count)
            {
                Debug.LogWarning($"Not all sprites were renamed. {sortedSprites.Count - spriteIndex} sprites remaining.");
            }

            // Apply the changes
            importer.spritesheet = spritesheet;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();

            EditorUtility.DisplayDialog(
                "Success",
                $"Renamed {spriteIndex} sprites with m_prefix '{m_prefix}'",
                "OK"
            );
        }
    }
};