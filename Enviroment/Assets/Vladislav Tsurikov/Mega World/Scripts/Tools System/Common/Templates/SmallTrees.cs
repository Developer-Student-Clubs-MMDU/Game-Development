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
	[Template("Trees/Small Trees", new Type[]{typeof(AdvancedBrushTool), typeof(StamperTool)}, new ResourceType[]{ResourceType.InstantItem, ResourceType.GameObject})]
	public class SmallTrees : Template
	{
		public override void Apply(Group group)
    	{
			ScatterSettings scatterSettings = (ScatterSettings)group.GetSettings(typeof(ScatterSettings));
			MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(typeof(MaskFilterSettings));

			#region Scatter Settings
            scatterSettings.Stack.Settings.Clear();

            Grid grid = (Grid)scatterSettings.Stack.CreateSettingsAndAdd(typeof(Grid), group);
            grid.RandomisationType = RandomisationType.Square;
    		grid.Vastness = 1;
    		grid.GridStep = new Vector2(5, 5);

            FailureRate failureRate = (FailureRate)scatterSettings.Stack.CreateSettingsAndAdd(typeof(FailureRate), group);
            failureRate.Value = 65;
			#endregion

			#region Mask Filters
    		maskFilterSettings.Stack.Clear();

    		HeightFilter heightFilter = (HeightFilter)maskFilterSettings.Stack.CreateSettingsAndAdd(typeof(HeightFilter), group);
    		heightFilter.MinHeight = 0;
    		heightFilter.MaxHeight = 620;
    		heightFilter.AddHeightFalloff = 100;

    		SlopeFilter slopeFilter = (SlopeFilter)maskFilterSettings.Stack.CreateSettingsAndAdd(typeof(SlopeFilter), group);
    		#endregion
		}

		public override void Apply(Prototype proto)
    	{
			TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetSettings(typeof(TransformComponentSettings));
            OverlapCheckSettings overlapCheckSettings = (OverlapCheckSettings)proto.GetSettings(typeof(OverlapCheckSettings));
			
    		#region Transform Components
    		transformComponentSettings.Stack.Clear();

    		TreeRotation treeRotation = (TreeRotation)transformComponentSettings.Stack.CreateSettingsAndAdd(typeof(TreeRotation), proto);
    		Align align = (Align)transformComponentSettings.Stack.CreateSettingsAndAdd(typeof(Align), proto);
    		PositionOffset positionOffset = (PositionOffset)transformComponentSettings.Stack.CreateSettingsAndAdd(typeof(PositionOffset), proto);
    		SlopePosition slopePosition = (SlopePosition)transformComponentSettings.Stack.CreateSettingsAndAdd(typeof(SlopePosition), proto);
    		Scale scale = (Scale)transformComponentSettings.Stack.CreateSettingsAndAdd(typeof(Scale), proto); 
    		#endregion

    		#region OverlapCheckSettings
    		overlapCheckSettings.OverlapShape = OverlapShape.Sphere;
    		overlapCheckSettings.SphereCheck.VegetationMode = true;
    		overlapCheckSettings.SphereCheck.Priority = 1;
    		overlapCheckSettings.SphereCheck.TrunkSize = 0.4f;
    		overlapCheckSettings.SphereCheck.ViabilitySize = 2f;
    		#endregion
		}
	}
}
#endif