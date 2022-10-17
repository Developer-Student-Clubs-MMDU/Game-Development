// CreateCloudsToy ca create a Cloudstoy Mngr from scrath 
// or can access an CloudsToy already existing in the scene and modify anything you want in runtime. (Color, number clouds, size, etc...).
//
// Keep in mind you have to assign all basic relevant data to create a cloud from zero (textures & shaders mostly).
//
// CloudsToy public presets:
// SetPresetFantasy();
// SetPresetSunrise();
// SetPresetStormy();
//
// To force update & Regenerate/Repaint all the clouds.
// EditorRepaintClouds();


/* //CloudsToy Main Variables that can be used to modify the clouds in runtime.
// In case you don't have the CloudsToy SRC version.
//
//
//=================================================================================
// Main controllers used in cloud adjustement in Inspector Window.
//=================================================================================
public enum TypePreset { None = 0, Stormy = 1, Sunrise = 2, Fantasy = 3 }	// Cloud preset. Presets change several Clouds vars at once.
public TypePreset CloudPreset = TypePreset.None;

public enum TypeRender { Bright = 0, Realistic = 1 }	// Type of shader to use in the clouds.( AlphaBlended/Additive for now).
public TypeRender CloudRender = TypeRender.Bright;

public enum TypeDetail { Low = 0, Normal = 1, High = 2 } 
public TypeDetail CloudDetail = TypeDetail.Low;			// Cloud's detail. How many particles the cloud is made of.

public enum TypeCloud { Nimbus1=0, Nimbus2=1, Nimbus3=2, Nimbus4=3, Cirrus1=4, Cirrus2=5, MixNimbus=6, MixCirrus=7, MixAll=8, PT1 = 9 }
public TypeCloud TypeClouds = TypeCloud.Nimbus1;		// Texture used in the cloud's particles.


//=================================================================================
// Particles Advanced Settings
//=================================================================================
public float positionCheckerTime = 0.1f;  // The time ratio used to check the cloud position. (LateUpdate) (10 times in a second).
public float SizeFactorPart = 1;		// Cloud Particle size multiplier (used in shuriken -> startSize before emitting the particles).
public float EmissionMult = 1;			// Cloud Particle number particles multiplier (used in shuriken -> Emitter -> count before emitting the particles).

public bool _showDebug = true;			// Show the Debug.Log in console.


//=================================================================================
// Soft Clouds: Clouds that strech (based on their velocity).
// We need the velicity on the particles of the cloud for get the strech effect (SpreadDir)
// Length scale value for the partcile Renderer.
//=================================================================================
public bool SoftClouds = false;						// Soft clouds are streched (not billboarded) particles.
public Vector3 SpreadDir = new Vector3(-1, 0, 0);	// strech Direction
public float LengthSpread = 1;						// strech Length 

//=================================================================================
// General Clouds Properties.
//=================================================================================
public int NumberClouds = 100;								// Number of active couds (can be inactive clouds too).
public Vector3 Side = new Vector3(1000, 500, 1000);			// CloudToy's Size where the clouds will be created (Blue Box in The Inspector).
public Vector3 MaximunVelocity = new Vector3(-10, 0, 0);	// Clouds Velocity (greater clouds will move slower than little ones).

//=================================================================================
// Velocity of the clouds
//=================================================================================
public float VelocityMultipier = 1;							// clouds Velocity multiplier.
public float DisappearMultiplier = 1.5f;					// Cloud disapear multiplier (Yellow box in inspector).

//=================================================================================
// Paint Cloud Section.
//=================================================================================
public enum TypePaintDistr { Random = 0, Below = 1 }
public TypePaintDistr PaintType = TypePaintDistr.Below;		// Paint type algorith to be applied.
public Color CloudColor = new Color(1, 1, 1, 1);			// Cloud Color (it's aaplied to the cloud material -> Main Color)
public Color MainColor = new Color(1, 1, 1, 1);				// Main Color to apply to each independent cloud particle.
public Color SecondColor = new Color(0.5f, 0.5f, 0.5f, 1);	// Secondary Color to apply to each independent cloud particle.
public int TintStrength = 50;								// if TintStrength == 0 only Main Color applies to the cloud. 
public float offset = 0.5f;									// High ofsset to know where start painting the secondary color.

//=================================================================================
// Maximun size of the clouds (wisth, height, depth).	
//=================================================================================
public int MaxWidthCloud = 100;
public int MaxHeightCloud = 40;
public int MaxDepthCloud = 100;
public bool  FixedSize = true;			// If not fixed size, clouds will have a random size based on this starting values.

//=================================================================================
// Shadow of each cloud.
//=================================================================================
public enum TypeShadow { All = 0, Most = 1, Half = 2, Some = 3, None = 4 }
public TypeShadow NumberOfShadows = TypeShadow.Some;

//=================================================================================
// Textures that can be used to make the clouds.
//=================================================================================
public Texture2D[] CloudsTextAdd = new Texture2D[6];			// Clouds Textures used with the Bright shader.
public Texture2D[] CloudsTextBlended = new Texture2D[6];		// Clouds Textures used with the AlphaBlended shader (Realistic)

//=================================================================================
// Angular velocity in the particle's of the cloud. (Autorotation if you like)
//=================================================================================
public bool IsAnimate = true;			// Are the cloud's particles being rotated over themselves?
public float AnimationVelocity = 0;		// Rotation Angle (used in Shuriken -> RotationOverLifetime module)
//
//
*/


