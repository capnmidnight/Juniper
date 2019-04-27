using UnityEngine;
using System.Collections;
using UnityEditor;

public class NoKeywordToggleDrawer : MaterialPropertyDrawer {

	override public void OnGUI (Rect position, MaterialProperty prop, string label, MaterialEditor editor)
	{
		bool state = EditorGUI.Toggle(position, label, (prop.floatValue==1));
		if (state != (prop.floatValue==1)) {
			prop.floatValue = state ? 1 : 0;
		}
	}
	override public void Apply (MaterialProperty prop)
	{
//		base.Apply (prop);
	}
	public override float GetPropertyHeight (MaterialProperty prop, string label, MaterialEditor editor)
	{
		return base.GetPropertyHeight (prop, label, editor);
	}
}
