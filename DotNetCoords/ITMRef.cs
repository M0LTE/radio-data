using System;
using DotNetCoords.Datum;

namespace DotNetCoords
{
  /// <summary>
  /// Class to represent an Irish Transverse Mercator (ITM) reference.
  /// ITM is the geographic coordinate system for Ireland
  /// <ul>
  /// 	<li>Projection: Transverse Mercator</li>
  /// 	<li>Reference ellipsoid: GRS80</li>
  /// 	<li>Units: metres</li>
  /// 	<li>Origin: 53° 30' North; 8° West</li>
  /// 	<li>False co-ordinates of origin: 600,000m West; 750,000m South</li>
  ///   <li>EPSG code: 2157</li>
  /// </ul>
  /// </summary>
  public sealed class ITMRef : CoordinateSystem
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="easting">The easting in metres</param>
    /// <param name="northing">The northing in metres</param>
    public ITMRef(double easting, double northing) : base(IRENET95Datum.Instance)
    {
      Easting = easting;
      Northing = northing;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ll">The LatLng to convert from</param>
    public ITMRef(LatLng ll) : base(IRENET95Datum.Instance)
    {
      var en = LatLngToEastingNorthing(ll, 750000, 600000, 53.5, -8, 0.99982);

      Easting = en.Easting;
      Northing = en.Northing;
    }

    private double easting;
    /// <summary>
    /// Gets the easting in metres relative to the origin
    /// </summary>
    public double Easting
    {
      get => easting;
      private set
      {
        if (value < 400000.0 || value >= 800000.0)
        {
          throw new ArgumentException("Easting (" + value
                                                  + ") is invalid. Must be greater than or equal to 400000 and "
                                                  + "less than 800000");
        }

        easting = value;
      }
    }

    private double northing;

    /// <summary>
    /// Gets the northing in metres relative to the origin
    /// </summary>
    public double Northing
    {
      get => northing;
      private set
      {
        if (value < 500000.0 || value > 1000000.0)
        {
          throw new ArgumentException("Northing (" + value
                                                   + ") is invalid. Must be greater than or equal to 500000 and less "
                                                   + "than or equal to 1000000");
        }

        northing = value;
      }
    }

    /// <summary>
    /// Convert this Irish Transverse Mercator (ITM) reference to a latitude/longitude pair using the
    /// IRENET95 datum. Note that, the LatLng object may need to be converted to the
    /// WGS84 datum depending on the application.
    /// </summary>
    /// <returns>
    /// A LatLng object representing this Irish Transverse Mercator (ITM) reference using the
    /// IRENET95 datum
    /// </returns>
    public override LatLng ToLatLng()
    {
      return ToLatLng(Easting, Northing, 750000, 600000, 53.5, -8, 0.99982);
    }

    /// <summary>
    /// Convert this Irish Transverse Mercator (ITM) reference to an Irish grid reference
    /// </summary>
    /// <returns>
    /// The Irish grid reference
    /// </returns>
    public IrishRef ToIrishRef()
    {
      // convert to LatLng then to IrishRef
      var ll = ToLatLng();
      return new IrishRef(ll);
    }
  }
}
