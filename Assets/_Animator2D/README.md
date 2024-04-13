# Etienne's 2D Animator
This package requires the Etienne git package : https://github.com/Omadel/Etienne.git
If it doesn't want to download, it could be that you don't have git installed.
Git app's link : https://git-scm.com/
Restart Unity AND UnityHub afetr installing it.
It should now launch correctly.


The creation of 2DAnimation and 2DAnimatorControllers are under "Assets/Create/2D/Animator/" or "Assets/Create/Etienne/2D/Animator".
The creation of a 2DAnimation can be shortened if you select multiple sprite assets before creating it, it will automaticly apply the selected sprites assets to the Animation2D asset.

To play an animation, create a "2D Object/Sprite/Aniumated Sprite" or add a "Animator2D" to a GameObject.
To the animator add its controller, the first state is played by default.

Here is an example how to play an animation:
```cs
using Etienne.Animator2D;
using UnityEngine;

class PlayAnimation : MonoBehaviour
{
	[SerializeField] private Animator2D animator;
	[SerializeField] private string stateName = "Jump";

	private void Start()
	{
		animator.SetState(stateName, false);
	}
}
```
The false boolean is to not force the re-start of the animation, if you want to force the animation to begin immedialty and a the 1st frame write true instead.