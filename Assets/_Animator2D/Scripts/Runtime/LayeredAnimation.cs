using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Etienne.Animator2D
{
	//TODO: create from animation selection
	//[CreateAssetMenu(menuName = "Layered animation")]
	public class LayeredAnimation : ScriptableObject
	{
		public Animation2D[] Animations=>animationLayers;
		[SerializeField] Animation2D[] animationLayers;
    }
}
