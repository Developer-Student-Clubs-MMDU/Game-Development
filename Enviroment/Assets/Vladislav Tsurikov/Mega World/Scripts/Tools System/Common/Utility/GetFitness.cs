using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class GetFitnessUtility
    {
        public static float GetFitness(Group group, Bounds bounds, RayHit rayHit)
        {
            if(IsMaskFilter(group))
            {
                MaskFilterSettings maskFilterSettings = (MaskFilterSettings)group.GetSettings(typeof(MaskFilterSettings));

                return GetFitnessFromMaskFilter(bounds, maskFilterSettings.Stack, maskFilterSettings.FilterMaskTexture2D, rayHit);
            }
            else
            {
                SimpleFilterSettings simpleFilterSettings = (SimpleFilterSettings)group.GetSettings(typeof(SimpleFilterSettings));

                return GetFitnessFromSimpleFilter(simpleFilterSettings, rayHit);
            }
        }

        public static float GetFitnessFromSimpleFilter(SimpleFilterSettings simpleFilterSettings, RayHit rayHit)
        {
            return simpleFilterSettings.GetFitness(rayHit.Point, rayHit.Normal);
        }

        public static float GetFitnessFromMaskFilter(Bounds bounds, MaskFilterStack stack, Texture2D filterMask, RayHit rayHit)
        {
            if(stack.Settings.Count != 0)
            {
                return GrayscaleFromTexture.GetFromWorldPosition(bounds, rayHit.Point, filterMask);
            }

            return 1;
        }

        public static bool IsMaskFilter(Group group)
        {
            if(group.FilterType == FilterType.MaskFilter)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}