using DotNetCoords.Datum;
using DotNetCoords.Ellipsoid;
using System;

namespace DotNetCoords
{
  /// <summary>
  /// <p>
  /// Class to represent an Ordnance Survey of Great Britain (OSGB) grid reference.
  /// </p>
  /// <p>
  /// 	<b>British National Grid</b><br/>
  /// 	<ul>
  /// 		<li>Projection: Transverse Mercator</li>
  /// 		<li>Reference ellipsoid: Airy 1830</li>
  /// 		<li>Units: metres</li>
  /// 		<li>Origin: 49°N, 2°W</li>
  /// 		<li>False co-ordinates of origin: 400000m east, -100000m north</li>
  ///     <li>EPSG: 27700</li>
  /// 	</ul>
  /// </p>
  /// <p>
  /// A full reference includes a two-character code identifying a particular
  /// 100,000m grid square. The table below shows how the two-character 100,000m
  /// grid squares are identified. The bottom left corner is at the false origin of
  /// the grid. Squares without values fall outside the boundaries of the British
  /// National Grid.
  /// </p>
  /// <table border="1">
  /// 	<tr>
  /// 		<th> km</th>
  /// 		<th>0</th>
  /// 		<th>100</th>
  /// 		<th>200</th>
  /// 		<th>300</th>
  /// 		<th>400</th>
  /// 		<th>500</th>
  /// 		<th>600</th>
  /// 		<th>700</th>
  /// 	</tr>
  /// 	<tr>
  /// 		<th>1200</th>
  /// 		<td>HL</td>
  /// 		<td>HM</td>
  /// 		<td>HN</td>
  /// 		<td>HO</td>
  /// 		<td>HP</td>
  /// 		<td>JL</td>
  /// 		<td>JM</td>
  /// 		<td>
  /// 		</td>
  /// 	</tr>
  /// 	<tr>
  /// 		<th>1100</th>
  /// 		<td>HQ</td>
  /// 		<td>HR</td>
  /// 		<td>HS</td>
  /// 		<td>HT</td>
  /// 		<td>HU</td>
  /// 		<td>JQ</td>
  /// 		<td>JR</td>
  /// 		<td>
  /// 		</td>
  /// 	</tr>
  /// 	<tr>
  /// 		<th>1000</th>
  /// 		<td>HV</td>
  /// 		<td>HW</td>
  /// 		<td>HX</td>
  /// 		<td>HY</td>
  /// 		<td>HZ</td>
  /// 		<td>JV</td>
  /// 		<td>JW</td>
  /// 		<td>
  /// 		</td>
  /// 	</tr>
  /// 	<tr>
  /// 		<th> 900</th>
  /// 		<td>NA</td>
  /// 		<td>NB</td>
  /// 		<td>NC</td>
  /// 		<td>ND</td>
  /// 		<td>NE</td>
  /// 		<td>OA</td>
  /// 		<td>OB</td>
  /// 		<td>
  /// 		</td>
  /// 	</tr>
  /// 	<tr>
  /// 		<th> 800</th>
  /// 		<td>NF</td>
  /// 		<td>NG</td>
  /// 		<td>NH</td>
  /// 		<td>NJ</td>
  /// 		<td>NK</td>
  /// 		<td>OF</td>
  /// 		<td>OG</td>
  /// 		<td>OH</td>
  /// 	</tr>
  /// 	<tr>
  /// 		<th> 700</th>
  /// 		<td>NL</td>
  /// 		<td>NM</td>
  /// 		<td>NN</td>
  /// 		<td>NO</td>
  /// 		<td>NP</td>
  /// 		<td>OL</td>
  /// 		<td>OM</td>
  /// 		<td>ON</td>
  /// 	</tr>
  /// 	<tr>
  /// 		<th> 600</th>
  /// 		<td>NQ</td>
  /// 		<td>NR</td>
  /// 		<td>NS</td>
  /// 		<td>NT</td>
  /// 		<td>NU</td>
  /// 		<td>OQ</td>
  /// 		<td>OR</td>
  /// 		<td>OS</td>
  /// 	</tr>
  /// 	<tr>
  /// 		<th> 500</th>
  /// 		<td>
  /// 		</td>
  /// 		<td>NW</td>
  /// 		<td>NX</td>
  /// 		<td>NY</td>
  /// 		<td>NZ</td>
  /// 		<td>OV</td>
  /// 		<td>OW</td>
  /// 		<td>OX</td>
  /// 	</tr>
  /// 	<tr>
  /// 		<th> 400</th>
  /// 		<td>
  /// 		</td>
  /// 		<td>SB</td>
  /// 		<td>SC</td>
  /// 		<td>SD</td>
  /// 		<td>SE</td>
  /// 		<td>TA</td>
  /// 		<td>TB</td>
  /// 		<td>TC</td>
  /// 	</tr>
  /// 	<tr>
  /// 		<th> 300</th>
  /// 		<td>
  /// 		</td>
  /// 		<td>SG</td>
  /// 		<td>SH</td>
  /// 		<td>SJ</td>
  /// 		<td>SK</td>
  /// 		<td>TF</td>
  /// 		<td>TG</td>
  /// 		<td>TH</td>
  /// 	</tr>
  /// 	<tr>
  /// 		<th> 200</th>
  /// 		<td>
  /// 		</td>
  /// 		<td>SM</td>
  /// 		<td>SN</td>
  /// 		<td>SO</td>
  /// 		<td>SP</td>
  /// 		<td>TL</td>
  /// 		<td>TM</td>
  /// 		<td>TN</td>
  /// 	</tr>
  /// 	<tr>
  /// 		<th> 100</th>
  /// 		<td>SQ</td>
  /// 		<td>SR</td>
  /// 		<td>SS</td>
  /// 		<td>ST</td>
  /// 		<td>SU</td>
  /// 		<td>TQ</td>
  /// 		<td>TR</td>
  /// 		<td>TS</td>
  /// 	</tr>
  /// 	<tr>
  /// 		<th> 0</th>
  /// 		<td>SV</td>
  /// 		<td>SW</td>
  /// 		<td>SX</td>
  /// 		<td>SY</td>
  /// 		<td>SZ</td>
  /// 		<td>TV</td>
  /// 		<td>TW</td>
  /// 		<td>
  /// 		</td>
  /// 	</tr>
  /// </table>
  /// <p>
  /// Within each 100,000m square, the grid is further subdivided into 1000m
  /// squares. These 1km squares are shown on Ordnance Survey 1:25000 and 1:50000
  /// mapping as the main grid. To reference a 1km square, give the easting and
  /// then the northing, e.g. TR2266. In this example, TR represents the 100,000m
  /// square, 22 represents the easting (in km) and 66 represents the northing (in
  /// km). This is commonly called a four-figure grid reference.
  /// </p>
  /// <p>
  /// It is possible to extend the four-figure grid reference for more accuracy.
  /// For example, a six-figure grid reference would be accurate to 100m and an
  /// eight-figure grid reference would be accurate to 10m.
  /// </p>
  /// <p>
  /// When providing local references, the 2 characters representing the 100,000m
  /// square are often omitted.
  /// </p>
  /// </summary>
  public sealed class OSRef : CoordinateSystem 
  {
    /**
     * The easting in metres relative to the origin of the British National Grid.
     */
    private double _easting;

