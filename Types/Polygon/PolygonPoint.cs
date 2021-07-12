﻿using ssi.Interfaces;
using ssi.Types.Polygon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    public class PolygonPoint
    {
        private double x;
        private double y;
        private double pointID;
        private static List<double> allIDs = new List<double>();

        public PolygonPoint(double x, double y, double pointID)
        {
            checkID(pointID);

            this.x = x;
            this.y = y;
            this.pointID = pointID;
        }
        public PolygonPoint(double x, double y)
        {
            double id = PolygonUtilities.IDcounter;
            PolygonUtilities.IDcounter++;

            this.x = x;
            this.y = y;
            this.pointID = id;
        }

        

        private void checkID(double pointID)
        {
            if (allIDs.Contains(pointID))
                throw new Exception("This id is already taken!");
            else
                allIDs.Add(pointID);
        }

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public double PointID { get => pointID; }

        public override bool Equals(object obj)
        {
            PolygonPoint point = obj as PolygonPoint;

            if (point == null)
                return false;
            
            return point.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            int hashCode = 198087538;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            hashCode = hashCode * -1521134295 + PointID.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}