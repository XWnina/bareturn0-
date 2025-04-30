using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

namespace JumpGame
{
    public class NpcTeachingDialogue : MonoBehaviour
    {
        [FormerlySerializedAs("bubbleBG")] public GameObject bubbleBg;
        public TextMeshProUGUI textP;
        public GameObject rightHint;
        public GameObject runHint;
        public GameObject dragHintIf;
        public GameObject whileHint;
        public GameObject codeWorkspace;
        public GameObject ifBlockPrefab;
        public Button runButton;
        public GameObject npcDialog; // NpcDialog 总容器


        private readonly Queue<string> _dialogueQueue = new Queue<string>();
        private bool _waitingForPlayer = false;
        private string _currentPhase = "";

        void Start()
        {
            rightHint.SetActive(false);
            dragHintIf.SetActive(false);
            runHint.SetActive(false);
            bubbleBg.SetActive(true);
            whileHint.SetActive(false);

            EnqueueAllDialogues();
            StartCoroutine(ShowNextDialogue());
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) && !_waitingForPlayer)
            {
                StartCoroutine(ShowNextDialogue());
            }
        }

        void EnqueueAllDialogues()
        {
            _dialogueQueue.Enqueue("Oh, it's great to see you here. This is where I usually collect minerals.");
            _dialogueQueue.Enqueue("Watch out! There’s a big rock ahead. You’ll need to jump over it.");
            _dialogueQueue.Enqueue("[SHOW_RIGHT_HINT]");
            _dialogueQueue.Enqueue("Look, there are buttons at the bottom of the panel.");
            _dialogueQueue.Enqueue("[HIDE_RIGHT_HINT_SHOW_DRAG_HINT]");
            _dialogueQueue.Enqueue("You can drag them into the code panel above.");
            _dialogueQueue.Enqueue("Now drag an 'if' block into the code panel.");
            _dialogueQueue.Enqueue("[WAIT_FOR_IF_BLOCK]");
            _dialogueQueue.Enqueue("This is an if block. Inside, you’ll see a dropdown menu labeled 'condition'.");
            _dialogueQueue.Enqueue("Try selecting 'rock ahead'.");
            _dialogueQueue.Enqueue("[WAIT_FOR_CONDITION]");
            _dialogueQueue.Enqueue("Good. Now the action dropdown lets you choose what to do.");
            _dialogueQueue.Enqueue("Try selecting 'jump'.");
            _dialogueQueue.Enqueue("[WAIT_FOR_ACTION]");
            _dialogueQueue.Enqueue("[SHOW_RUN_HINT]");
            _dialogueQueue.Enqueue("Great! Now click the 'Run' button to execute your code.");
            _dialogueQueue.Enqueue("[WAIT_FOR_RUN]");
            
            _dialogueQueue.Enqueue("Wow, look! You did it.");
            _dialogueQueue.Enqueue("Now you see an 'else' button on the block.");
            _dialogueQueue.Enqueue("You can click it to add an else clause for when the condition is false. " +
                                   "Try selecting 'walk' as the action when there is no rock ahead.");
            _dialogueQueue.Enqueue("[WAIT_FOR_ELSE_WALK]");
            _dialogueQueue.Enqueue("Now click the 'Run' button again to test it.");
            _dialogueQueue.Enqueue("[WAIT_FOR_RUN_AGAIN]");
            
            _dialogueQueue.Enqueue("Nice! But that only walks a short distance.");
            _dialogueQueue.Enqueue("Let's try using a while block to walk continuously.");
            _dialogueQueue.Enqueue("Drag in a while block into the panel.");
            _dialogueQueue.Enqueue("[WAIT_FOR_WHILE_BLOCK]");
            _dialogueQueue.Enqueue("Set the condition to 'is grounded'.And set the action to 'walk'.");
            _dialogueQueue.Enqueue("[WAIT_FOR_WHILE_SETUP]");
            _dialogueQueue.Enqueue("Now run the code and see what happens!");
            _dialogueQueue.Enqueue("[WAIT_FOR_FINAL_RUN]");
            _dialogueQueue.Enqueue("Great! Next, you can try nesting an if statement inside the while loop. You're doing an excellent job — I'll be waiting for you at the finish line. See you there!");
            _dialogueQueue.Enqueue("[FINISH_TEACHING]");

        }

        IEnumerator ShowNextDialogue()
        {
            if (_dialogueQueue.Count == 0)
            {
                textP.text = "";
                yield break;
            }

            string line = _dialogueQueue.Dequeue();

            if (line.StartsWith("["))
            {
                switch (line)
                {
                    case "[SHOW_RIGHT_HINT]":
                        rightHint.SetActive(true);
                        yield return ShowNextDialogue();
                        break;
                    case "[HIDE_RIGHT_HINT_SHOW_DRAG_HINT]":
                        rightHint.SetActive(false);
                        dragHintIf.SetActive(true);
                        yield return ShowNextDialogue();
                        break;
                    case "[WAIT_FOR_IF_BLOCK]":
                        _waitingForPlayer = true;
                        _currentPhase = "if";
                        yield return new WaitUntil(() => HasIfBlock());
                        dragHintIf.SetActive(false);
                        _waitingForPlayer = false;
                        yield return ShowNextDialogue();
                        break;
                    case "[WAIT_FOR_CONDITION]":
                        _waitingForPlayer = true;
                        _currentPhase = "condition";
                        yield return new WaitUntil(() => IsConditionSet());
                        _waitingForPlayer = false;
                        yield return ShowNextDialogue();
                        break;
                    case "[WAIT_FOR_ACTION]":
                        _waitingForPlayer = true;
                        _currentPhase = "action";
                        yield return new WaitUntil(() => IsActionSet());
                        _waitingForPlayer = false;
                        yield return ShowNextDialogue();
                        break;
                    case "[SHOW_RUN_HINT]":
                        runHint.SetActive(true);
                        yield return ShowNextDialogue();
                        break;
                    case "[WAIT_FOR_RUN]":
                        _waitingForPlayer = true;
                        _currentPhase = "run";
                        runButton.onClick.AddListener(OnRunClicked);
                        yield break;
                    case "[WAIT_FOR_ELSE_WALK]":
                        _waitingForPlayer = true;
                        _currentPhase = "else";
                        yield return new WaitUntil(() => IsElseSetToWalk());
                        _waitingForPlayer = false;
                        yield return ShowNextDialogue();
                        break;

                    case "[WAIT_FOR_RUN_AGAIN]":
                        _waitingForPlayer = true;
                        _currentPhase = "run2";
                        runButton.onClick.AddListener(OnRunClicked);
                        yield break;

                    case "[WAIT_FOR_WHILE_BLOCK]":
                        whileHint.SetActive(true);
                        _waitingForPlayer = true;
                        _currentPhase = "while";
                        yield return new WaitUntil(() => HasWhileBlock());
                        _waitingForPlayer = false;
                        whileHint.SetActive(false);
                        yield return ShowNextDialogue();
                        break;

                    case "[WAIT_FOR_WHILE_SETUP]":
                        _waitingForPlayer = true;
                        yield return new WaitUntil(() => IsWhileSetupCorrect());
                        _waitingForPlayer = false;
                        yield return ShowNextDialogue();
                        break;

                    case "[WAIT_FOR_FINAL_RUN]":
                        _waitingForPlayer = true;
                        _currentPhase = "run3";
                        runButton.onClick.AddListener(OnRunClicked);
                        yield break;
                    
                    case "[FINISH_TEACHING]":
                        rightHint.SetActive(false);
                        dragHintIf.SetActive(false);
                        runHint.SetActive(false);
                        bubbleBg.SetActive(false);
                        whileHint.SetActive(false); 
                        npcDialog.SetActive(false);
                        break;

                }
            }
            else
            {
                textP.text = line;
            }
        }

        void OnRunClicked()
        {
            if (_currentPhase == "run" || _currentPhase == "run2" || _currentPhase == "run3")
            {
                runButton.onClick.RemoveListener(OnRunClicked);
                _waitingForPlayer = false;
                runHint.SetActive(false);
                StartCoroutine(ShowNextDialogue());
            }
        }


        bool HasIfBlock()
        {
            foreach (Transform child in codeWorkspace.transform)
            {
                if (child.name.Contains("IfBlock")) return true;
            }
            return false;
        }

        bool IsConditionSet()
        {
            foreach (Transform child in codeWorkspace.transform)
            {
                var headerRow = child.Find("HeaderRow");
                if (headerRow != null)
                {
                    var dropdown = headerRow.GetComponentInChildren<TMP_Dropdown>();
                    if (dropdown != null && dropdown.options[dropdown.value].text == "rock ahead")
                        return true;
                }
            }
            return false;
        }

        bool IsActionSet()
        {
            foreach (Transform child in codeWorkspace.transform)
            {
                var trueContainer = child.Find("TrueContainer");
                if (trueContainer != null)
                {
                    var dropdown = trueContainer.GetComponentInChildren<TMP_Dropdown>();
                    if (dropdown != null && dropdown.options[dropdown.value].text == "jump")
                        return true;
                }
            }
            return false;
        }
        bool HasWhileBlock()
        {
            foreach (Transform child in codeWorkspace.transform)
                if (child.name.Contains("WhileBlock")) return true;
            return false;
        }

        bool IsWhileSetupCorrect()
        {
            foreach (Transform child in codeWorkspace.transform)
            {
                if (child.name.Contains("WhileBlock"))
                {
                    var headerRow = child.Find("HeaderRow");
                    var body = child.Find("TrueContainer");
                    if (headerRow != null && body != null)
                    {
                        var condition = headerRow.GetComponentInChildren<TMP_Dropdown>();
                        var action = body.GetComponentInChildren<TMP_Dropdown>();
                        if (condition != null && condition.options[condition.value].text == "is grounded" &&
                            action != null && action.options[action.value].text == "walk")
                            return true;
                    }
                }
            }
            return false;
        }
        bool IsElseSetToWalk()
        {
            foreach (Transform child in codeWorkspace.transform)
            {
                var elseDropdown = child.Find("ElseContainer")?.GetComponentInChildren<TMP_Dropdown>();
                if (elseDropdown != null && elseDropdown.options[elseDropdown.value].text == "walk")
                    return true;
            }
            return false;
        }
        private Coroutine _hideCoroutine;
        public void TriggerObstacleFeedback()
        {
            npcDialog.SetActive(true);
            bubbleBg.SetActive(true);
            textP.text = "Hmm... It seems you hit the rock. Try jumping over it!";
    
            if (_hideCoroutine != null)
                StopCoroutine(_hideCoroutine);
    
            _hideCoroutine = StartCoroutine(HideNpcDialogAfterDelay(5f));
        }
        IEnumerator HideNpcDialogAfterDelay(float seconds)
        {
            yield return new WaitForSeconds(seconds);
    
            if (!_waitingForPlayer) {
                npcDialog.SetActive(false);
                bubbleBg.SetActive(false);
            }
        }
        
        public void OnPlayerReachedFlag()
        {
            if (_currentPhase == "run3" && _waitingForPlayer)
            {
                PlayerController player = Object.FindFirstObjectByType<PlayerController>();
                if (player != null)
                {
                    player.ForceStop();
                    player.reachedGoal = false; // ✅ 重置以便后续还能用 while
                }

                _waitingForPlayer = false;
                StartCoroutine(ShowNextDialogue());
            }
        }







    }
}
