using System;
using System.Collections.Generic;
using System.Text;
using DotNetCoords.Ellipsoid;

namespace DotNetCoords.Datum
{
  /// <summary>
  /// 
  /// </summary>
  public sealed class IRENET95Datum : Datum<IRENET95Datum>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="IRENET95Datum"/> class.
    /// </summary>
    public IRENET95Datum()
    {
      Name = "IRENET95 Datum";
      ReferenceEllipsoid = GRS80Ellipsoid.Instance;
      DX = 0.0;
      DY = 0.0;
      DZ = 0.0;
      DS = 0.0;
      RX = 0.0;
      RY = 0.0;
      RZ = 0.0;
    }
  }
}
