using System.Collections.Generic;
using UnityEngine;

public class EditorStateMachine : MonoBehaviour
{
	private Stack<EditorState> states = new Stack<EditorState>();

	public LevelVisualizer levelVisualizer;

	public Level level;

	public LevelStyleDB styleDB;

	public LevelThemeDB themeDB;

	public DebugGUI debugGUI;

	public bool debugTextOnCharacters = true;

	public bool enemiesCanSee = true;

	public bool showDebugGUI = true;

	public bool useVirtualJoystickInput;

	private GameplayEngine gameplayEngine;

	private static EditorStateMachine instance;

	public static EditorStateMachine Instance
	{
		get
		{
			return instance;
		}
	}

	public EditorState CurrentState
	{
		get
		{
			return (states.Count != 0) ? states.Peek() : null;
		}
	}

	public GameplayEngine GameplayEngine
	{
		get
		{
			return gameplayEngine;
		}
		set
		{
			gameplayEngine = value;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	public void SetState(EditorState newState)
	{
		if (CurrentState != null)
		{
			CurrentState.End();
			CurrentState.enabled = false;
		}
		foreach (EditorState state in states)
		{
			state.End();
			state.enabled = false;
		}
		states.Clear();
		states.Push(newState);
		newState.Begin();
		newState.enabled = true;
	}

	public void PushState(EditorState newState)
	{
		if (CurrentState != null)
		{
			CurrentState.Pause();
			CurrentState.enabled = false;
		}
		states.Push(newState);
		newState.Begin();
		newState.enabled = true;
	}

	public void PopState()
	{
		if (states.Count < 2)
		{
			Debug.LogError("PopState called, but not enough states on stack to pop");
			return;
		}
		EditorState editorState = states.Pop();
		editorState.End();
		editorState.enabled = false;
		CurrentState.Resume();
		CurrentState.enabled = true;
	}

	public void FixedUpdate()
	{
		if (CurrentState != null)
		{
			CurrentState.Update();
		}
	}
}
