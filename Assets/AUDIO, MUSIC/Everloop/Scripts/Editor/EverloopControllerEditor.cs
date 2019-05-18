using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(EverloopController))]
public class EverloopControllerEditor : Editor {

	private AnimBool showDriftParams;
	private bool showDetailedInfo = true;
	private int numActiveTracks;
	private int trackNumberVariance;
	private float trackFadeInDuration;
	private float trackFadeOutDuration;
	private float avgFadeTimeout;
	private float fadeTimeoutVariance;

	void OnEnable() {
		showDriftParams = new AnimBool(serializedObject == null || 
		                               serializedObject.FindProperty("driftTracks") == null ||
		                               serializedObject.FindProperty("driftTracks").boolValue);
	}

	public override void OnInspectorGUI() {
		EverloopController t = (EverloopController)target;
		int numTracks = t.GetComponentsInChildren<AudioSource>().Length;

		EditorGUI.BeginChangeCheck();

		float volume = EditorGUILayout.Slider("Master volume", t.volume, 0f, 1f);

		bool fadeInOnStart = EditorGUILayout.ToggleLeft("Fade in on start", t.fadeInOnStart);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Master fade in/out duration");
		float masterFadeDuration = EditorGUILayout.FloatField(t.masterFadeDuration);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();

		if (showDetailedInfo) {
			EditorGUILayout.HelpBox("Autopilot starts and stops random tracks according to the specified preferences",
				MessageType.None);
		}
		showDriftParams.target = EditorGUILayout.ToggleLeft("Enable autopilot", showDriftParams.target);
		t.enableAutopilot = showDriftParams.target;
		using (var group = new EditorGUILayout.FadeGroupScope(showDriftParams.faded)) {
			if (group.visible) {
				EditorGUI.indentLevel++;

				if (showDetailedInfo) {
					EditorGUILayout.HelpBox("Average number of tracks to be playing at the same time", 
											MessageType.None);
				}

				EditorGUILayout.LabelField("Number of active tracks:");
				numActiveTracks = EditorGUILayout.IntSlider(t.numActiveTracks, 1, numTracks);
				EditorGUILayout.Space();

				if (showDetailedInfo) {
					EditorGUILayout.HelpBox("Maximum deviation from the average active tracks", MessageType.None);
				}
				EditorGUILayout.LabelField("Track number variance:");
				trackNumberVariance = EditorGUILayout.IntSlider(t.trackNumberVariance, 0, numTracks);
				EditorGUILayout.Space();

				if (showDetailedInfo) {
					EditorGUILayout.HelpBox("How long it takes to fade in/out a track in autopilot", MessageType.None);
				}
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Tracks fade in duration");
				trackFadeInDuration = EditorGUILayout.FloatField(t.trackFadeInDuration);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Tracks fade out duration");
				trackFadeOutDuration = EditorGUILayout.FloatField(t.trackFadeOutDuration);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space();

				if (showDetailedInfo) {
					EditorGUILayout.HelpBox("How long it takes on average before removing or adding a track", 
											MessageType.None);
				}
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Track change interval");
				avgFadeTimeout = EditorGUILayout.FloatField(t.avgFadeTimeout);
				EditorGUILayout.EndHorizontal();

				if (showDetailedInfo) {
					EditorGUILayout.HelpBox("Maximum deviation from the average track change interval", 
											MessageType.None);
				}
				EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField("Track change interval variance:");
				fadeTimeoutVariance = EditorGUILayout.Slider(t.fadeTimeoutVariance, 0f, t.avgFadeTimeout);
				EditorGUILayout.EndVertical();

				EditorGUI.indentLevel--;
			}
		}

		showDetailedInfo = EditorGUILayout.ToggleLeft("Show detailed info", showDetailedInfo);

		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(target, "EverloopController");
			t.volume = volume;
			t.fadeInOnStart = fadeInOnStart;
			t.masterFadeDuration = masterFadeDuration;
			t.numActiveTracks = numActiveTracks;
			t.trackNumberVariance = trackNumberVariance;
			t.trackFadeInDuration = trackFadeInDuration;
			t.trackFadeOutDuration = trackFadeOutDuration;
			t.avgFadeTimeout = avgFadeTimeout;
			t.fadeTimeoutVariance = fadeTimeoutVariance;
		}

		EditorGUILayout.Separator();
		EditorGUILayout.HelpBox(string.Format("{0} tracks detected.", numTracks), 
								numTracks > 1? MessageType.Info : MessageType.Error);

		EditorUtility.SetDirty(t);

		//base.OnInspectorGUI();
	}

	void OnInspectorUpdate() {
		this.Repaint();
	}
}
