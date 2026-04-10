/*------------------------------------------------------------------------------------------------------------------------------
| Eldwyn Grove: Dialogue System
| Author: Chandler Mays
|------------------------------
| Hello! This is the README file for how to create a Dialogue asset. Thanks for opening this up and taking the time to read it.
| I hope it helps you understand how to create a Dialogue asset and what it is used for!
-------------------------------------------------------------------------------------------------------------------------------*/
########################
/*----------------------
| --- KEY CONCEPTS --- |
----------------------*/
-- 1.1 Dialogue Asset --

A Dialogue is a ScriptableObject that holds all the nodes for one conversation. You can have as many Dialogue assets
as you need -- one per NPC, one per quest stage, etc. The asset itself stores the node list and their positions in the editor canvas.

-- 1.2 Dialogue Nodes --

Each node represents one line (or paragraph) of dialogue. A node stores:
	* The text to be displayed
	* A list of child node IDs that define what comes next in the conversation
	* An optional 'On Enter Action' keyword fired when the player arrives at this node.
	* An optional 'On Exit Action' keyword fired when the player leaves this node.
	* An optional 'Condition' that must be satisfied for this node to be reachable (i.e. HasItem, DoesNotHaveQuest, CompletedObjective, etc.)
		-> See the list of predicates in IConditionChecker.cs for more details on what conditions you can use.

-- 1.3 Root Node --

The root node is the first node the player sees when a dialogue starts.
The engine determines the root automatically: any node that is not a child of any other node is considered a potential root.
If you have multiple disconnected starting nodes, the first one whose 'Condition' passes will be used.
	* This lets you vary the opening line based on game state!

-- 1.4 Branching & Choice --

If a node has more than one valid child, the engine currently selects one at random. This is designed to add variety to repeated NPC interactions.
Deliberate player-facing choice menus are planned for a future update.

-- 1.5 Conditions --

A 'Condition' is a logical gate on a node. If the condition fails, that node is skipped entirely when the engine is looking for the next valid child.
Conditions are built from 'Predicate(s)' combined with AND/OR logic:
	* All top-level AND groups must pass (conjunction)
	* Inside each AND group, at least one Predicate must pass (disjunction)
	* Any individual Predicate can be negated with the Negate checkbox.

-- 1.6 Actions (On Enter/Exit) --

Action keywords let nodes trigger in-scene events. When a node is entered or exited, the engine looks for a DialogueEventTrigger component on the NPC
and calls Trigger(keyword). You wire up what happens (unlock a quest, complete an objective, obtain an item, etc.) in the Inspector using UnityEvents.
	* Keep action keywords short and descriptive, i.e. "StartQuestX", "CompleteQuestY", etc.

#####################################
/*-----------------------------------
| --- CREATING A DIALOGUE ASSET --- |
-----------------------------------*/
Step 1 -- Create the Asset

	* In the Project window, navigate to the folder where you keep dialogue assets (i.e. Assets/Game/Resources/Dialogues)
	* Right-click -> Create -> Eldwyn Grove -> Dialogue
	* Name the asset something descriptive (i.e. "MaplebrookMayor", "QuestX_Stage2", etc.)
	(A blank root node is created automatically the first time the asset is enabled. You will see it when you open the editor.)

Step 2 -- Open the Dialogue Editor

	* Double-click the Dialogue asset in the Project window. The Dialogue Editor window opens automatically.
	* Alternatively, go to Window -> Dialogue Editor, then single-click the asset to select it.
	(You can dock the Dialogue Editor window anywhere in the Unity layout just like any other editor panel.)

Step 3 -- Navigate the Canvas

	* Scroll with the mouse wheel to pan vertically.
	* Hold the middle mouse button and drag to pan freely in any direction.
	* Nodes can be dragged individually with the left mouse button.

Step 4 -- Write Node Text

	* Click on a node to select it. The text area inside the node is an editable text field.
	* Click inside the field and type the NPC's line of dialogue.
	* Press Tab or click elsewhere to confirm. Changes are recorded for Undo (Ctrl+Z).

Step 5 -- Add More Nodes

	* Click the + button on an existing node to create a new child node branching off from it.
	* The new node appears to the right of the parent and is automatically linked as a child.

Step 6 -- Link and Unlink Nodes

Use the Link / Child / Unlink buttons to build or edit connections between any two nodes:
	* Click 'Link' on the node you want to be the parent.
	* Click 'Child' on the node you want to be the child. The connection is drawn!
	* To remove a connection, click 'Link' on the parent node, then click 'Unlink' on the connected child.
	* Click 'Cancel' at any time to abort a link operation.
	(Bezier curves show all parent -> child connections. Curves flow left-to-right, left side of a node is the 'incoming' side, right side is the 'outgoing' side.)

