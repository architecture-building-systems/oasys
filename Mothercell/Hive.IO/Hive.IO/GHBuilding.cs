﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;

using Rhino.Geometry;


namespace Hive.IO
{
    public class GHBuilding : GH_Component
    {
        public GHBuilding()
          : base("Hive.IO.Building", "Hive.IO.Building",
              "Hive Building, representing thermal and construction properties. Like a multi-zone building model.",
              "[hive]", "IO")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Zone Geometry", "Zone Geometry", "Zone geometries. Breps. Can be multiple boxes. Should have window surfaces overlaied (dont cut them out)", GH_ParamAccess.list);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.Building", "Hive.IO.Building", "Hive.IO.Building Object, that contains all zones and windows. Solved for adjacencies", GH_ParamAccess.item);
        }


        public override void CreateAttributes()
        {
            m_attributes = new BuildingComponentAttributes(this);
        }

        private class BuildingComponentAttributes : GH_ComponentAttributes
        {
            public BuildingComponentAttributes(IGH_Component component) : base(component) { }

            public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                (Owner as GHBuilding)?.DisplayForm();
                return GH_ObjectResponse.Handled;
            }
        }

        FormBuilding _form;
        public void DisplayForm()
        {
            if (_form != null) 
                return;

            _form = new FormBuilding();

            _form.FormClosed += OnFormClosed;

            GH_WindowsFormUtil.CenterFormOnCursor(_form, true);
            _form.Show(Grasshopper.Instances.DocumentEditor);
            _form.Location = Cursor.Position;
        }

        private void OnFormClosed(object sender, FormClosedEventArgs formClosedEventArgs)
        {
            _form = null;
        }



        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Show UI", ShowUiClicked, null, true, false);
        }
        private void ShowUiClicked(object sender, EventArgs e)
        {
            DisplayForm();
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Brep> breps = new List<Brep>();
            if ((!DA.GetDataList(0, breps))) 
                return;
            if (breps.Count == 0)   // could get an empty list
                return;


        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("43a45a89-485b-4134-b073-17bac23e76d5"); }
        }
    }
}