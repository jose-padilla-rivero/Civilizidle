using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;

public class Analytics : MonoBehaviour
{
	DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
	protected bool firebaseInitialized = false;
	private string logText = "";
	const int kMaxLogSize = 16382;
	private Vector2 controlsScrollViewVector = Vector2.zero;
	private Vector2 scrollViewVector = Vector2.zero;

	public virtual void Start()
	{
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
			dependencyStatus = task.Result;
			if (dependencyStatus == DependencyStatus.Available)
			{
				InitializeFirebase();
				Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Debug;
				Debug.LogError(
				  "Firebase initialized!!!! ====================================================");
			}
			else
			{
				Debug.LogError(
				  "Could not resolve all Firebase dependencies: " + dependencyStatus);
				UnityEngine.Debug.LogError(System.String.Format(
					"Could not resolve all Firebase dependencies: {0}", dependencyStatus));

			}
		});
	}

	void InitializeFirebase()
	{
		DebugLog("Enabling data collection.");
		FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

		
	}

	// Output text to the debug log text field, as well as the console.
	public void DebugLog(string s)
	{
		print(s);
		logText += s + "\n";

		while (logText.Length > kMaxLogSize)
		{
			int index = logText.IndexOf("\n");
			logText = logText.Substring(index + 1);
		}

		scrollViewVector.y = int.MaxValue;
	}
}
