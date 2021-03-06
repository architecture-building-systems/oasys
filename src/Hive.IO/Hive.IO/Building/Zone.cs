﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using rg = Rhino.Geometry;

namespace Hive.IO.Building
{

    /// <summary>
    /// Thermal Zone.
    /// Geometry must be (1) Brep, (2) closed, (3) convex, and (4) not contain any curves, i.e. lines only.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Zone
    {
        #region Main Properties
        /// <summary>
        /// The actual zone geometry, as rhino Brep
        /// </summary>
        [JsonProperty]
        public rg.Brep ZoneGeometry { get; private set; }
        /// <summary>
        /// Unique index, used to identify the zone when it is part of a Building object
        /// </summary>
        [JsonProperty]
        public int Index { get; private set; }

        /// <summary>
        /// Zone volume in [m^3]
        /// </summary>
        [JsonProperty]
        public double Volume { get; private set; }

        /// <summary>
        /// Tolerance for geometric operations. Get from RhinoDoc.ModelAbsoluteTolerance?
        /// </summary>
        [JsonProperty]
        public double Tolerance { get; private set; }
        /// <summary>
        /// fix the horizon to one year, hourly
        /// </summary>
        private const int _horizon = 8760;
        private const int _HoursPerDay = 24;
        #endregion


        #region Loads And Schedules
        /// <summary>
        /// Zone name, e.g. 'Kitchen01'
        /// </summary>
        [JsonProperty]
        public string Name { get; private set; }
        /// <summary>
        /// Internal loads.
        /// Values indicate full / maximal value.
        /// Unit in W/sqm
        /// </summary>
        public struct StructInternalLoads
        {
            public double Occupants;
            public double Devices;
            public double Lighting;
        }
        /// <summary>
        /// Internal loads structure. Values in W/m2
        /// </summary>
        [JsonProperty]
        public StructInternalLoads InternalLoads;

        /// <summary>
        /// Processed schedules from SIA 2024.
        /// </summary>
        public ZoneSchedules Schedules;

        #endregion


        #region Building Components

        public IEnumerable<Component> SurfaceComponents =>
            Walls.Cast<Component>().Concat(Ceilings).Concat(Roofs).Concat(Floors).Concat(Windows);

        /// <summary>
        /// Wall components of this zone. Cannot be empty.
        /// </summary>
        [JsonProperty]
        public Wall[] Walls { get; private set; }
        /// <summary>
        /// Ceiling components of this zone. Cannot be empty.
        /// </summary>
        [JsonProperty]
        public Ceiling[] Ceilings { get; private set; }
        /// <summary>
        /// Floor components of this zone. Cannot be empty. A void would also be a floor, but with material property 'air' or something
        /// </summary>
        [JsonProperty]
        public Floor[] Floors { get; private set; }
        /// <summary>
        /// Apertures of this zone, e.g. windows, skylights, doors, ventilation openings, etc.. Can be empty.
        /// </summary>
        [JsonProperty]
        public Window[] Windows { get; private set; }
        /// <summary>
        /// Roof components of this zone. Can be empty.
        /// </summary>
        [JsonProperty]
        public Roof[] Roofs { get; private set; }

        /// <summary>
        /// Shading devices
        /// </summary>
        [JsonProperty]
        public Shading[] ShadingDevices { get; private set; }

        public double WallArea => Walls.Sum(w => w.Area);
        public double RoofArea => Roofs.Sum(r => r.Area);
        public double WindowArea => Windows.Sum(w => w.Area);
        public double FloorArea => Floors.Sum(f => f.Area);

        #endregion


        #region Energy Demand
        /// <summary>
        /// in kWh per month
        /// </summary>
        [JsonProperty]
        public double[] HeatingLoadsMonthly { get; private set; }
        /// <summary>
        /// in kWh per hour
        /// </summary>
        [JsonProperty]
        public double [] HeatingLoadsHourly { get; private set; }

        /// <summary>
        /// in kWh per month
        /// </summary>
        [JsonProperty]
        public double[] DHWLoadsMonthly { get; private set; }
        /// <summary>
        /// in kWh per hour
        /// </summary>
        [JsonProperty]
        public double [] DHWLoadsHourly { get; private set; }
        /// <summary>
        /// in kWh per month
        /// </summary>
        [JsonProperty]
        public double[] CoolingLoadsMonthly { get; private set; }
        /// <summary>
        /// in kWh per hour
        /// </summary>
        [JsonProperty]
        public double [] CoolingLoadsHourly { get; private set; }
        /// <summary>
        /// in kWh per month
        /// </summary>
        [JsonProperty]
        public double[] ElectricityLoadsMonthly { get; private set; }
        /// <summary>
        /// in kWh per hour
        /// </summary>
        [JsonProperty]
        public double [] ElectricityLoadsHourly { get; private set; }

        /// <summary>
        /// Differs from ElectricityLoadsMonthly, which can be negative (surplus electricity from e.g. PV). ConsumedElectricityMonthly is what we really consume in the zone
        /// </summary>
        [JsonProperty]
        public double [] ConsumedElectricityMonthly { get; set; } // set in GhResults... FIX ME
        [JsonProperty]
        public double [] ConsumedHeatingMonthly { get; set; } // same

        /// <summary>
        /// Determines if adaptive comfort should be used instead of setpoints from SIA 2024.
        /// </summary>
        public bool RunAdaptiveComfort { get; set; }

        #endregion


        #region Losses and Gains
        [JsonProperty]
        public double[] OpaqueTransmissionHeatLossesMonthly { get; private set; }
        [JsonProperty]
        public double[] OpaqueTransmissionHeatLossesHourly { get; private set; }
        [JsonProperty]
        public double [] TransparentTransmissionHeatLossesMonthly { get; private set; }
        [JsonProperty]
        public double[] TransparentTransmissionHeatLossesHourly { get; private set; }
        [JsonProperty]
        public double[] VentilationHeatLossesMonthly { get; private set; }
        [JsonProperty]
        public double[] VentilationHeatLossesHourly { get; private set; }
        [JsonProperty]
        public double[] InternalHeatGainsMonthly { get; private set; }
        [JsonProperty]
        public double[] InternalHeatGainsHourly { get; private set; }
        [JsonProperty]
        public double[] SolarGainsMonthly { get; private set; }
        [JsonProperty]
        public double[] SolarGainsHourly { get; private set; }
        [JsonProperty]
        public double [][] SolarGainsPerWindowMonthly { get; private set; }
        [JsonProperty]
        public double[][] SolarGainsPerWindowHourly { get; private set; }
        #endregion


        #region Error handling
        /// <summary>
        /// For simplicity of thermal calculations, avoid curves etc., only accept linear floorplans and geometries
        /// </summary>
        [JsonProperty]
        public bool IsLinear { get; private set; }
        /// <summary>
        /// For simplicity of thermal calculations, only accept convex zones
        /// </summary>
        [JsonProperty]
        public bool IsConvex { get; private set; }
        /// <summary>
        /// Zone geometry must be a closed Brep, since it defines a thermal space
        /// </summary>
        [JsonProperty]
        public bool IsClosed { get; private set; }
        /// <summary>
        /// Check planarity of all surfaces. Must be for simplicity
        /// </summary>
        [JsonProperty]
        public bool IsPlanar { get; private set; }
        /// <summary>
        /// Main bool, if this is false, then no thermal simulations can be done
        /// </summary>
        [JsonProperty]
        public bool IsValid { get; private set; }
        /// <summary>
        /// Stricter validity check for EnergyPlus. Can still perform SIA and RC simulations
        /// </summary>
        [JsonProperty]
        public bool IsValidEPlus { get; private set; }
        /// <summary>
        /// Checking whether window surfaces (if any exist) are lying on the zone geometry. 
        /// Window surfaces associated to a zone cannot just lie somewhere else.
        /// </summary>
        [JsonProperty]
        public bool IsWindowsOnZone { get; private set; }
        /// <summary>
        /// Windows self intersection
        /// </summary>
        [JsonProperty]
        public bool IsWindowsCollisionFree { get; private set; }
        /// <summary>
        /// For additional floor surfaces, they have to be within the volume
        /// </summary>
        [JsonProperty]
        public bool IsFloorInZone { get; private set; }
        /// <summary>
        /// Checking if a floor has been detected. No load simulations possible without floor.
        /// </summary>
        [JsonProperty]
        public bool IsFloorExist { get; private set; }
        [JsonProperty]
        public string ErrorText { get; private set; }
        #endregion

        [JsonConstructor]
        internal Zone()
        {
            // only for use in deserialization
        }

        #region Constructor
        /// <summary>
        /// Create an instance of a Zone object
        /// </summary>
        /// <param name="zone_geometry">Brep geometry. Must be closed, linear and convex.</param>
        /// <param name="index">Unique identifier</param>
        /// <param name="tolerance"></param>
        /// <param name="roomType">Zone name, e.g. kitchen 1</param>
        /// <param name="windowSrfs"></param>
        /// <param name="floorSrfs"></param>
        /// <param name="shadingSrfs"></param>
        public Zone(rg.Brep zone_geometry, int index, double tolerance, string roomType, rg.BrepFace[] windowSrfs = null, rg.BrepFace[] floorSrfs = null, rg.BrepFace[] shadingSrfs = null)
        {
            if (zone_geometry == null)
            {
                throw new NullReferenceException("Zone BREP cannot be null.");
            }
            this.ZoneGeometry = zone_geometry;
            this.Index = index;
            this.Tolerance = tolerance;

            // check if floor is in zone
            var floorList = new List<rg.BrepFace>();
            if (floorSrfs != null && floorSrfs.Length > 0)
            {
                foreach (var floor in floorSrfs)
                {
                    if (CheckFloorInZone(zone_geometry, floor))
                        floorList.Add(floor);
                    else
                        this.IsFloorInZone = false;
                }
            }

            // we want to have these checks to be all 'true'. Some are optional, though. Defined later in this.IsValid
            // only IsClosed needs to be strictly guaranteed in all cases
            this.IsClosed = false;
            this.IsConvex = false;
            this.IsPlanar = false;
            this.IsWindowsOnZone = true; // zone might have no windows. so default is true
            this.IsWindowsCollisionFree = true;
            this.IsFloorInZone = true;
            this.IsFloorExist = true;

            this.IsClosed = CheckClosedness(this.ZoneGeometry);

            if (this.IsClosed)
            {
                this.IsLinear = CheckLinearity(this.ZoneGeometry);
                this.IsPlanar = CheckPlanarity(this.ZoneGeometry);
                this.IsConvex = CheckConvexity(this.ZoneGeometry, this.Tolerance);
            }


            // identify building components based on their surface angles
            Tuple<Wall[], Ceiling[], Roof[], Floor[], Window[], Shading[]> tuple = IdentifyComponents(zone_geometry, windowSrfs, shadingSrfs);
            this.Walls = tuple.Item1;
            this.Ceilings = tuple.Item2;
            this.Roofs = tuple.Item3;
            this.Floors = new Floor[floorList.Count + tuple.Item4.Length];
            int mainFloors = tuple.Item4.Length;
            int additionalFloors = floorList.Count;
            for (int i = 0; i < mainFloors; i++)
                this.Floors[i] = tuple.Item4[i];
            for (int i = mainFloors; i < mainFloors + additionalFloors; i++)
                this.Floors[i] = new Floor(floorList[i - mainFloors]);
            this.Windows = tuple.Item5;
            this.ShadingDevices = tuple.Item6;
            

            // check, if floor is detected
            this.IsFloorExist = !(this.Floors.Length <= 0);

            // check window surfaces. Also assign them as subsurface to a wall
            if (windowSrfs != null && windowSrfs.Length > 0)
            {
                this.IsWindowsOnZone = CheckWindowsOnZone(this.ZoneGeometry, windowSrfs, this.Tolerance);
                this.IsWindowsCollisionFree = CheckWindowsIsFreeOfIntersections(windowSrfs, this.Tolerance);
            }

            this.IsValidEPlus = CheckValidity(this.IsClosed, this.IsConvex, this.IsLinear, this.IsPlanar, this.IsWindowsOnZone, this.IsWindowsCollisionFree);
            this.IsValid = (this.IsClosed && this.IsWindowsOnZone && this.IsWindowsCollisionFree && this.IsFloorExist) ? true : false;
            this.ErrorText = String.Format("Edges are linear: {0} \n " + "Zone is convex: {1} \n " + "Zone geometry is a closed polysurface: {2} \n " + "Zone surfaces are planar: {3} \n " 
                                           + "Windows lie on zone surfaces: {4} \n " + "Windows have no self intersection: {5} \n " 
                                           + "Additional floor surfaces lie within zone geometry: {6} \n " + "Floor surface detected: {7}", 
                this.IsLinear, this.IsConvex, this.IsClosed, this.IsPlanar, this.IsWindowsOnZone, this.IsWindowsCollisionFree, this.IsFloorInZone, this.IsFloorExist);

            // define standard building physical properties upon inizialization. 
            // Can be changed later via Windows Form

            if (this.IsValid)
            {
                this.Volume = zone_geometry.GetVolume();
                this.Name = roomType;
                this.InternalLoads.Occupants = 16.0;
                this.InternalLoads.Lighting = 4.0;
                this.InternalLoads.Devices = 3.0;
                this.Schedules = Sia2024Schedules.Lookup(roomType);
            }
        }
        #endregion


        #region Setters


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Q_s_per_window"></param>
        public void SetWindowIrradiance(double[][] Q_s_per_window)
        {
            this.SolarGainsPerWindowMonthly = new double[Q_s_per_window.Length][];
            this.SolarGainsPerWindowHourly = new double[Q_s_per_window.Length][];
            for (int i = 0; i < Q_s_per_window.Length; i++)
            {
                this.SolarGainsPerWindowMonthly[i] = new double[Misc.MonthsPerYear];
                if (Q_s_per_window[i].Length == Misc.HoursPerYear)
                {
                    this.SolarGainsPerWindowHourly[i] = new double[Misc.HoursPerYear];
                    Q_s_per_window[i].CopyTo(this.SolarGainsPerWindowHourly[i],0);
                    Misc.GetCumulativeMonthlyValue(Q_s_per_window[i]).CopyTo(this.SolarGainsPerWindowMonthly[i],0);
                }else
                {
                    Q_s_per_window[i].CopyTo(this.SolarGainsPerWindowMonthly[i], 0);
                }
            }
        }


        /// <summary>
        /// Setting energy demands of this zone. Loads have to be computed externally, e.g. with Hive.Core SIA380
        /// </summary>
        /// <param name="heatingLoads"></param>
        /// <param name="dhwLoads"></param>
        /// <param name="coolingLoads"></param>
        /// <param name="electricityLoads"></param>
        public void SetEnergyDemands(double[] heatingLoads, double[] dhwLoads, double[] coolingLoads, double[] electricityLoads)
        {
            this.HeatingLoadsMonthly = new double[Misc.MonthsPerYear];
            this.DHWLoadsMonthly = new double[Misc.MonthsPerYear];
            this.CoolingLoadsMonthly = new double[Misc.MonthsPerYear];
            this.ElectricityLoadsMonthly = new double[Misc.MonthsPerYear];

            if (heatingLoads.Length == Misc.HoursPerYear)
            {
                this.HeatingLoadsHourly = new double[Misc.HoursPerYear];
                heatingLoads.CopyTo(this.HeatingLoadsHourly,0);
                Misc.GetCumulativeMonthlyValue(heatingLoads).CopyTo(this.HeatingLoadsMonthly, 0);
            }
            else
            {
                heatingLoads.CopyTo(this.HeatingLoadsMonthly, 0);
            }

            if (dhwLoads.Length == Misc.HoursPerYear)
            {
                this.DHWLoadsHourly = new double[Misc.HoursPerYear];
                dhwLoads.CopyTo(this.DHWLoadsHourly,0);
                Misc.GetCumulativeMonthlyValue(dhwLoads).CopyTo(this.DHWLoadsMonthly,0);
            }
            else
            {
                dhwLoads.CopyTo(this.DHWLoadsMonthly, 0);
            }

            if (coolingLoads.Length == Misc.HoursPerYear)
            {
                this.CoolingLoadsHourly=new double[Misc.HoursPerYear];
                coolingLoads.CopyTo(this.CoolingLoadsHourly,0);
                Misc.GetCumulativeMonthlyValue(coolingLoads).CopyTo(this.CoolingLoadsMonthly,0);
            }
            else
            {
                coolingLoads.CopyTo(this.CoolingLoadsMonthly, 0);
            }

            if (electricityLoads.Length == Misc.HoursPerYear)
            {
                this.ElectricityLoadsHourly=new double[Misc.HoursPerYear];
                electricityLoads.CopyTo(this.ElectricityLoadsHourly,0);
                Misc.GetCumulativeMonthlyValue(electricityLoads).CopyTo(this.ElectricityLoadsMonthly,0);
            }
            else
            {
                electricityLoads.CopyTo(this.ElectricityLoadsMonthly, 0);
            }
        }


        public void SetLossesAndGains(double[] Qt_opaque, double [] Qt_transparent, double[] Qv, double[] Qi, double[] Qs)
        {
            this.OpaqueTransmissionHeatLossesMonthly = new double[Misc.MonthsPerYear];
            this.TransparentTransmissionHeatLossesMonthly = new double[Misc.MonthsPerYear];
            this.VentilationHeatLossesMonthly = new double[Misc.MonthsPerYear];
            this.InternalHeatGainsMonthly = new double[Misc.MonthsPerYear];
            this.SolarGainsMonthly = new double[Misc.MonthsPerYear];

            if (Qt_opaque.Length == Misc.HoursPerYear)
            {
                this.OpaqueTransmissionHeatLossesHourly=new double[Misc.HoursPerYear];
                Qt_opaque.CopyTo(this.OpaqueTransmissionHeatLossesHourly,0);
                Misc.GetCumulativeMonthlyValue(Qt_opaque).CopyTo(this.OpaqueTransmissionHeatLossesMonthly,0);
            }
            else
            {
                Qt_opaque.CopyTo(this.OpaqueTransmissionHeatLossesMonthly, 0);
            }

            if (Qt_transparent.Length == Misc.HoursPerYear)
            {
                this.TransparentTransmissionHeatLossesHourly = new double[Misc.HoursPerYear];
                Qt_transparent.CopyTo(this.TransparentTransmissionHeatLossesHourly,0);
                Misc.GetCumulativeMonthlyValue(Qt_transparent).CopyTo(this.TransparentTransmissionHeatLossesMonthly,0);
            }
            else
            {
                Qt_transparent.CopyTo(this.TransparentTransmissionHeatLossesMonthly, 0);
            }

            if (Qv.Length == Misc.HoursPerYear)
            {
                this.VentilationHeatLossesHourly=new double[Misc.HoursPerYear];
                Qv.CopyTo(this.VentilationHeatLossesHourly,0);
                Misc.GetCumulativeMonthlyValue(Qv).CopyTo(this.VentilationHeatLossesMonthly,0);
            }
            else
            {
                Qv.CopyTo(this.VentilationHeatLossesMonthly, 0);
            }

            if (Qi.Length == Misc.HoursPerYear)
            {
                this.InternalHeatGainsHourly=new double[Misc.HoursPerYear];
                Qi.CopyTo(this.InternalHeatGainsHourly,0);
                Misc.GetCumulativeMonthlyValue(Qi).CopyTo(this.InternalHeatGainsMonthly,0);
            }
            else
            {
                Qi.CopyTo(this.InternalHeatGainsMonthly, 0);
            }

            if (Qs.Length == Misc.HoursPerYear)
            {
                this.SolarGainsHourly=new double[Misc.HoursPerYear];
                Qs.CopyTo(this.SolarGainsHourly,0);
                Misc.GetCumulativeMonthlyValue(Qs).CopyTo(this.SolarGainsMonthly,0);
            }
            else
            {
                Qs.CopyTo(this.SolarGainsMonthly, 0);
            }
        }

        #endregion



        #region internalMethods

        public Zone Clone()
        {
            return MemberwiseClone() as Zone;
        }


        private static bool CheckFloorInZone(rg.Brep zone, rg.BrepFace floor)
        {
            //check if all points of middle floor are inside brep or on face.
            //but not all points are on face, otherwise it would be redundant(same as wall or base floor)

            double tol = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            var pts = new List<rg.Point3d>();

            foreach (rg.BrepVertex vertex in floor.Brep.Vertices)
            {
                rg.Point3d pt = vertex.Location;
                if (!zone.IsPointInside(pt, tol, false))
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Check the linearity of the brep zone geometry. No curves allowed for simplicity.
        /// </summary>
        /// <param name="brep"></param>
        /// <returns>True if all is linear</returns>
        private static bool CheckLinearity(rg.Brep brep)
        {
            bool isLinear = true;
            foreach (rg.BrepEdge edge in brep.Edges)
            {
                if (edge.IsLinear() == false)
                {
                    isLinear = false;
                    break;
                }
            }
            return isLinear;
        }


        /// <summary>
        /// Check the convexity of the zone. Hive only allows convex spaces, for simplicity.
        /// </summary>
        /// <param name="brep">geometry that is checked for convexity</param>
        /// <param name="tolerance">model tolerance, for intersection checks</param>
        /// <returns>True, if convex</returns>
        private static bool CheckConvexity(rg.Brep brep, double tolerance)
        {
            int vertexCount = brep.Vertices.Count;

            for (int i = 0; i < vertexCount; i++)
            {
                rg.BrepVertex vert1 = brep.Vertices[i];
                for (int u = 0; u < vertexCount; u++)
                {
                    if (i == u) continue;

                    rg.BrepVertex vert2 = brep.Vertices[u];
                    rg.LineCurve line = new rg.LineCurve(vert1.Location, vert2.Location);
                    rg.Curve[] overlap_curves;
                    rg.Point3d[] inter_points;
                    if (rg.Intersect.Intersection.CurveBrep(line, brep, tolerance, out overlap_curves, out inter_points))
                    {
                        if (overlap_curves.Length > 0 || inter_points.Length > 0)
                        {
                            if (inter_points.Length > 2)
                                return false;
                            else if (inter_points.Length == 2)
                            {
                                //check if middle point is within the brep or not. if not, its convex
                                rg.Point3d middlepoint = (inter_points[0] + inter_points[1]) / 2.0;
                                if (!brep.IsPointInside(middlepoint, tolerance, false))
                                {
                                    return false;
                                }
                            } //do i need a case with inter_points == 1?
                        }
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// Check the closedness of the brep. No open breps allowed, since a the brep is used to define a thermal space.
        /// </summary>
        /// <param name="brep"></param>
        /// <returns>True, if it is closed</returns>
        private static bool CheckClosedness(rg.Brep brep)
        {
            return brep.IsSolid;
        }


        /// <summary>
        /// Check for planarity of surfaces
        /// </summary>
        /// <param name="brep"></param>
        /// <returns></returns>
        private static bool CheckPlanarity(rg.Brep brep)
        {
            rg.Collections.BrepSurfaceList srfs = brep.Surfaces;
            foreach (rg.Surface srf in srfs)
            {
                if (!srf.IsPlanar()) return false;
            }
            return true;
        }


        // !!!!!!!!!!! TO DO
        // ADAPT to work for non-planar and non-linear geometry

        /// <summary>
        /// Check whether window surfaces lie on the zone geometry.
        /// </summary>
        /// <param name="brep"></param>
        /// <param name="windows"></param>
        /// <returns></returns>
        private static bool CheckWindowsOnZone(rg.Brep brep, rg.BrepFace[] windows, double tolerance)
        {
            int roundingDecimals = tolerance.ToString().Split('.')[1].Length;

            // check for Windows on Zone
            bool[] equalAreas = new bool[windows.Length];
            for (int i = 0; i < windows.Length; i++)
            {
                rg.BrepFace srf = windows[i];
                rg.Brep srfbrep = rg.Brep.CreateTrimmedSurface(srf, srf.UnderlyingSurface(), tolerance);
                rg.Curve[] intersectionCrvs;
                rg.Point3d[] intersectionPts;
                //rg.Intersect.Intersection.BrepSurface(brep, srf, tolerance, out intersectionCrvs, out intersectionPts);
                rg.Intersect.Intersection.BrepBrep(brep, srfbrep, tolerance, out intersectionCrvs, out intersectionPts);
                if (intersectionCrvs.Length == 0)
                    return false;

                rg.Curve curve = intersectionCrvs[0];
                if (intersectionCrvs.Length > 1)
                {
                    rg.Curve[] crv = rg.Curve.JoinCurves(intersectionCrvs);
                    if (crv.Length > 1 && !crv[0].IsClosed)
                        return false;
                    else
                        curve = crv[0];
                }

                rg.AreaMassProperties amp = rg.AreaMassProperties.Compute(curve);
                if(amp == null)
                    return false;
                double curveArea = amp.Area;

                double srfArea = rg.AreaMassProperties.Compute(srf).Area;
                if (Math.Round(curveArea, roundingDecimals) != Math.Round(srfArea, roundingDecimals))
                    return false;
                else
                    equalAreas[i] = true;
            }
            foreach (bool equalArea in equalAreas)
                if (!equalArea)
                    return false;

            return true;
        }

        private static bool CheckWindowOnZone(rg.Brep brep, rg.BrepFace window, double tolerance)
        {
            int roundingDecimals = tolerance.ToString().Split('.')[1].Length;

            // check for Windows on Zone
            bool equalArea = false;

            rg.BrepFace srf = window;
            rg.Brep srfbrep = rg.Brep.CreateTrimmedSurface(srf, srf.UnderlyingSurface(), tolerance);
            rg.Curve[] intersectionCrvs;
            rg.Point3d[] intersectionPts;
            //rg.Intersect.Intersection.BrepSurface(brep, srf, tolerance, out intersectionCrvs, out intersectionPts);
            rg.Intersect.Intersection.BrepBrep(brep, srfbrep, tolerance, out intersectionCrvs, out intersectionPts);
            if (intersectionCrvs.Length == 0)
                return false;
            rg.Curve curve = intersectionCrvs[0];
            if (intersectionCrvs.Length > 1)
            {
                rg.Curve[] crv = rg.Curve.JoinCurves(intersectionCrvs);
                if (crv.Length > 1 && !crv[0].IsClosed)
                    return false;
                else
                    curve = crv[0];
            }

            double curveArea, srfArea;
            rg.AreaMassProperties amp = rg.AreaMassProperties.Compute(curve);
            if (amp == null)
                return false;
            curveArea = rg.AreaMassProperties.Compute(curve).Area;
            srfArea = rg.AreaMassProperties.Compute(srf).Area;

            
            if (Math.Round(curveArea, roundingDecimals) != Math.Round(srfArea, roundingDecimals))
                return false;
            else
                equalArea = true;

            if (!equalArea)
                return false;

            return true;
        }


        // !!!!!!!!!!! TO DO
        // ADAPT to work for non-planar and non-linear geometry

        /// <summary>
        /// Check for windows-2-windows intersections
        /// </summary>
        /// <param name="windows"></param>
        /// <param name="tolerance"></param>
        /// <returns>Returns 'false' if there are windows-2-windows intersections</returns>
        private static bool CheckWindowsIsFreeOfIntersections(rg.Surface[] windows, double tolerance)
        {
            for (int i = 0; i < windows.Length - 1; i++)
            {
                rg.Brep w1 = windows[i].ToBrep();
                for (int u = (i + 1); u < windows.Length; u++)
                {
                    rg.Curve[] intersectionCrvs;
                    rg.Point3d[] intersectionPts;
                    // intersection check with 2 windows. They should not intersect, otherwise return an error message to Grasshopper
                    rg.Intersect.Intersection.BrepSurface(w1, windows[u], tolerance, out intersectionCrvs, out intersectionPts);
                    if (intersectionCrvs.Length > 0)
                        foreach(var crv in intersectionCrvs)
                            if (crv.IsClosed)   // they always have to be closed for intersections. otherwise the windows might just be touching
                                return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Check if all conditions are fulffilled.
        /// </summary>
        /// <returns>True, if zone geometry is valid</returns>
        private static bool CheckValidity(bool closed, bool convex, bool linear, bool planar, bool windowsOnZone, bool windowsSelfIntersect)
        {
            if (convex && closed && linear && planar && windowsOnZone && windowsSelfIntersect)
                return true;
            else
                return false;
        }


        /// <summary>
        /// Identifies and initializes building components from input geometries
        /// </summary>
        /// <param name="zone_geometry"></param>
        /// <param name="window_geometry"></param>
        /// <param name="shading_geometry"></param>
        /// <returns></returns>
        private Tuple<Wall[], Ceiling[], Roof[], Floor[], Window[], Shading[]>
            IdentifyComponents(rg.Brep zone_geometry, rg.BrepFace[] window_geometry, rg.BrepFace[] shading_geometry)
        {
            List<int> wall_indices = new List<int>();
            List<int> ceiling_indices = new List<int>();
            List<int> roof_indices = new List<int>();
            List<int> floor_indices = new List<int>();

            for (int i = 0; i < zone_geometry.Faces.Count(); i++)
            {
                rg.BrepFace srf = zone_geometry.Faces[i];
                srf.ClosestPoint(rg.AreaMassProperties.Compute(srf).Centroid, out double u, out double v);
                rg.Vector3d normal = srf.NormalAt(u, v); // for some reason, the bottom surface also has postivie normal here?!... using wrong point at line above?
                double angle = rg.Vector3d.VectorAngle(normal, new rg.Vector3d(0, 0, 1)) * 180 / Math.PI;

                // Floor: flat surface with  normal pointing downwards. 
                //  but careful, it could also be an overhanging wall. so floor is that surface with the lowest corner point
                //  lets say, floor MUST be flat
                // Ceiling: Same, but there must be an adjacent zone surface, such that this surface is internal. Hive 0.2
                if (normal.Z == -1.0)
                {
                    floor_indices.Add(i);
                }
                else if (angle < 45.0)                  // Roof: surface angle < 45? 
                {
                    roof_indices.Add(i);
                }
                else                                    // Wall: surface angle >= 45?
                {
                    wall_indices.Add(i);
                }
            }
            Wall[] walls = new Wall[wall_indices.Count()];
            Ceiling[] ceilings = new Ceiling[ceiling_indices.Count()];
            Roof[] roofs = new Roof[roof_indices.Count()];
            Floor[] floors = new Floor[floor_indices.Count()];

            for (int i = 0; i < walls.Length; i++)
                walls[i] = new Wall(zone_geometry.Faces[wall_indices[i]]);
            for (int i = 0; i < ceilings.Length; i++)
                ceilings[i] = new Ceiling(zone_geometry.Faces[ceiling_indices[i]]);
            for (int i = 0; i < roofs.Length; i++)
                roofs[i] = new Roof(zone_geometry.Faces[roof_indices[i]]);
            for (int i = 0; i < floors.Length; i++)
                floors[i] = new Floor(zone_geometry.Faces[floor_indices[i]]);


            var windowList = new List<Window>();
            if (window_geometry != null && window_geometry.Length > 0)
            {
                foreach (var w in walls.Cast<Component>().Concat(roofs))
                {
                    w.SubComponents = new List<Component>();
                    foreach (var win in window_geometry)
                    {
                        if (CheckWindowOnZone(w.BrepGeometry, win, this.Tolerance))     // how to return error message, if window is not on zone without double running this routine? (in constructor, CheckWindowSSSOnZone())
                        {
                            Window window = new Window(win);
                            windowList.Add(window);
                            w.SubComponents.Add(window);
                        }
                    }
                }
            }

            Shading[] shadings = new Shading[0];
            if (shading_geometry != null && shading_geometry.Length > 0)
            {
                shadings = new Shading[shading_geometry.Length];
                for (int i = 0; i < shading_geometry.Length; i++)
                {
                    shadings[i] = new Shading(shading_geometry[i]);
                }
            }

            return new Tuple<Wall[], Ceiling[], Roof[], Floor[], Window[], Shading[]>(walls, ceilings, roofs, floors, windowList.ToArray(), shadings);
        }
        #endregion
    }

}
