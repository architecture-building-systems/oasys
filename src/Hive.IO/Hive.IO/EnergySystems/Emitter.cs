﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Hive.IO.EnergySystems
{
    public class Radiator : Emitter
    {
        public Radiator(double investmentCost, double embodiedGhg, bool isHeating, bool isCooling, double inletTemperature, double returnTemperature) 
            : base(investmentCost, embodiedGhg, isHeating, isCooling)
        {

            double [] inletTemperatures = new double[Horizon];
            double [] returnTemperatures = new double[Horizon];
            for (int t = 0; t < Horizon; t++)
            {
                inletTemperatures[t] = inletTemperature;
                returnTemperatures[t] = returnTemperature;
            }
        }

        /// <summary>
        /// Setting supply and return carriers. Should contain information on energy (function of flowrate and temperature)
        /// </summary>
        /// <param name="waterIn">Supply</param>
        /// <param name="waterOut">Return</param>
        public void SetWaterflow(Water waterIn, Water waterOut)
        {
            base.InletCarrier = waterIn;
            base.ReturnCarrier = waterOut;
        }
    }


    public class AirDiffuser : Emitter
    {
        public AirDiffuser(double investmentCost, double embodiedGhg, bool isHeating, bool isCooling, double inletTemperature, double returnTemperature)
            : base(investmentCost, embodiedGhg, isHeating, isCooling)
        {
            double[] inletTemperatures = new double[Horizon];
            double[] returnTemperatures = new double[Horizon];
            for (int t = 0; t < Horizon; t++)
            {
                inletTemperatures[t] = inletTemperature;
                returnTemperatures[t] = returnTemperature;
            }
        }

        public void SetAirflow(Air airIn, Air airOut)
        {
            base.InletCarrier = airIn;
            base.ReturnCarrier = airOut;
        }
    }




    /// <summary>
    /// Emitting systems that emit heat/cooling to the room
    /// E.g. Underfloor heater, radiator, floor heating, ceiling cooling panels, etc.
    /// Also setting supply and return temperatures, cooling and heating set points...
    /// </summary>
    public abstract class Emitter
    {
        public const int Horizon = 8760;

        /// <summary>
        /// Technology name. E.g. Vaillant Floor Heater
        /// </summary>
        public string Name { get; protected set; }


        /// <summary>
        /// Indicating whether this technology produces heat
        /// </summary>
        public bool IsHeating { get; protected set; }
        /// <summary>
        /// Indicating whether this technology produces cooling
        /// </summary>
        public bool IsCooling { get; protected set; }


        /// <summary>
        /// Capacity of technology. Unit is defined in 'CapacityUnit'
        /// </summary>
        public double Capacity { get; protected set; }

        /// <summary>
        /// Unit of technology capacity (e.g. "kW", or "sqm", etc.)
        /// </summary>
        public string CapacityUnit { get; protected set; }

        /// <summary>
        /// Investment cost per this.CapacityUnit
        /// </summary>
        public double SpecificInvestmentCost { get; protected set; }
        /// <summary>
        /// Life cycle GHG emissions, in kgCO2eq. per this.CapacityUnit
        /// </summary>
        public double SpecificEmbodiedGhg { get; protected set; }


        /// <summary>
        /// Input stream. For most emitters, this would be water
        /// </summary>
        public EnergyCarrier InletCarrier { get; protected set; }
        /// <summary>
        /// Output streams. Must be of same type as InletCarrier (?)
        /// </summary>
        public EnergyCarrier ReturnCarrier { get; protected set; }



        protected Emitter(double investmentCost, double embodiedGhg,
            bool isHeating, bool isCooling)
        {
            this.SpecificInvestmentCost = investmentCost;
            this.SpecificEmbodiedGhg = embodiedGhg;
            this.IsHeating = isHeating;
            this.IsCooling = isCooling;
        }



        public void SetEmitterName(string name)
        {
            this.Name = name;
        }
    }
}