    /**
     * The northing in metres relative to the origin of the British National Grid.
     */
    private double _northing;


    /// <summary>
    /// Create a new Ordnance Survey grid reference using a given easting and
    /// northing. The easting and northing must be in metres and must be relative
    /// to the origin of the British National Grid.
    /// </summary>
    /// <param name="easting">The easting in metres. Must be greater than or equal to 0.0 and
    /// less than 800000.0.</param>
    /// <param name="northing">The northing in metres. Must be greater than or equal to 0.0 and
    /// less than 1400000.0.</param>
    /// <exception cref="ArgumentException">If the northing or easting are out of range</exception>
    public OSRef(double easting, double northing)
      : base(OSGB36Datum.Instance)
    {
      Easting = easting;
      Northing = northing;
    }

    /// <summary>
    /// Take a string formatted as a 6, 8 or 10 figure OS grid reference (e.g. "TG514131")
    /// and create a new OSRef object that represents that grid reference. The
    /// first character must be H, N, S, O or T. The second character can be any
    /// uppercase character from A through Z excluding I.
    /// </summary>
    /// <param name="gridRef">a string representing a 6, 8 or 10 figure Ordnance Survey grid reference
    /// in the form XY123456.</param>
    /// <exception cref="ArgumentException">If the northing or easting are out of range</exception>
    public OSRef(string gridRef) : base(OSGB36Datum.Instance)
    {
      int multiplier;
      int length;
      switch (gridRef.Length)
      {
        case 8: 
          multiplier = 100;
          length = 3;
          break;
        case 10: 
          multiplier = 10;
          length = 4;
          break;
        case 12: 
          multiplier = 1;
          length = 5;
          break;
        default: throw new ArgumentException("Unexpected length of grid reference (should be 8, 10 or 12 characters long)", nameof(gridRef));
      }

      var char1 = gridRef[0];
      var char2 = gridRef[1];
      // Thanks to Nick Holloway for pointing out the radix bug here
      var east = int.Parse(gridRef.Substring(2, length)) * multiplier;
      var north = int.Parse(gridRef.Substring(2+length, length)) * multiplier;
      
      switch (char1)
      {
        case 'H':
          north += 1000000;
          break;
        case 'N':
          north += 500000;
          break;
        case 'O':
          north += 500000;
          east += 500000;
          break;
        case 'T':
          east += 500000;
          break;
      }
      int char2ord = char2;
      if (char2ord > 73)
        char2ord--; // Adjust for no I
      double nx = ((char2ord - 65) % 5) * 100000;
      var ny = (4 - Math.Floor((double)(char2ord - 65) / 5)) * 100000;

      Easting = east + nx;
      Northing = north + ny;
    }

