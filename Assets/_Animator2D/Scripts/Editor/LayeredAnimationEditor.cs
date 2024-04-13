using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Etienne.Animator2D;

namespace EtienneEditor.Animator2D
{
	[CustomEditor(typeof(LayeredAnimation))]
	public class LayeredAnimationEditor : Editor<LayeredAnimation>
	{
		public override bool HasPreviewGUI()
		{

			return Target.Animations != null && Target.Animations.Length >= 1;
		}
		
		public override void DrawPreview(Rect previewArea)
		{
			for (int i = 0; i < Target.Animations.Length; i++) 
			{
				Animation2DEditor.DrawSpritePreview(previewArea, Target.Animations[i].Frames[0].Sprite, i==0);
			}
		}
    }
}
