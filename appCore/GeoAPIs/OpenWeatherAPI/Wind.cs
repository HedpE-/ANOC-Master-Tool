using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenWeatherAPI
{

    public class Wind
    {
        public enum DirectionEnum
        {
            North,
            North_North_East,
            North_East,
            East_North_East,
            East,
            East_South_East,
            South_East,
            South_South_East,
            South,
            South_South_West,
            South_West,
            West_South_West,
            West,
            West_North_West,
            North_West,
            North_North_West,
            Unknown
        }

        public double speed { get; set; }
        public double deg { get; set; }

        [JsonIgnore]
        public double SpeedMetersPerSecond { get { return speed; } }
        [JsonIgnore]
        public double SpeedFeetPerSecond { get { return speed * 3.28084; } }
        [JsonIgnore]
        public DirectionEnum Direction
        {
            get
            {
                try { return assignDirection(deg); }
                catch { return DirectionEnum.Unknown; }
            }
        }
        [JsonIgnore]
        System.Drawing.Image picture;
        [JsonIgnore]
        public System.Drawing.Image DirectionPicture
        {
            get
            {
                if (picture == null)
                {
                    if(appCore.Settings.GlobalProperties.ExternalResourceFilesLocation.Exists)
                    {
                        if(appCore.Settings.GlobalProperties.ExternalResourceFilesLocation.GetFiles("wind-dart-*.png").Length > 0)
                        {
                            switch (Direction)
                            {
                                case DirectionEnum.North:
                                    picture = System.Drawing.Image.FromFile(appCore.Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\wind-dart-white-1.png");
                                    break;
                                case DirectionEnum.North_North_East:
                                case DirectionEnum.North_East:
                                case DirectionEnum.East_North_East:
                                    picture = System.Drawing.Image.FromFile(appCore.Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\wind-dart-white-5.png");
                                    break;
                                case DirectionEnum.East:
                                    picture = System.Drawing.Image.FromFile(appCore.Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\wind-dart-white-2.png");
                                    break;
                                case DirectionEnum.East_South_East:
                                case DirectionEnum.South_East:
                                case DirectionEnum.South_South_East:
                                    picture = System.Drawing.Image.FromFile(appCore.Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\wind-dart-white-6.png");
                                    break;
                                case DirectionEnum.South:
                                    picture = System.Drawing.Image.FromFile(appCore.Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\wind-dart-white-3.png");
                                    break;
                                case DirectionEnum.South_South_West:
                                case DirectionEnum.South_West:
                                case DirectionEnum.West_South_West:
                                    picture = System.Drawing.Image.FromFile(appCore.Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\wind-dart-white-7.png");
                                    break;
                                case DirectionEnum.West:
                                    picture = System.Drawing.Image.FromFile(appCore.Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\wind-dart-white-4.png");
                                    break;
                                case DirectionEnum.West_North_West:
                                case DirectionEnum.North_West:
                                case DirectionEnum.North_North_West:
                                    picture = System.Drawing.Image.FromFile(appCore.Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + @"\wind-dart-white-8.png");
                                    break;
                            }
                        }
                    }
                }
                return picture;
            }
        }

        public string directionEnumToString(DirectionEnum dir)
        {
            switch (dir)
            {
                case DirectionEnum.East:
                    return "East";
                case DirectionEnum.East_North_East:
                    return "East North-East";
                case DirectionEnum.East_South_East:
                    return "East South-East";
                case DirectionEnum.North:
                    return "North";
                case DirectionEnum.North_East:
                    return "North East";
                case DirectionEnum.North_North_East:
                    return "North North-East";
                case DirectionEnum.North_North_West:
                    return "North North-West";
                case DirectionEnum.North_West:
                    return "North West";
                case DirectionEnum.South:
                    return "South";
                case DirectionEnum.South_East:
                    return "South East";
                case DirectionEnum.South_South_East:
                    return "South South-East";
                case DirectionEnum.South_South_West:
                    return "South South-West";
                case DirectionEnum.South_West:
                    return "South West";
                case DirectionEnum.West:
                    return "West";
                case DirectionEnum.West_North_West:
                    return "West North-West";
                case DirectionEnum.West_South_West:
                    return "West South-West";
                case DirectionEnum.Unknown:
                    return "Unknown";
                default:
                    return "Unknown";
            }
        }

        private DirectionEnum assignDirection(double degree)
        {
            if (fB(degree, 348.75, 360))
                return DirectionEnum.North;
            if (fB(degree, 0, 11.25))
                return DirectionEnum.North;
            if (fB(degree, 11.25, 33.75))
                return DirectionEnum.North_North_East;
            if (fB(degree, 33.75, 56.25))
                return DirectionEnum.North_East;
            if (fB(degree, 56.25, 78.75))
                return DirectionEnum.East_North_East;
            if (fB(degree, 78.75, 101.25))
                return DirectionEnum.East;
            if (fB(degree, 101.25, 123.75))
                return DirectionEnum.East_South_East;
            if (fB(degree, 123.75, 146.25))
                return DirectionEnum.South_East;
            if (fB(degree, 168.75, 191.25))
                return DirectionEnum.South;
            if (fB(degree, 191.25, 213.75))
                return DirectionEnum.South_South_West;
            if (fB(degree, 213.75, 236.25))
                return DirectionEnum.South_West;
            if (fB(degree, 236.25, 258.75))
                return DirectionEnum.West_South_West;
            if (fB(degree, 258.75, 281.25))
                return DirectionEnum.West;
            if (fB(degree, 281.25, 303.75))
                return DirectionEnum.West_North_West;
            if (fB(degree, 303.75, 326.25))
                return DirectionEnum.North_West;
            if (fB(degree, 326.25, 348.75))
                return DirectionEnum.North_North_West;
            return DirectionEnum.Unknown;
        }

        //fB = fallsBetween
        private bool fB(double val, double min, double max)
        {
            if ((min <= val) && (val <= max))
                return true;
            return false;
        }
    }
}
