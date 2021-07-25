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
            int no_grids = int.Parse(tx_lines.Text);
            List<GeometricPlane> plans = new List<GeometricPlane>();
            List<Point> midPoints = new List<Point>();
            List<Vector> vectors = new List<Vector>();

            while (no_grids>0)
            {
                ArrayList line = input.PickLine();
                Vector vec= new Vector((line[0] as Point) - (line[1] as Point));
                Point mid_point = getmidpoint((line[0] as Point) , (line[1] as Point));
                Vector vecz= new Vector(0, 0, 1);
                GeometricPlane plane = new GeometricPlane((line[0] as Point), vecz, vec);
                plans.Add(plane);
                midPoints.Add(mid_point);
                vectors.Add(vec);

                no_grids -= 1;

            }

            plans.Remove(plans[plans.Count - 1]);
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
                        con.Number = -100000;
                        ComponentInput componentInput = new ComponentInput();
                        Polygon polygon = new Polygon();
                        polygon.Points = conPoints;
                        //componentInput.AddTwoInputPositions(p1, p2);
                        //componentInput.AddInputPolygon(polygon);
                        componentInput.AddOneInputPosition(p1);
                        componentInput.AddOneInputPosition(p2);
                        con.SetComponentInput(componentInput);
                        con.LoadAttributesFromFile("standard");

                        t3d.Point mid1 = midPoints[i];
                        t3d.Point mid2 = midPoints[i+1];
                        double dis = Distance.PointToPoint(mid1, mid2);
                        con.SetAttribute("purlinLength", dis);
                        Vector vector = new Vector(mid2- mid1);
                     vector.Normalize();
                        double X= vector.X;
                        double Y = vector.Y;
                        double direction;
                        if ((int)X != 0)
                        {
                            direction = X;
                            con.SetAttribute("axis", 1);

                            
                        }
                        else
                        {
                            direction = Y;
                            con.SetAttribute("axis", 0);

                        }
                        if (direction>0)
                        {
                            con.SetAttribute("cb_singleDoule", 0);

                        }
                        else
                        {
                            con.SetAttribute("cb_singleDoule", 1);
                        }

                        con.Insert();

                    }
                }

            }
            myModel.CommitChanges();

        }

       
        public static t3d.Point getmidpoint(t3d.Point p1, t3d.Point p2)
        {
            double dis = t3d.Distance.PointToPoint(p1, p2);
            t3d.Vector vec = new t3d.Vector(p2 - p1);
            vec.Normalize();
            return p1 + 0.5 * dis * vec;
        }

    }
}
