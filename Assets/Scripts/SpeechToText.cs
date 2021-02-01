using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;

[RequireComponent(typeof(PublishInput))]
public class SpeechToText : MonoBehaviour
{
	public TextMeshProUGUI output;

	protected DictationRecognizer dictationRecognizer;

	
	private float lastResultTime = 0f;
	public float holdTime = 1f;
	private bool needToClear = false;

	PublishInput emitter;

	void Start()
	{
		emitter = GetComponent<PublishInput>();
		StartDictationEngine();
	}

	private void DictationRecognizer_OnDictationHypothesis(string text)
	{
		//Debug.Log("Dictation hypothesis: " + text);
		output.text = text;
	}

	private void Update()
	{
		if (needToClear && Time.time > lastResultTime + holdTime)
		{
			output.text = "";
			needToClear = false;
		}
	}

	private void DictationRecognizer_OnDictationComplete(DictationCompletionCause completionCause)
	{
		switch (completionCause)
		{
			case DictationCompletionCause.TimeoutExceeded:
			case DictationCompletionCause.PauseLimitExceeded:
			case DictationCompletionCause.Canceled:
			case DictationCompletionCause.Complete:
				// Restart required
				CloseDictationEngine();
				StartDictationEngine();
				break;
			case DictationCompletionCause.UnknownError:
			case DictationCompletionCause.AudioQualityFailure:
			case DictationCompletionCause.MicrophoneUnavailable:
			case DictationCompletionCause.NetworkFailure:
				// Error
				CloseDictationEngine();
				break;
		}
	}

	private void DictationRecognizer_OnDictationResult(string text, ConfidenceLevel confidence)
	{
		//Debug.Log("Dictation result: " + text);

		lastResultTime = Time.time;
		needToClear = true;
		//emitter.EmitString(text);
		GameManager.instance.Parse(text);
	}

	private void DictationRecognizer_OnDictationError(string error, int hresult)
	{
		Debug.Log("Dictation error: " + error);
	}

	private void OnApplicationQuit()
	{
		CloseDictationEngine();
	}

	private void StartDictationEngine()
	{
		dictationRecognizer = new DictationRecognizer();
		dictationRecognizer.DictationHypothesis += DictationRecognizer_OnDictationHypothesis;
		dictationRecognizer.DictationResult += DictationRecognizer_OnDictationResult;
		dictationRecognizer.DictationComplete += DictationRecognizer_OnDictationComplete;
		dictationRecognizer.DictationError += DictationRecognizer_OnDictationError;
		dictationRecognizer.Start();
	}

	private void CloseDictationEngine()
	{
		if (dictationRecognizer != null)
		{
			dictationRecognizer.DictationHypothesis -= DictationRecognizer_OnDictationHypothesis;
			dictationRecognizer.DictationComplete -= DictationRecognizer_OnDictationComplete;
			dictationRecognizer.DictationResult -= DictationRecognizer_OnDictationResult;
			dictationRecognizer.DictationError -= DictationRecognizer_OnDictationError;
			if (dictationRecognizer.Status == SpeechSystemStatus.Running)
			{
				dictationRecognizer.Stop();
			}
			dictationRecognizer.Dispose();
		}
	}

}