using UnityEngine;
using System.Collections.Generic;

namespace VladislavTsurikov.Extensions
{
    public static class LayerEx
    {
        public static bool IsLayerBitSet(int layerBits, int layerNumber)
        {
            return (layerBits & (1 << layerNumber)) != 0;
        }
    }
}