    /// <summary>
    /// Convert this latitude and longitude into an OSGB (Ordnance Survey of Great
    /// Britain) grid reference.
    /// </summary>
    /// <param name="ll">The latitude and longitude.</param>
    /// <exception cref="ArgumentException">If the northing or easting are out of range</exception>
    public OSRef(LatLng ll) : base(OSGB36Datum.Instance)
    {
      var en = LatLngToEastingNorthing(ll, -100000.0, 400000.0, 49.0, -2.0, 0.9996012717);
      Easting = en.Easting;
      Northing = en.Northing;
    }

    /// <summary>
    /// Return a string representation of this OSGB grid reference showing the
    /// easting and northing.
    /// </summary>
    /// <returns>
    /// a string representation of this OSGB grid reference.
    /// </returns>
    public override string ToString() 
    {
      return "(" + _easting + ", " + _northing + ")";
    }

    /// <summary>
    /// Return a string representation of this OSGB grid reference using the
    /// ten-figure notation in the form XY 12345 67890.
    /// </summary>
    /// <returns>A string representing this OSGB grid reference in ten-figure
    /// notation</returns>
    public string ToTenFigureString()
    {
      var e = (int)Math.Floor((_easting - (100000 * HundredKmEast())));
      var n = (int)Math.Floor((_northing - (100000 * HundredKmNorth())));

      var es = e.ToString("D5");
      var ns = n.ToString("D5");

      return GetGridReferenceLetters() + " " + es + " " + ns;
    }

    /// <summary>
    /// Return a string representation of this OSGB grid reference using the
    /// six-figure notation in the form XY123456.
    /// </summary>
    /// <returns>A string representing this OSGB grid reference in six-figure
    /// notation</returns>
    public string ToSixFigureString() 
    {
      var e = (int)Math.Floor((_easting - (100000 * HundredKmEast())) / 100);
      var n = (int)Math.Floor((_northing - (100000 * HundredKmNorth())) / 100);
      
      var es = e.ToString("D3");
      var ns = n.ToString("D3");

      return GetGridReferenceLetters() + es + ns;
    }

    private int HundredKmEast()
    {
      return (int)Math.Floor(_easting / 100000);
    }

    private int HundredKmNorth()
    {
      return (int)Math.Floor(_northing / 100000);
    }

    private string GetGridReferenceLetters()
    {
      string firstLetter;
      if (HundredKmNorth() < 5)
      {
        if (HundredKmEast() < 5)
        {
          firstLetter = "S";
        }
        else
        {
          firstLetter = "T";
        }
      }
      else if (HundredKmNorth() < 10)
      {
        if (HundredKmEast() < 5)
        {
          firstLetter = "N";
        }
        else
        {
          firstLetter = "O";
        }
      }
      else
      {
        firstLetter = "H";
      }

      var index = 65 + ((4 - (HundredKmNorth() % 5)) * 5) + (HundredKmEast() % 5);
      
      if (index >= 73)
        index++;
      var secondLetter = ((char)index).ToString();

      return firstLetter + secondLetter;
    }

    /// <summary>
    /// Convert this OSGB grid reference to a latitude/longitude pair using the
    /// OSGB36 datum. Note that, the LatLng object may need to be converted to the
    /// WGS84 datum depending on the application.
    /// </summary>
    /// <returns>
    /// A LatLng object representing this OSGB grid reference using the
    /// OSGB36 datum
    /// </returns>
    public override LatLng ToLatLng()
    {
      return ToLatLng(Easting, Northing, -100000.0, 400000.0, 49.0, -2.0, 0.9996012717);
    }

    /// <summary>
    /// Gets the easting in metres relative to the origin of the British National Grid..
    /// </summary>
    /// <value>The easting.</value>
    /// <exception cref="ArgumentException">If the easting is out of range</exception>
    public double Easting 
    {
      get => _easting;
      private set
      {
        if (value < 0.0 || value >= 800000.0)
        {
          throw new ArgumentException("Easting (" + value
              + ") is invalid. Must be greater than or equal to 0.0 and "
              + "less than 800000.0.");
        }

        _easting = value;
      }
    }

    /// <summary>
    /// Gets the northing in metres relative to the origin of the British National
    /// Grid.
    /// </summary>
    /// <value>The northing.</value>
    /// <exception cref="ArgumentException">If the northing is out of range</exception>
    public double Northing 
    {
      get => _northing;
      private set
      {
        if (value < 0.0 || value >= 1400000.0)
        {
          throw new ArgumentException("Northing (" + value
              + ") is invalid. Must be greater than or equal to 0.0 and less "
              + "than 1400000.0.");
        }

        _northing = value;
      }
    }
  }
}