Step 7 -- Delete a Node

	* Click the x button on the node you want to remove.
	* All references to that node from other nodes are cleaned up automatically.
	* Deletion is recorded for Undo (Ctrl+Z).
	(Deleting the only root node causes a new blank root to be regenerated.)

######################################
/*------------------------------------
| --- ADDING CONDITIONS TO NODES --- |
------------------------------------*/

Conditions let you gate nodes behind game state -- quests, items, level, etc. Conditions are edited in the Inspector, not in the editor window.

-- 1.1 Opening the Inspector for a Node --

	* In the Dialogue Editor, click a node. The node becomes the active Unity Selection.
	* Look at the Unity Inspector panel -- it now shows the selected DialogueNode's properties, including the 'Condition' section.

-- 1.2 How the 'Condition' is Structures --

The 'Condition' field contains a list of AND groups. Expand it to see them. Each AND group contains one or more OR predicates. Think of it as:
	(Predicate A OR Predicate B) AND (Predicate C OR Predicate D)

Add AND groups by expanding the array. Add OR predicates inside each AND group with the 'Add Condition' button.

-- 1.3 Predicate Reference --

Each predicate has a 'Type' dropdown, one or two parameter fields, and a Negate checkbox. Select the predicate type that matches what you want to check:
	* kHasQuest				--			  Quest Name				--		Player currently has the quest active			--		i.e. Has quest 'The Maple Harvest'
	* kDoesNotHaveQuest		--			  Quest Name				--		Player does not have the quest					--		i.e. Does not have quest 'The Maple Harvest'
	* kCompletedQuest       --			  Quest Name				--      Player has fully completed the quest			--      i.e. Finished 'The Maple Harvest'
	* kCompletedObjective   --      Quest Name + Objective Name     --      Specific objective inside a quest is done		--      i.e. Gathered 10 sticks
	* kHasItem              --            Item Name					--		Player has the item in their inventory			--		i.e. Has 'Iron Ore'
	* kHasItems				--       Item Name + Quantity			--		Player has at least this many of the item		--		i.e. Has 5 'Iron Ore'
	* kHasItemEquipped      -- 		      Item Name					--		Player currently has the item equipped			--		i.e. Has 'Iron Sword' equipped
	* kHasLevel 			--           Level Number				--		Player's current level is at least this			--		i.e. Player is level 10+

	(The Negate checkbox inverts the result. For example, kHasItem with Negate = true means 'player does NOT have this item'.)

-- 1.4 Filtering Items by Quest --

When using kHasItem, kHasItems, or kHasItemEquipped, a Filter by Quest dropdown appears above the item list. This narrows the item picker to only QuestItems associated with that quest,
making it easier to find the right item in large projects. Choosing 'Any' shows all items.

######################################
/*------------------------------------
| --- ON ENTER / ON EXIT ACTIONS --- |
------------------------------------*/

Actions let a dialogue node trigger events in the scene when it is reached or left. They are purely keyword-based -- you type a string, and the
engine fires it against whatever DialogueEventTrigger components exist on the NPC.

-- 1.1 Setting an Action on a Node --

	* Click the node in the Dialogue Editor to select it.
	* In the Inspector, find the 'On Enter Action' and 'On Exit Action' fields.
	* Type a keyword (i.e. Start Quest: The Maple Harvest)
		- On Enter fires as soon as the player arrives at node.
		- On Exit fires as the player moves away from the node.

-- 1.2 Wiring up the Action on the NPC --

	* Select the NPC GameObject in the scene.
	* Add a DialogueEventTrigger component if it doesn't already have one.
	* In the Action Trigger Pairs list, click + to add a new pair.
	* In the Action field, type the exact same keyword you used in the node (i.e. Start Quest: The Maple Harvest)
	* Expand the On Trigger event and wire up any UnityEvent target (i.e. QuestAssigner.AssignQuest(...), assigning the quest provided in the QuestAssigner parameter)
		- IMPORTANT: An NPC can only assign one quest, as the QuestAssigner only takes in one Quest asset parameter.

##########################################
/*----------------------------------------
| --- ATTACHING A DIALOGUE TO AN NPC --- |
----------------------------------------*/

1. Select the NPC GameObject in the scene.
2. Find (or add) the AIDialogueHandler component on it.
3. Drag the Dialogue asset into the 'Dialogue' field in the Inspector.
4. Fill in the 'Speaker Name' field -- this is the name displayed above the dialogue text in the UI (i.e. "Mayor of Maplebrook")