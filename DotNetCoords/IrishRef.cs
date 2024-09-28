using System;
using DotNetCoords.Datum;

namespace DotNetCoords
{
  /// <summary>
  /// Class to represent an Irish National Grid reference.
  /// <p>
  /// 	<b>Irish National Grid</b><br/>
  /// 	<ul>
  /// 		<li>Projection: Transverse Mercator</li>
  /// 		<li>Reference ellipsoid: Modified Airy</li>
  /// 		<li>Units: metres</li>
  /// 		<li>Origin: 53°30'N, 8°W</li>
  /// 		<li>False co-ordinates of origin: 200000m east, 250000m north</li>
  ///     <li>EPSG: 29903</li>
  /// 	</ul>
  /// </p>
  /// </summary>
  public sealed class IrishRef : CoordinateSystem 
  {
    /**
     * The easting in metres relative to the origin of the British National Grid.
     */
    private double _easting;

    /**
     * The northing in metres relative to the origin of the British National Grid.
     */
    private double _northing;

    private const double ScaleFactor = 1.000035;

    private const double FalseOriginLatitude = 53.5;

    private const double FalseOriginLongitude = -8.0;

    private const double FalseOriginEasting = 200000.0;

    private const double FalseOriginNorthing = 250000.0;

    /// <summary>
    /// Create a new Irish grid reference using a given easting and
    /// northing. The easting and northing must be in metres and must be relative
    /// to the origin of the Irish National Grid.
    /// </summary>
    /// <param name="easting">the easting in metres. Must be greater than or equal to 0.0 and
    /// less than 800000.0.</param>
    /// <param name="northing">the northing in metres. Must be greater than or equal to 0.0 and
    /// less than 1400000.0.</param>
    public IrishRef(double easting, double northing) : base (Ireland1965Datum.Instance)
    {
      Easting = easting;
      Northing = northing;
    }

    /// <summary>
    /// Take a string formatted as a six-figure Irish grid reference (e.g. "N514131")
    /// and create a new IrishRef object that represents that grid reference. 
    /// </summary>
    /// <param name="gridRef">A string representing a six-figure Irish grid reference
    /// in the form XY123456</param>
    public IrishRef(string gridRef) : base (Ireland1965Datum.Instance)
    {
      // if (ref.matches(""))
      // TODO 2006-02-05 : check format
      var ch = gridRef[0];
      // Thanks to Nick Holloway for pointing out the radix bug here
      var east = int.Parse(gridRef.Substring(1, 3)) * 100;
      var north = int.Parse(gridRef.Substring(4, 3)) * 100;
      if (ch > 73)
        ch--; // Adjust for no I
      double nx = ((ch - 65) % 5) * 100000;
      var ny = (4 - Math.Floor((double)(ch - 65) / 5)) * 100000;

      Easting = east + nx;
      Northing = north + ny;
    }

    /// <summary>
    /// Create an IrishRef object from the given latitude and longitude.
    /// </summary>
    /// <param name="ll">The latitude and longitude.</param>
    public IrishRef(LatLng ll) : base(Ireland1965Datum.Instance)
    {
      var en = LatLngToEastingNorthing(ll, FalseOriginNorthing, FalseOriginEasting, FalseOriginLatitude,
        FalseOriginLongitude, ScaleFactor);
      
      Easting = en.Easting;
      Northing = en.Northing;
    }

    /// <summary>
    /// Return a String representation of this Irish grid reference showing the
    /// easting and northing in metres.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current Irish grid reference.
    /// </returns>
    public override string ToString() 
    {
      return "(" + _easting + ", " + _northing + ")";
    }

    /// <summary>
    /// Return a String representation of this Irish grid reference using the
    /// six-figure notation in the form X123456
    /// </summary>
    /// <returns>A string representing this Irish grid reference in six-figure
    /// notation</returns>
    public string ToSixFigureString() 
    {
      var hundredkmE = (int) Math.Floor(_easting / 100000);
      var hundredkmN = (int) Math.Floor(_northing / 100000);
      
      var charOffset = 4 - hundredkmN;
      var index = 65 + (5 * charOffset) + hundredkmE;
      if (index >= 73)
        index++;
      var letter = ((char)index).ToString();

      var e = (int) Math.Floor((_easting - (100000 * hundredkmE)) / 100);
      var n = (int) Math.Floor((_northing - (100000 * hundredkmN)) / 100);
      var es = "" + e;
      if (e < 100)
        es = "0" + es;
      if (e < 10)
        es = "0" + es;
      var ns = "" + n;
      if (n < 100)
        ns = "0" + ns;
      if (n < 10)
        ns = "0" + ns;

      return letter + es + ns;
    }

    /// <summary>
    /// Convert this Irish grid reference to a latitude/longitude pair using the
    /// Ireland 1965 datum. Note that, the LatLng object may need to be converted to the
    /// WGS84 datum depending on the application.
    /// </summary>
    /// <returns>
    /// A LatLng object representing this Irish grid reference using the
    /// Ireland 1965 datum
    /// </returns>
    public override LatLng ToLatLng()
    {
      return ToLatLng(Easting, Northing, FalseOriginNorthing, FalseOriginEasting, FalseOriginLatitude,
        FalseOriginLongitude, ScaleFactor);
    }


    /// <summary>
    /// Gets the easting in metres relative to the origin of the Irish Grid.
    /// </summary>
    /// <value>The easting.</value>
    public double Easting 
    {
      get => _easting;
      private set
      {
        if (value < 0.0 || value >= 400000.0)
        {
          throw new ArgumentException("Easting (" + value
              + ") is invalid. Must be greater than or equal to 0 and "
              + "less than 400000");
        }

        _easting = value;
      }
    }

    /// <summary>
    /// Gets the northing in metres relative to the origin of the Irish
    /// Grid.
    /// </summary>
    /// <value>The northing.</value>
    public double Northing 
    {
      get => _northing;
      private set
      {
        if (value < 0.0 || value > 500000.0)
        {
          throw new ArgumentException("Northing (" + value
              + ") is invalid. Must be greater than or equal to 0 and less "
              + "than or equal to 500000");
        }

        _northing = value;
      }
    }

    /// <summary>
    /// Convert this Irish grid reference to an Irish Transverse Mercator (ITM) reference
    /// </summary>
    /// <returns>The Irish Transverse Mercator (ITM) reference</returns>
    public ITMRef ToITMRef()
    {
      var ll = ToLatLng();
      return new ITMRef(ll);
    }
  }
}
