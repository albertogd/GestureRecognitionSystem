using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum DistanceType
    {
        Normal,
        Selective,
        Threshold,
        SelectiveWithThreshold,
        SelectiveUpperBody,
        SelectiveArms,
        SelectiveUpperBodySquared,
        SelectiveUpperBodyCube
    }
}