using UnityEngine;
using System.Collections;

public class CreateCloudsToy : MonoBehaviour {

	public bool createNewCloudsToy = true;

	[Space(10)]
	[Tooltip("if createNewCloudsToy is true, goCloud will be this transform.\n" +
		"if not goCloud will be the gameobject where CloudsToy is located (some CloudsToy Mngr).")]
	public GameObject goCloud;	// CloudsToy Game Object.

	[SerializeField]
	[Tooltip("if createNewCloudsToy is true, this var will contain the new Component of CloudsToy script.\n" +
		"if not here will be assigned the CloudsToy script (contained in the GoCloud).")]
	private CloudsToy _cloudsToy;	// Cloudstoy component container


	// Clouds shaders & projector material to be used.
	// Use by default the same values the CloudsToy Prefab has (dragging it in the inspector).
	[Space(10)]
	public Shader realisticShader;
	public Shader brightShader;
	public Material projectorMaterial;


	// Cloud Textures. ( Assign in inspector the same textures the CloudsToy Prefab has).
	[Space(10)]
	public Texture2D[] CloudsTextAdd = new Texture2D[6];
	public Texture2D[] CloudsTextBlended = new Texture2D[6];

	[Space(10)]
	public Color MainColor = Color.blue;

	private bool isInitialized = false;
	
	IEnumerator Start () {

		// CreateCloudsToy Creation: Can  be done by assignning the needed variables that the original prefab has.
		// So, by assigning the needed textures and shaders, it will work.
		if(createNewCloudsToy){		
			goCloud = this.gameObject;
			_cloudsToy = goCloud.AddComponent<CloudsToy> ();		// Create the CloudsToy Component
			_cloudsToy.enabled = true;

			// Assign the clouds shaders
			_cloudsToy.realisticShader = realisticShader;
			_cloudsToy.brightShader = brightShader;
			_cloudsToy.projectorMaterial = projectorMaterial;

			// Assign the basic textures to create the clouds with.
			for (int i = 0; i < CloudsTextAdd.Length; i++)
				_cloudsToy.CloudsTextAdd[i] = CloudsTextAdd[i];
			
			for (int i = 0; i < CloudsTextBlended.Length; i++)
				_cloudsToy.CloudsTextBlended[i] = CloudsTextBlended[i];

			// default cloud type value
			_cloudsToy.SetPresetStormy(); // Creates Stormy type clouds.
		}
		else{
			if(!goCloud.activeSelf) goCloud.SetActive(true);
			_cloudsToy = goCloud.GetComponent<CloudsToy> ();	// Can be accessed and modified.
		}


		yield return new WaitForSeconds(1.0f);

		// Tweaking the values... afetr 1 second to see the changes in the editor while playing.
		// Blue awfull clouds!!!
		_cloudsToy.SoftClouds = false;
		_cloudsToy.NumberClouds = 170;
		_cloudsToy.SizeFactorPart = 1;
		_cloudsToy.DisappearMultiplier = 3;
		_cloudsToy.VelocityMultipier = 0;
		_cloudsToy.PaintType = CloudsToy.TypePaintDistr.Below;
		_cloudsToy.CloudDetail = CloudsToy.TypeDetail.High;
		_cloudsToy.CloudColor = MainColor;		// Blue Main Color in the clouds.
		_cloudsToy.MainColor = Color.grey;
		_cloudsToy.SecondColor = Color.gray * 0.5f;
		_cloudsToy.TintStrength = 80;
		_cloudsToy.offset = 0.8f;
		_cloudsToy.PT1ScaleWidth = 55;
		_cloudsToy.PT1ScaleHeight = 5;
		_cloudsToy.MaxDepthCloud = 130;
		_cloudsToy.IsAnimate = true;
		_cloudsToy.AnimationVelocity = 0.6f;
		_cloudsToy.NumberOfShadows = CloudsToy.TypeShadow.Some;

		_cloudsToy.EditorRepaintClouds();
		isInitialized = true;
	}

