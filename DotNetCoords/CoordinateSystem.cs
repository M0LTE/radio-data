using System;

namespace DotNetCoords
{
  internal sealed class EastingNorthing
  {
    public EastingNorthing(double easting, double northing)
    {
      Easting = easting;
      Northing = northing;
    }

    public double Easting { get; }
    public double Northing { get; }
  }

  /// <summary>
  /// Base class for classes defining co-ordinate systems.
  /// </summary>
  public abstract class CoordinateSystem 
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CoordinateSystem"/> class.
    /// </summary>
    /// <param name="datum">The datum.</param>
    protected CoordinateSystem(Datum.Datum datum)
    {
      Datum = datum;
    }

    /// <summary>
    /// Convert a co-ordinate in the co-ordinate system to a point represented
    /// by a latitude and longitude and a perpendicular height above (or below) a
    /// reference ellipsoid.
    /// </summary>
    /// <returns>A LatLng representation of a point in a co-ordinate system.</returns>
    public abstract LatLng ToLatLng();

    /// <summary>
    /// Internal lat/long conversion
    /// </summary>
    /// <param name="easting"></param>
    /// <param name="northing"></param>
    /// <param name="falseOriginNorthing"></param>
    /// <param name="falseOriginEasting"></param>
    /// <param name="falseOriginLatitude"></param>
    /// <param name="falseOriginLongitude"></param>
    /// <param name="scaleFactor"></param>
    /// <returns></returns>
    internal LatLng ToLatLng(double easting, double northing, double falseOriginNorthing, double falseOriginEasting, double falseOriginLatitude, double falseOriginLongitude,
      double scaleFactor)
    {
      var n0 = falseOriginNorthing;
      var e0 = falseOriginEasting;
      var phi0 = Util.ToRadians(falseOriginLatitude);
      var lambda0 = Util.ToRadians(falseOriginLongitude);

      var a = Datum.ReferenceEllipsoid.SemiMajorAxis;
      var b = Datum.ReferenceEllipsoid.SemiMinorAxis;
      var eSquared = Datum.ReferenceEllipsoid.EccentricitySquared;
      var e = easting;
      var N = northing;
      var n = (a - b) / (a + b);
      double m;
      var phiPrime = ((N - n0) / (a * scaleFactor)) + phi0;
      do
      {
        m = (b * scaleFactor)
            * (((1 + n + ((5.0 / 4.0) * n * n) + ((5.0 / 4.0) * n * n * n)) * (phiPrime - phi0))
                - (((3 * n) + (3 * n * n) + ((21.0 / 8.0) * n * n * n))
                    * Math.Sin(phiPrime - phi0) * Math.Cos(phiPrime + phi0))
                + ((((15.0 / 8.0) * n * n) + ((15.0 / 8.0) * n * n * n))
                    * Math.Sin(2.0 * (phiPrime - phi0)) * Math
                    .Cos(2.0 * (phiPrime + phi0))) - (((35.0 / 24.0) * n * n * n)
                * Math.Sin(3.0 * (phiPrime - phi0)) * Math
                .Cos(3.0 * (phiPrime + phi0))));
        phiPrime += (N - n0 - m) / (a * scaleFactor);
      } while ((N - n0 - m) >= 0.001);
      var v = a * scaleFactor
          * Math.Pow(1.0 - eSquared * Util.SinSquared(phiPrime), -0.5);
      var rho = a * scaleFactor * (1.0 - eSquared)
          * Math.Pow(1.0 - eSquared * Util.SinSquared(phiPrime), -1.5);
      var etaSquared = (v / rho) - 1.0;
      var vii = Math.Tan(phiPrime) / (2 * rho * v);
      var viii = (Math.Tan(phiPrime) / (24.0 * rho * Math.Pow(v, 3.0)))
          * (5.0 + (3.0 * Util.TanSquared(phiPrime)) + etaSquared - (9.0 * Util
              .TanSquared(phiPrime) * etaSquared));
      var ix = (Math.Tan(phiPrime) / (720.0 * rho * Math.Pow(v, 5.0)))
          * (61.0 + (90.0 * Util.TanSquared(phiPrime)) + (45.0 * Util
              .TanSquared(phiPrime) * Util.TanSquared(phiPrime)));
      var x = Util.Sec(phiPrime) / v;
      var xi = (Util.Sec(phiPrime) / (6.0 * v * v * v))
          * ((v / rho) + (2 * Util.TanSquared(phiPrime)));
      var xii = (Util.Sec(phiPrime) / (120.0 * Math.Pow(v, 5.0)))
          * (5.0 + (28.0 * Util.TanSquared(phiPrime)) + (24.0 * Util
              .TanSquared(phiPrime) * Util.TanSquared(phiPrime)));
      var xiia = (Util.Sec(phiPrime) / (5040.0 * Math.Pow(v, 7.0)))
          * (61.0 + (662.0 * Util.TanSquared(phiPrime))
              + (1320.0 * Util.TanSquared(phiPrime) * Util.TanSquared(phiPrime)) + (720.0
              * Util.TanSquared(phiPrime) * Util.TanSquared(phiPrime) * Util
              .TanSquared(phiPrime)));
      var phi = phiPrime - (vii * Math.Pow(e - e0, 2.0))
                   + (viii * Math.Pow(e - e0, 4.0)) - (ix * Math.Pow(e - e0, 6.0));
      var lambda = lambda0 + (x * (e - e0)) - (xi * Math.Pow(e - e0, 3.0))
                      + (xii * Math.Pow(e - e0, 5.0)) - (xiia * Math.Pow(e - e0, 7.0));

      return new LatLng(Util.ToDegrees(phi), Util.ToDegrees(lambda), 0, Datum);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal EastingNorthing LatLngToEastingNorthing(LatLng ll, double falseOriginNorthing, double falseOriginEasting, double falseOriginLatitude, double falseOriginLongitude,
      double scaleFactor)
    {
      // take a copy of the LatLng so we don't modify the original LatLng
      var llc = new LatLng(ll);
      llc.ToDatum(Datum);

      var n0 = falseOriginNorthing;
      var e0 = falseOriginEasting;
      var phi0 = Util.ToRadians(falseOriginLatitude);
      var lambda0 = Util.ToRadians(falseOriginLongitude);
      var a = Datum.ReferenceEllipsoid.SemiMajorAxis;
      var b = Datum.ReferenceEllipsoid.SemiMinorAxis;
      var eSquared = Datum.ReferenceEllipsoid.EccentricitySquared;

      var phi = Util.ToRadians(llc.Latitude);
      var lambda = Util.ToRadians(llc.Longitude);
      var n = (a - b) / (a + b);
      var v = a * scaleFactor
                * Math.Pow(1.0 - eSquared * Util.SinSquared(phi), -0.5);
      var rho = a * scaleFactor * (1.0 - eSquared)
                * Math.Pow(1.0 - eSquared * Util.SinSquared(phi), -1.5);
      var etaSquared = (v / rho) - 1.0;
      var m = (b * scaleFactor)
              * (((1 + n + ((5.0 / 4.0) * n * n) + ((5.0 / 4.0) * n * n * n)) * (phi - phi0))
                - (((3 * n) + (3 * n * n) + ((21.0 / 8.0) * n * n * n))
                   * Math.Sin(phi - phi0) * Math.Cos(phi + phi0))
                + ((((15.0 / 8.0) * n * n) + ((15.0 / 8.0) * n * n * n))
                   * Math.Sin(2.0 * (phi - phi0)) * Math.Cos(2.0 * (phi + phi0))) - (((35.0 / 24.0)
                    * n * n * n)
                  * Math.Sin(3.0 * (phi - phi0)) * Math.Cos(3.0 * (phi + phi0))));
      var I = m + n0;
      var ii = (v / 2.0) * Math.Sin(phi) * Math.Cos(phi);
      var iii = (v / 24.0) * Math.Sin(phi) * Math.Pow(Math.Cos(phi), 3.0)
          * (5.0 - Util.TanSquared(phi) + (9.0 * etaSquared));
      var iiia = (v / 720.0) * Math.Sin(phi) * Math.Pow(Math.Cos(phi), 5.0)
          * (61.0 - (58.0 * Util.TanSquared(phi)) + Math.Pow(Math.Tan(phi), 4.0));
      var iv = v * Math.Cos(phi);
      var V = (v / 6.0) * Math.Pow(Math.Cos(phi), 3.0)
          * ((v / rho) - Util.TanSquared(phi));
      var vi = (v / 120.0)
          * Math.Pow(Math.Cos(phi), 5.0)
          * (5.0 - (18.0 * Util.TanSquared(phi)) + (Math.Pow(Math.Tan(phi), 4.0))
              + (14 * etaSquared) - (58 * Util.TanSquared(phi) * etaSquared));

      var N = I + (ii * Math.Pow(lambda - lambda0, 2.0))
                 + (iii * Math.Pow(lambda - lambda0, 4.0))
                 + (iiia * Math.Pow(lambda - lambda0, 6.0));
      var e = e0 + (iv * (lambda - lambda0)) + (V * Math.Pow(lambda - lambda0, 3.0))
                 + (vi * Math.Pow(lambda - lambda0, 5.0));

      return new EastingNorthing(e, N);
    }

    /// <summary>
    /// Gets the datum.
    /// </summary>
    /// <value>The datum.</value>
    public Datum.Datum Datum { get; }
  }
}
