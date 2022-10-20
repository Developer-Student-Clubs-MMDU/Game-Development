#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using VladislavTsurikov.MegaWorldSystem.AdvancedBrush;
using VladislavTsurikov.MegaWorldSystem.Stamper;

namespace VladislavTsurikov.MegaWorldSystem
{
	[Template("Cliffs/Small Rocks", new Type[]{typeof(AdvancedBrushTool), typeof(StamperTool)}, new ResourceType[]{ResourceType.InstantItem, ResourceType.GameObject})]
	public class SmallCliffs : Template
	{
		public override void Apply(Group group)
    	{
			MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(typeof(MaskFilterSettings));
			ScatterSettings scatterSettings = (ScatterSettings)group.GetSettings(typeof(ScatterSettings));

			#region Scatter Settings
			scatterSettings.Stack.Settings.Clear();

            Grid grid = (Grid)scatterSettings.Stack.CreateSettingsAndAdd(typeof(Grid), group);
            grid.RandomisationType = RandomisationType.Square;
    		grid.Vastness = 1;
    		grid.GridStep = new Vector2(1.3f, 1.3f);

            FailureRate failureRate = (FailureRate)scatterSettings.Stack.CreateSettingsAndAdd(typeof(FailureRate), group);
            failureRate.Value = 60f;
			#endregion

			#region Mask Filters
			maskFilterSettings.Stack.Clear();

    		NoiseFilter noiseFilter = (NoiseFilter)maskFilterSettings.Stack.CreateSettingsAndAdd(typeof(NoiseFilter), group);
            noiseFilter.NoiseSettings = new NoiseSettings();
    		noiseFilter.NoiseSettings.TransformSettings = new NoiseSettings.NoiseTransformSettings();
    		noiseFilter.NoiseSettings.TransformSettings.Scale = new Vector3(31, 40, 31);

            MaskOperationsFilter remapFilter = (MaskOperationsFilter)maskFilterSettings.Stack.CreateSettingsAndAdd(typeof(MaskOperationsFilter), group);
            remapFilter.MaskOperations = MaskOperations.Remap;
    		remapFilter.RemapRange.x = 0.44f;
    		remapFilter.RemapRange.y = 0.47f;

            MaskOperationsFilter invertFilter = (MaskOperationsFilter)maskFilterSettings.Stack.CreateSettingsAndAdd(typeof(MaskOperationsFilter), group);
			invertFilter.MaskOperations = MaskOperations.Invert;
    		invertFilter.StrengthInvert = 1;

    		SlopeFilter slopeFilter = (SlopeFilter)maskFilterSettings.Stack.CreateSettingsAndAdd(typeof(SlopeFilter), group);
    		slopeFilter.MinSlope = 48;
    		slopeFilter.MaxSlope = 90;
            slopeFilter.AddSlopeFalloff = 28;
			#endregion
		}

		public override void Apply(Prototype proto)
    	{
			TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetSettings(typeof(TransformComponentSettings));
            OverlapCheckSettings overlapCheckSettings = (OverlapCheckSettings)proto.GetSettings(typeof(OverlapCheckSettings));

    		#region Transform Components
    		transformComponentSettings.Stack.Clear();

            Rotation rotation = (Rotation)transformComponentSettings.Stack.CreateSettingsAndAdd(typeof(Rotation), proto);
    		rotation.RandomizeOrientationX = 100;
    		rotation.RandomizeOrientationY = 100;
    		rotation.RandomizeOrientationZ = 100;

            Scale scale = (Scale)transformComponentSettings.Stack.CreateSettingsAndAdd(typeof(Scale), proto); 
			scale.MinScale = new Vector3(0.8f, 0.8f, 0.8f);
    		scale.MaxScale = new Vector3(1.2f, 1.2f, 1.2f);
    		#endregion

    		#region OverlapCheckSettings
    		overlapCheckSettings.OverlapShape = OverlapShape.None;
    		#endregion
		}
	}
}
#endif