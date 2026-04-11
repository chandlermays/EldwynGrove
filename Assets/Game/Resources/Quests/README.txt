/*------------------------------------------------------------------------------------------------------------------------------
| Eldwyn Grove: Quest System
| Author: Chandler Mays
|------------------------------
| Hello! This is the README file for how to create a Quest asset. Thanks for opening this up and taking the time to read it.
| I hope it helps you understand how to create a Quest asset and what it is used for!
-------------------------------------------------------------------------------------------------------------------------------*/
########################
/*----------------------
| --- KEY CONCEPTS --- |
----------------------*/
-- 1.1 Quest Asset --

A Quest is a ScriptableObject that holds an ordered list of QuestObjectives and an optional list of Rewards. A quest is considered complete the moment
every one of its objectives has been marked complete. Once complete, it moves from the active quest list to a permanent completd set and its rewards
are granted automatically.

-- 1.2 QuestObjective --

A QuestObjective is a ScriptableObject with a single description field. Objectives have no logic of their own -- they are simply named milestones that
the QuestManager tracks completion for. You link objectives to a quest inside the Quest asset's Objectives lit, and reference them from components like
ObjectiveCompletion.
	* The asset name (not the description) is what the save system uses to identify an objective.
	* Choose clear, unique asset names -- i.e. "Talk to the Blacksmith", "Collect Iron Ore", depending on the scope of the project.

-- 1.3 QuestManager -- 

The QuestManager lives on the Player GameObject and is the single source of truth for all quest state. It maintains the active quest dictionary, the completed
quest set, a pending notification queue, and handles saving and loading. All quest progress should be driven through QuestManager -- never modify QuestStatus
directly from design-facing components.

-- 1.4 QuestStatus --

QuestStatus is a runtime wrapper created by QuestManager for each active quest. It records which objectives have been compleed. You never create or serialize
QuestStatus manually -- QuestManager manages its lifecycle entirely.

-- 1.5 Notification Queue --

When a quest is started or completed, QuestManager does not fire the OnQuestStared or OnQuestCompleted events immediately. It enqueues them and waits for
FlushCompletionNotifs() to be called. This lets you control exactly when the 'Quest Started' and 'Quest Completed' banners appear -- i.e. after a dialogue sequence is finished.

-- 1.6 Rewards --

Each Quest has a list of Reward entries. Each reward is an InventoryItem reference plus a quantity. Rewards are granted automatically when the quest completes.
CurrencyItem rewards go straight to the player's Wallet; all other item types go to the Inventory (or are dropped if the inventory is full).

##################################
/*--------------------------------
| --- CREATING A QUEST ASSET --- |
--------------------------------*/

Step 1 -- Create the Asset

	* In the Project window, navigate to the folder where you keep quest assets (i.e. Assets/Game/Resources/Quests)
	* Right-click -> Create -> Eldwyn Grove -> Quests -> Quest
	* Name the asset. The asset name becomes the quest's Title shown in the UI -- "The Maple Harvest", "Crafty and Resourceful", etc.

Step 2 -- Add Objectives

	* Select the Quest asset in the Project window.
	* In the Inspector, find the Objectives list and click the '+' button to add a new entry.
	* Drag or assign your QuestObjective assets into each slot.
	* Order matters for display in the Journal UI -- put objectives in the sequence you expect the player to complete them!

Step 3 -- Add Rewards

	* In the Inspector, find the Rewards list and click the '+' button to add a new entry.
	* Assign an InventoryItem asset to the Item field and set the quantity (minimum value is 1).
	* Repeat for each reward you want to grant on completion.

############################################
/*------------------------------------------
| --- CREATING A QUEST OBJECTIVE ASSET --- |
------------------------------------------*/

QuestObjectives must be created before a Quest can reference them.

	* In the Project window, navigate to the folder where you keep quest objective assets (i.e. Assets/Game/Resources/Quests/Objectives)
	* Right-click -> Create -> Eldwyn Grove -> Quests -> Quest Objective
	* Name the asset. The asset name is what the save system uses to identify the objective -- "Talk to the Blacksmith", "Collect Iron Ore", etc.
	* Select the asset and in the Inspector, write a clear description of the objective. This is what shows in the Journal UI when the player views the quest details.

##########################################
/*----------------------------------------
| --- ASSIGNING QUESTS TO THE PLAYER --- |
----------------------------------------*/

-- 1.1 QuestAssigner + DialogueEventTrigger --

	* Select the NPC or trigger GameObject that should give the quest.
	* Add a QuestAssigner component.
	* Drag the player's QuestManager into the 'Quest Manager' field.
	* Drag the Quest asset you want to assign into the 'Quest' field.
	* Call QuestAssigner.AssignQuest() from a UnityEvent, a DialogueEventTrigger action, or any other event source.
	
	(The most common pattern is to wire AssignQuest() to a DialogueEventTrigger on the NPC
	using an On Exit action keyword on the dialogue node where the NPC gives the quest)

-- 1.2 Guards & Duplicate Prevention --

QuestManager.AddQuest() silently rejects the call if the quest is already active or already completed. It is safe to wire AssignQuest() to
events that fire multiple times -- the player will only receive the quest once.

#################################
/*-------------------------------
| --- COMPLETING OBJECTIVES --- |
-------------------------------*/

There are currently three ways to complete an objective. All of them ultimately call QuestManager.CompleteObjective(quest, objective).
The method you choose depends on what triggers the completion in your scene:

-- 1.1 Trigger Zone --

Use this for location-based objectives: reaching a location, entering a dungeon, or stepping into a marked zone.

	* Create an empty GameObject in the scene and position it at the target location.
	* Add a Collider component (Box, Sphere, or Capsule) and check 'Is Trigger'.
	* Add a TriggerObjective component, which auto-requires a Collider.
	* Add an ObjectiveCompletion component to the same GameObject.
	* In the ObjectiveCompletion Inspector, assign the player's QuestManager, select the Quest, then select the Objective from the filtered dropdown.

-- 1.2 ObjectiveCompletion (UnityEvent or Dialogue) --

Use this for objectives completed by interaction: talking to an NPC, handling over an item, using an object, etc.

	* Add an ObjectiveCompletion component to the relevant NPC or interactable object.
	* Assign QuestManager, Quest, and Objective in the Inspector.
	* Call ObjectiveCompletion.CompleteObjective() from any UnityEvent -- a button click, an animation event, a DialogueEventTrigger On Exit action, etc.

-- 1.3 Multiple Objectives on One GameObject --

A single NPC or trigger zone can complete more than one objective. Add multiple ObjectiveCompletion components -- one per objective -- and call each one's
CompleteObjective() from the appropriate event. Unity allows multiple instances of the same component type on a single GameObject.