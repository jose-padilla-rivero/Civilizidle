using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;

namespace IdleClickerKit {
 
	/// <summary>
	/// Handles the clicks.
	/// </summary>
	public class ClickManager : Manager <ClickManager> {

		[Tooltip ("What we call the main click attribute")]
		[SerializeField]
		[ContextMenuItem ("Add 1000 Clicks", "Add1000Clicks")]
		[ContextMenuItem ("Add 100000 Clicks", "Add100000Clicks")]
		protected string clickName = "Clicks";

		[Tooltip ("If true this is the main click type the game is about.")]
		[SerializeField]
		protected bool isMainClick = true;

		protected static Dictionary<string, ClickManager> clickManagers;

		/// <summary>
		/// The clicks. This is persisted.
		/// </summary>
		protected long clicks;

		/// <summary>
		/// The clicks. This is persisted.
		/// </summary>
		protected long totalClicks;

		/// <summary>
		/// How much we increment clicks by each click. This is caculated on start, and each new upgrade
		/// affects this value. It is NOT persisted.
		/// </summary>
		protected int clickIncrement = 1;



		/// <summary>
		///  Firebase
		/// </summary>
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

			Debug.LogError( "Firebase Hell yeahhhh -======================================---------- ");

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

		/// <summary>
		/// If there is already a manager destroy self, else initialise and assign to the static reference.
		/// </summary>
		void Awake(){
			if (isMainClick) {
				if (manager == null) {
					if (!initialised)
						Init ();
					manager = this;
				} else if (manager != this) {
					Destroy (gameObject);	
				} else if (!initialised) {
					Init ();
				}
			}
		}

		/// <summary>
		/// Registers the new type of click manager
		/// </summary>
		/// <param name="manager">Manager.</param>
		protected virtual void RegisterNewClickManager(ClickManager manager) {
			if (clickManagers == null) clickManagers = new Dictionary<string, ClickManager> ();
			if (clickManagers.ContainsKey (clickName)) {
				Debug.LogError ("Multiple ClickManagers with the same clickName defined. Destroying self.");
				Destroy (gameObject);	
			} else {
				clickManagers.Add (manager.clickName, manager);
			}
		}

		/// <summary>
		/// Gets the Instance the specified clickName.
		/// </summary>
		/// <param name="clickName">Click name.</param>
		public static ClickManager GetInstance(string clickName) {
			if (clickName == null || clickName == "") return Instance;
			if (clickManagers == null) clickManagers = new Dictionary<string, ClickManager> ();
			if (clickManagers.ContainsKey(clickName)) {
				return clickManagers [clickName];	
			}
			// Couldn't find it, lets search
			ClickManager[] cms = FindObjectsOfType<ClickManager> ();
			foreach (ClickManager cm in cms) {
				cm.Init ();
			}
			if (clickManagers.ContainsKey(clickName)) {
				return clickManagers [clickName];	
			}
			Debug.LogWarning(string.Format("ClickManagers with the clickName '{0}' could not be found. Returning default.", clickName));
			return Instance;
		}

		/// <summary>
		/// Determines whether this instance is valid manager to be returned by a call to Instance.
		/// </summary>
		/// <returns><c>true</c> if this instance is valid manager; otherwise, <c>false</c>.</returns>
		override protected bool IsValidManager {
			get {
				return isMainClick;
			}
		}

		/// <summary>
		/// Get the name of the clicks.
		/// </summary>Create
		/// <value>The name of the click.</value>
		virtual public string ClickName {
			get { return clickName; }
		}

		/// <summary>
		/// Publicly accessible clicks.
		/// </summary>
		/// <value>The clicks.</value>
		virtual public long Clicks {
			get  { return clicks; }
		}

		/// <summary>
		/// Publicly accessible total clicks.
		/// </summary>
		/// <value>The total clicks.</value>
		virtual public long TotalClicks {
			get  { return totalClicks; }
		}

		/// <summary>
		/// Publicly accessible click increment.
		/// </summary>
		/// <value>The clicks.</value>
		virtual public int ClickIncrement {
			get  { return clickIncrement; }
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void Init() {
			if (!initialised) {
				initialised = true;
				clicks = 0;
				totalClicks = 0;
				Load (this);
				RegisterNewClickManager (this);
			}
		}

		/// <summary>
		/// Add clicks.
		/// </summary>
		/// <param name="amount">Amount.</param>
		public void AddClicks(int amount) {
			clicks += amount;
			totalClicks +=  amount;
			Save (this);
		}

		public bool Purchase(int amount) {
			if (clicks >= amount) { 
				clicks -= amount;
				Save (this);
				return true;
			}
			return false;
		}

		public void IncreaseClickIncrement(int amount) {
			clickIncrement += amount;
		}


#region Persistable

		/// <summary>
		/// Gets the unique save key.
		/// </summary>
		override public string UniqueSaveKey {
			get {
				return "Data_ClickManager_" + clickName;
			}
		}

		/// <summary>
		/// Gets the save data.
		/// </summary>
		/// <value>The save data.</value>
		override public object SaveData {
			get {
				return new object[] {clicks, totalClicks};
			}
			set {
				if (value.GetType () == SavedObjectType) {
					clicks = (long)((object[])value) [0];
					totalClicks = (long)((object[])value) [0];
				}
			}
		}

		/// <summary>
		/// Get the type of object this Persistable saves.
		/// </summary>
		override public System.Type SavedObjectType {
			get {
				return typeof(object[]);
			}
		}

		/// <summary>
		/// Things to do after reset.
		/// </summary>
		override public void PostResetAction() {
			clicks = 0;
			totalClicks = 0;
		}

		/// <summary>
		/// Adds 1,000 clicks. Used for the context menu.
		/// </summary>
		protected void Add1000Clicks() {
			AddClicks (1000);
		}

		/// <summary>
		/// Adds 100,000 clicks. Used for the context menu.
		/// </summary>
		protected void Add100000Clicks() {
			AddClicks (100000);
		}

#endregion
	}
}

