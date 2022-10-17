using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class VisualisationSettings 
    {
        public bool VisualizeOverlapCheckSettings = false;
        public BrushHandlesSettings BrushHandlesSettings = new BrushHandlesSettings();
        public MaskFiltersSettings MaskFiltersSettings = new MaskFiltersSettings();
        public AdvancedSimpleFilterSettings SimpleFilterSettings = new AdvancedSimpleFilterSettings();
    }
}
