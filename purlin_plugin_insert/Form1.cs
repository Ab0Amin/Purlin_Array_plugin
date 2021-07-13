using System;
using System.Collections;
using System.Collections.Generic;
using Tekla.Structures;
using Tekla.Structures.Geometry3d;
using t3d = Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.Operations;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Solid;
using System.Linq;
using System.Windows.Forms;
namespace purlin_plugin_insert
{
    public partial class Form1 : Form
    {
        Model myModel = new Model();
        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            myModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
            Picker input = new Picker();
            ArrayList points = input.PickPoints(Picker.PickPointEnum.PICK_POLYGON, "Picker Frame Points");
            //ModelObjectEnumerator grids = input.PickObjects(Picker.PickObjectsEnum.PICK_N_OBJECTS);

            ArrayList line1 = input.PickLine();
            ArrayList line2 = input.PickLine();
            Vector vec1 = new Vector((line1[0] as Point) -( line1[1] as Point)); 
            Vector vec2 = new Vector((line2[0] as Point) - (line2[1] as Point));

            Vector vecZ = new Vector(0, 0, 1);
            GeometricPlane g1 = new GeometricPlane((line1[0] as Point), vecZ, vec1);
            GeometricPlane g2 = new GeometricPlane((line2[0] as Point), vecZ, vec2);

            List<GeometricPlane> plans = new List<GeometricPlane>();
            plans.Add(g1);
            plans.Add(g2);

            double dis = Distance.PointToPoint((line1[0] as Point), (line2[0] as Point));


            for (int i = 0; i < plans.Count; i++)
            {
                GeometricPlane currentPlan = plans[i];
                for (int t = 0; t < points.Count; t++)
                {
                    if (t+1< points.Count)
                    {
                        Point p1 = points[t] as Point;
                        Point p2 = points[t + 1] as Point;
                        p1 = Projection.PointToPlane(p1, currentPlan);
                        p2 = Projection.PointToPlane(p2, currentPlan);
                        ArrayList conPoints = new ArrayList();
                        conPoints.Add(p1);
                        conPoints.Add(p2);
                        Component con = new Component();
                        con.Name = "Purlin_Array";
                        ComponentInput componentInput = new ComponentInput();
                        Polygon polygon = new Polygon();
                        polygon.Points = conPoints;
                        componentInput. AddTwoInputPositions(p1,p2);
                        componentInput. AddInputPolygon(polygon);
                        
                        con.SetComponentInput(componentInput);
                        con.LoadAttributesFromFile("standard");
                        con.Insert();
                        CustomPart  d = new CustomPart();
                        d.SetInputPositions(p1, p2);
                        d.Name = "Purlin_Array";
                        d.Insert();
                    }
                }

            }
            myModel.CommitChanges();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Picker input = new Picker(); 
            object points = input.PickObject(Picker.PickObjectEnum.PICK_ONE_OBJECT);

        }
    }
}
