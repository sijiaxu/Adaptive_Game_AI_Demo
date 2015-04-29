using UnityEngine;
using System;
using UnityEditor;

[CustomEditor(typeof(GerstnerDisplace))]
public class GerstnerDisplaceEditor : Editor 
{    
    private SerializedObject serObj;

	public void OnEnable () 
	{
		serObj = new SerializedObject (target);
	}
	
    public override void OnInspectorGUI () 
    {
    	serObj.Update(); 
    	
    	GameObject go = ((GerstnerDisplace)serObj.targetObject).gameObject;
    	WaterBase wb = (WaterBase)go.GetComponent(typeof(WaterBase));    	
    	Material sharedWaterMaterial = wb.sharedMaterial;
    	
        GUILayout.Label ("Animates vertices using up to 4 generated waves", EditorStyles.miniBoldLabel);    
        
		if(sharedWaterMaterial) 
		{			
			Vector4 amplitude = WaterEditorUtility.GetMaterialVector("_GAmplitude", sharedWaterMaterial);
			Vector4 frequency = WaterEditorUtility.GetMaterialVector("_GFrequency", sharedWaterMaterial);
			Vector4 steepness = WaterEditorUtility.GetMaterialVector("_GSteepness", sharedWaterMaterial);
			Vector4 speed = WaterEditorUtility.GetMaterialVector("_GSpeed", sharedWaterMaterial);
			Vector4 directionAB = WaterEditorUtility.GetMaterialVector("_GDirectionAB", sharedWaterMaterial);
			Vector4 directionCD = WaterEditorUtility.GetMaterialVector("_GDirectionCD", sharedWaterMaterial);
			
			float amplitudeA = amplitude.x;
			float amplitudeB = amplitude.y;
			float amplitudeC = amplitude.z;
			float amplitudeD = amplitude.w;

			float frequencyA  = frequency.x;
			float frequencyB  = frequency.y;
			float frequencyC  = frequency.z;
			float frequencyD  = frequency.w;

			float steepnessA  = steepness.x;
			float steepnessB  = steepness.y;
			float steepnessC  = steepness.z;
			float steepnessD  = steepness.w;

			float speedA  = speed.x;
			float speedB  = speed.y;
			float speedC  = speed.z;
			float speedD  = speed.w;
			
			float xDirectionA  = directionAB.x;
			float yDirectionA  = directionAB.y;
			float xDirectionB  = directionAB.z;
			float yDirectionB  = directionAB.w;
			float xDirectionC  = directionCD.x;
			float yDirectionC  = directionCD.y;
			float xDirectionD  = directionCD.z;
			float yDirectionD  = directionCD.w;

			GUILayout.Label ("Wave A", EditorStyles.boldLabel);
									
			EditorGUILayout.BeginHorizontal ();
			amplitudeA = EditorGUILayout.FloatField("Amplitude", amplitudeA);
			frequencyA = EditorGUILayout.FloatField("Frequency", frequencyA);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			steepnessA = EditorGUILayout.FloatField("Steepness", steepnessA);
			speedA = EditorGUILayout.FloatField("Speed", speedA);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			xDirectionA = EditorGUILayout.FloatField("Direction (X)", xDirectionA);
			yDirectionA = EditorGUILayout.FloatField("Direction (Y)", yDirectionA);
			EditorGUILayout.EndHorizontal ();

			GUILayout.Label ("Wave B", EditorStyles.boldLabel);
									
			EditorGUILayout.BeginHorizontal ();
			amplitudeB = EditorGUILayout.FloatField("Amplitude", amplitudeB);
			frequencyB = EditorGUILayout.FloatField("Frequency", frequencyB);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			steepnessB = EditorGUILayout.FloatField("Steepness", steepnessB);
			speedB = EditorGUILayout.FloatField("Speed", speedB);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			xDirectionB = EditorGUILayout.FloatField("Direction (X)", xDirectionB);
			yDirectionB = EditorGUILayout.FloatField("Direction (Y)", yDirectionB);
			EditorGUILayout.EndHorizontal ();			

			GUILayout.Label ("Wave C", EditorStyles.boldLabel);
									
			EditorGUILayout.BeginHorizontal ();
			amplitudeC = EditorGUILayout.FloatField("Amplitude", amplitudeC);
			frequencyC = EditorGUILayout.FloatField("Frequency", frequencyC);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			steepnessC = EditorGUILayout.FloatField("Steepness", steepnessC);
			speedC = EditorGUILayout.FloatField("Speed", speedC);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			xDirectionC = EditorGUILayout.FloatField("Direction (X)", xDirectionC);
			yDirectionC = EditorGUILayout.FloatField("Direction (Y)", yDirectionC);
			EditorGUILayout.EndHorizontal ();	
			
			GUILayout.Label ("Wave D", EditorStyles.boldLabel);
									
			EditorGUILayout.BeginHorizontal ();
			amplitudeD = EditorGUILayout.FloatField("Amplitude", amplitudeD);
			frequencyD = EditorGUILayout.FloatField("Frequency", frequencyD);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			steepnessD = EditorGUILayout.FloatField("Steepness", steepnessD);
			speedD = EditorGUILayout.FloatField("Speed", speedD);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			xDirectionD = EditorGUILayout.FloatField("Direction (X)", xDirectionD);
			yDirectionD = EditorGUILayout.FloatField("Direction (Y)", yDirectionD);
			EditorGUILayout.EndHorizontal ();	
			
			amplitude = new Vector4 (amplitudeA, amplitudeB, amplitudeC, amplitudeD);
			frequency = new Vector4 (frequencyA, frequencyB, frequencyC, frequencyD);
			steepness = new Vector4 (steepnessA, steepnessB, steepnessC, steepnessD);
			speed = new Vector4 (speedA, speedB, speedC, speedD);
			float dirALen = 1.0f;
			float dirBLen = 1.0f;
			float dirCLen = 1.0f;
			float dirDLen = 1.0f;
			directionAB = new Vector4 (xDirectionA / dirALen, yDirectionA / dirALen, xDirectionB / dirBLen, yDirectionB / dirBLen);
			directionCD = new Vector4 (xDirectionC / dirCLen, yDirectionC / dirCLen, xDirectionD / dirDLen, yDirectionD / dirDLen);
			if (GUI.changed) {
				WaterEditorUtility.SetMaterialVector("_GAmplitude", amplitude, sharedWaterMaterial);
				WaterEditorUtility.SetMaterialVector("_GFrequency", frequency, sharedWaterMaterial);
				WaterEditorUtility.SetMaterialVector("_GSteepness", steepness, sharedWaterMaterial);
				WaterEditorUtility.SetMaterialVector("_GSpeed", speed, sharedWaterMaterial);
				WaterEditorUtility.SetMaterialVector("_GDirectionAB", directionAB, sharedWaterMaterial);
				WaterEditorUtility.SetMaterialVector("_GDirectionCD", directionCD, sharedWaterMaterial);
			}
			
			/*
			 
			Vector4 animationTiling = WaterEditorUtility.GetMaterialVector("_AnimationTiling", sharedWaterMaterial);
			Vector4 animationDirection = WaterEditorUtility.GetMaterialVector("_AnimationDirection", sharedWaterMaterial);
			
			float firstTilingU = animationTiling.x*100.0F;
			float firstTilingV = animationTiling.y*100.0F;
			float firstDirectionU = animationDirection.x;
			float firstDirectionV = animationDirection.y;

			float secondTilingU = animationTiling.z*100.0F;
			float secondTilingV = animationTiling.w*100.0F;
			float secondDirectionU = animationDirection.z;
			float secondDirectionV = animationDirection.w;
						
			
			EditorGUILayout.BeginHorizontal ();
			firstTilingU = EditorGUILayout.FloatField("First Tiling U", firstTilingU);
			firstTilingV = EditorGUILayout.FloatField("First Tiling V", firstTilingV);
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal ();
			secondTilingU = EditorGUILayout.FloatField("Second Tiling U", secondTilingU);
			secondTilingV = EditorGUILayout.FloatField("Second Tiling V", secondTilingV);
			EditorGUILayout.EndHorizontal ();			
			
			EditorGUILayout.BeginHorizontal ();
			firstDirectionU = EditorGUILayout.FloatField("1st Animation U", firstDirectionU);
			firstDirectionV = EditorGUILayout.FloatField("1st Animation V", firstDirectionV);
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal ();
			secondDirectionU = EditorGUILayout.FloatField("2nd Animation U", secondDirectionU);
			secondDirectionV = EditorGUILayout.FloatField("2nd Animation V", secondDirectionV);
			EditorGUILayout.EndHorizontal ();
		
			animationDirection = new Vector4(firstDirectionU,firstDirectionV, secondDirectionU,secondDirectionV);
			animationTiling = new Vector4(firstTilingU/100.0F,firstTilingV/100.0F, secondTilingU/100.0F,secondTilingV/100.0F);				
			
			WaterEditorUtility.SetMaterialVector("_AnimationTiling", animationTiling, sharedWaterMaterial);
			WaterEditorUtility.SetMaterialVector("_AnimationDirection", animationDirection, sharedWaterMaterial);
			
			EditorGUILayout.Separator ();		
			
	    	GUILayout.Label ("Displacement Strength", EditorStyles.boldLabel);				
			
			float heightDisplacement = WaterEditorUtility.GetMaterialFloat("_HeightDisplacement", sharedWaterMaterial);
			
			heightDisplacement = EditorGUILayout.Slider("Height", heightDisplacement, 0.0F, 5.0F);
			WaterEditorUtility.SetMaterialFloat("_HeightDisplacement", heightDisplacement, sharedWaterMaterial);	
			*/	
		}
		
    	serObj.ApplyModifiedProperties();
    }
}