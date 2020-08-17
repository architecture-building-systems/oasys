﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Hive.IO.EnergySystems;

namespace Hive.IO.GHComponents
{
    public class GhMergerGasBoiler : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhMergerGasBoiler class.
        /// </summary>
        public GhMergerGasBoiler()
          : base("Merger GasBoiler Hive", "HiveMergerBoiler",
              "Description",
              "[hive]", "IO")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.quinary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Gas", "Gas", "Gas", GH_ParamAccess.item);
            pManager.AddIntegerParameter("horizon", "horizon", "horizon", GH_ParamAccess.item);
            pManager.AddNumberParameter("heatGenerated", "heatGenerated", "heatGenerated (kWh)", GH_ParamAccess.list);
            pManager.AddNumberParameter("cost", "cost", "cost", GH_ParamAccess.list);
            pManager.AddNumberParameter("ghg", "ghg", "ghg", GH_ParamAccess.list);
            pManager.AddNumberParameter("suppTemp", "suppTemp", "suppTemp for water output. necessary to know for COP calculation", GH_ParamAccess.list);

            pManager.AddGenericParameter("Hive.IO.GasBoiler", "GasBoiler", "GasBoiler", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.GasBoiler", "GasBoiler", "GasBoiler", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Gas gas = null;
            DA.GetData(0, ref gas);

            int horizon = 8760;
            DA.GetData(1, ref horizon);

            var energyGenerated = new List<double>();
            DA.GetDataList(2, energyGenerated);

            var energyCost = new List<double>();
            DA.GetDataList(3, energyCost);

            var ghg = new List<double>();
            DA.GetDataList(4, ghg);

            var supplyTemp = new List<double>();
            DA.GetDataList(5, supplyTemp);

            GasBoiler boiler = null;
            DA.GetData(6, ref boiler);


            boiler.SetInput(gas);

            // this creates a water EnergyCarrier that will be infused into the AirSourceHeatPump
            boiler.SetOutput(horizon, energyGenerated.ToArray(), energyCost.ToArray(), ghg.ToArray(), supplyTemp.ToArray());

            DA.SetData(0, boiler);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a8196942-924f-44d7-83e7-19565b462b6f"); }
        }
    }
}