	void Update(){
		if(!isInitialized) return;

		if(_cloudsToy.CloudColor != MainColor) 
			_cloudsToy.CloudColor = MainColor;
	}
		
}




/*
// Here you can see the CloudsToy original code to create the clouds using the presets.
// So you can know what variables are actually being tweaked.

// Clouds type Presets.
public void  SetPresetStormy (){

	CloudPreset = TypePreset.Stormy;
	CloudRender = TypeRender.Realistic;
	CloudDetail = TypeDetail.Normal;
	SetCloudDetailParams();
	TypeClouds = Type.Nimbus2;
	SoftClouds = false;
	SpreadDir = new Vector3(-1, 0, 0);
	LengthSpread = 1;
	NumberClouds = 100;
	//Side = new Vector3(2000, 500, 2000);
	DisappearMultiplier = 2;
	MaximunVelocity = new Vector3(-10, 0, 0);
	VelocityMultipier = 0.85f;
	PaintType = TypePaintDistr.Below;
	CloudColor = new Color(0.9f, 0.9f, 0.9f, 0.5f);
	MainColor = new Color(0.62f, 0.62f, 0.62f, 0.3f);
	SecondColor = new Color(0.31f, 0.31f, 0.31f, 1);
	TintStrength = 80;
	offset = 0.8f;
	MaxWithCloud = 200;
	MaxTallCloud = 50;
	MaxDepthCloud = 200;
	FixedSize = false;
	NumberOfShadows = TypeShadow.Some;
}

public void  SetPresetSunrise (){

	CloudPreset = TypePreset.Sunrise;
	CloudRender = TypeRender.Bright;
	CloudDetail = TypeDetail.Low;
	SetCloudDetailParams();
	EmissionMult = 1.6f;
	SizeFactorPart = 1.5f;
	TypeClouds = Type.Cirrus1;
	SoftClouds = true;
	SpreadDir = new Vector3(-1, 0, 0);
	LengthSpread = 4;
	NumberClouds = 135;
	//Side = new Vector3(2000, 500, 2000);
	DisappearMultiplier = 2;
	MaximunVelocity = new Vector3(-10, 0, 0);
	VelocityMultipier = 6.2f;
	PaintType = TypePaintDistr.Below;
	CloudColor = new Color(0.2f, 0.2f, 0.2f, 1f);
	MainColor = new Color(1f, 1f, 0.66f, 0.5f);
	SecondColor = new Color(1f, 0.74f, 0f, 1f);
	TintStrength = 100;
	offset = 1;
	MaxWithCloud = 500;
	MaxTallCloud = 20;
	MaxDepthCloud = 500;
	FixedSize = true;
	NumberOfShadows = TypeShadow.None;

}

public void  SetPresetFantasy (){ 

	CloudPreset = TypePreset.Fantasy;
	CloudRender = TypeRender.Bright;
	CloudDetail = TypeDetail.Low;
	EmissionMult = 0.3f;
	SetCloudDetailParams();
	TypeClouds = Type.Nimbus4;
	SoftClouds = false;
	SpreadDir = new Vector3(-1, 0, 0);
	LengthSpread = 1;
	NumberClouds = 200;
	//Side = new Vector3(2000, 500, 2000);
	DisappearMultiplier = 2;
	MaximunVelocity = new Vector3(-10, 0, 0);
	VelocityMultipier = 0.50f;
	PaintType = TypePaintDistr.Random;
	CloudColor = new Color(0.15f, 0.15f, 0.15f, 0.5f);
	MainColor = new Color(1, 0.62f, 0, 1);
	SecondColor = new Color(0.5f, 0.5f, 0.5f, 1);
	TintStrength = 50;
	offset = 0.2f;
	MaxWithCloud = 200;
	MaxTallCloud = 50;
	MaxDepthCloud = 200;
	FixedSize = true;
	NumberOfShadows = TypeShadow.Some;
}

*/
