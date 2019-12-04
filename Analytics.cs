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
				Debug.LogError(
				  "Firebase initialized!!!! ====================================================");
			}
			else
			{
				Debug.LogError(
				  "Could not resolve all Firebase dependencies: " + dependencyStatus);
			}
		});
	}

	void InitializeFirebase()
	{
		DebugLog("Enabling data collection.");
		FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

		DebugLog("Set user properties.");
		// Set the user's sign up method.
		FirebaseAnalytics.SetUserProperty(
		  FirebaseAnalytics.UserPropertySignUpMethod,
		  "Google");
		// Set the user ID.
		FirebaseAnalytics.SetUserId("uber_user_510");
		// Set default session duration values.
		FirebaseAnalytics.SetMinimumSessionDuration(new TimeSpan(0, 0, 10));
		FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));
		firebaseInitialized = true;

		Debug.LogError("Firebase Hell yeahhhh -======================================---------- ");

		Firebase.Analytics.FirebaseAnalytics
.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin);

		// Log an event with a float parameter
		Firebase.Analytics.FirebaseAnalytics
		  .LogEvent("progress", "percent", 0.4f);

		// Log an event with an int parameter.
		Firebase.Analytics.FirebaseAnalytics
		  .LogEvent(
			Firebase.Analytics.FirebaseAnalytics.EventPostScore,
			Firebase.Analytics.FirebaseAnalytics.ParameterScore,
			42
		  );

		// Log an event with a string parameter.
		Firebase.Analytics.FirebaseAnalytics
		  .LogEvent(
			Firebase.Analytics.FirebaseAnalytics.EventJoinGroup,
			Firebase.Analytics.FirebaseAnalytics.ParameterGroupId,
			"spoon_welders"
		  );

		// Log an event with multiple parameters, passed as a struct:
		Firebase.Analytics.Parameter[] LevelUpParameters = {
  new Firebase.Analytics.Parameter(
	Firebase.Analytics.FirebaseAnalytics.ParameterLevel, 5),
  new Firebase.Analytics.Parameter(
	Firebase.Analytics.FirebaseAnalytics.ParameterCharacter, "mrspoon"),
  new Firebase.Analytics.Parameter(
	"hit_accuracy", 3.14f)
};
		Firebase.Analytics.FirebaseAnalytics.LogEvent(
		  Firebase.Analytics.FirebaseAnalytics.EventLevelUp,
		  LevelUpParameters);
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
