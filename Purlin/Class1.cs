using System;
using System.Collections;
using System.Collections.Generic;
using Tekla.Structures;
using Tekla.Structures.Geometry3d;
using t3d = Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.Operations;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Plugins;
using Tekla.Structures.Solid;
using System.Linq;
using System.Windows.Forms;

namespace Purlin_Array
{
    public class StructuresData3
    {

        //[Tekla.Structures.Plugins.StructuresField("P1")]
        //public double Parameter1;

        // purlin
        [StructuresField("purlin_profile")]
        public string purlin_profile;
        [StructuresField("purlin_material")]
        public string purlin_material;
        [StructuresField("purlin_perfix")]
        public string purlin_perfix;
        [StructuresField("purlin_name")]
        public string purlin_name;
        [StructuresField("purlin_startNO")]
        public int purlin_startNO;

        // dim

        [StructuresField("extenstion_1")]
        public double extenstion_1;
        [StructuresField("extenstion_2")]
        public double extenstion_2;
        [StructuresField("extenstion_12")]
        public double extenstion_12;
        [StructuresField("extenstion_22")]
        public double extenstion_22;
        [StructuresField("VLpffset")]
        public double VLpffset;
        [StructuresField("HZpffset")]
        public double HZpffset;
        [StructuresField("purlinLength")]
        public double purlinLength;
        [StructuresField("purlinLength2")]
        public double purlinLength2;
        [StructuresField("spacings")]
        public string spacings;

        // combobox
        [StructuresField("cb_rotation")]
        public int cb_rotation;

        [StructuresField("cb_singleDoule")]
        public int cb_singleDoule;

        [StructuresField("axis")]
        public int axis;



    }

    [Plugin("Purlin_Array")]
    [PluginUserInterface("Purlin_Array.PurlinPlugin")]






    public class Purlin_Array : PluginBase
    {

        private readonly StructuresData3 data;
        private readonly Model myModel;

        public Purlin_Array(StructuresData3 data)
        {
            this.data = data;
            myModel = new Model();

            myModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
        }

        public override List<PluginBase.InputDefinition> DefineInput()
        {
            List<PluginBase.InputDefinition> inputDefinitionList = new List<PluginBase.InputDefinition>();
            //Picker picker = new Picker();
            //Part mainPart = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Pick Main Part") as Part;
            //Part secendaryPart = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Pick Secandry Part") as Part;
            Picker input = new Picker();

            t3d.Point point11 = input.PickPoint("Pick first Point");
            t3d.Point point22 = input.PickPoint("Pick secand Point");
            //Picker inputs = new Picker();
            //ModelObjectEnumerator rafter_1 = inputs.PickObjects(Picker.PickObjectsEnum.PICK_N_PARTS, "Pick webs of first Rafter");
            //ModelObjectEnumerator rafter_2 = inputs.PickObjects(Picker.PickObjectsEnum.PICK_N_PARTS, "Pick webs of second Rafter");

            //ArrayList polygonPoints = inputs.PickPoints(Picker.PickPointEnum.PICK_POLYGON);



            PluginBase.InputDefinition inputDefinition1 = new PluginBase.InputDefinition(point11);
            PluginBase.InputDefinition inputDefinition2 = new PluginBase.InputDefinition(point22);
            inputDefinitionList.Add(inputDefinition1);
            inputDefinitionList.Add(inputDefinition2);
            return inputDefinitionList;
        }

        public override bool Run(List<PluginBase.InputDefinition> Input)
        {
            try
            {
                #region defults
                //purlin
                if (this.IsDefaultValue(this.data.purlin_profile))
                    this.data.purlin_profile = "PFC-430*100*64";
                if (this.IsDefaultValue(this.data.purlin_material))
                    this.data.purlin_material = "A653M SS G340 Z275 G90";
                if (this.IsDefaultValue(this.data.purlin_perfix))
                    this.data.purlin_perfix = "";
                if (this.IsDefaultValue(this.data.purlin_startNO))
                    this.data.purlin_startNO = 4001;
                if (this.IsDefaultValue(this.data.purlin_name))
                    this.data.purlin_name = "";


                //  dim
                if (this.IsDefaultValue(this.data.extenstion_1))
                    this.data.extenstion_1 = 0;
                if (this.IsDefaultValue(this.data.extenstion_2))
                    this.data.extenstion_2 = 0;
                if (this.IsDefaultValue(this.data.extenstion_12))
                    this.data.extenstion_12 = 0;
                if (this.IsDefaultValue(this.data.extenstion_22))
                    this.data.extenstion_22 = 0;
                if (this.IsDefaultValue(this.data.VLpffset))
                    this.data.VLpffset = 0;
                if (this.IsDefaultValue(this.data.HZpffset))
                    this.data.HZpffset = 0;
                if (this.IsDefaultValue(this.data.purlinLength))
                    this.data.purlinLength = 4000;
                if (this.IsDefaultValue(this.data.purlinLength2))
                    this.data.purlinLength2 = 4000;
                if (this.IsDefaultValue(this.data.spacings))
                    this.data.spacings = "";


                // combobox
                if (this.IsDefaultValue(this.data.cb_rotation))
                    this.data.cb_rotation = 0;
                if (this.IsDefaultValue(this.data.cb_singleDoule))
                    this.data.cb_singleDoule = 0;
                if (this.IsDefaultValue(this.data.axis))
                    this.data.axis = 0;


                #endregion

                //this.createconnection((Part)this.myModel.SelectModelObject((Identifier)Input[0].GetInput()), (Part)this.myModel.SelectModelObject((Identifier)Input[1].GetInput()));
                this.createconnection((t3d.Point)Input[0].GetInput(), (t3d.Point)Input[1].GetInput());
            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());
            }
            return true;
        }

        public void createconnection(t3d.Point point1, t3d.Point point2)
        {

            Position.RotationEnum rotation;
            Position.DepthEnum leftOrright;
            Position.PlaneEnum planPos;
            TransformationPlane currentPlan = myModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            TransformationPlane workingPlan;


            t3d.Point point1G = (point1);
            t3d.Point point2G = (point2);
            t3d.Point zeroPoint = new Point(0, 0, 0);
            Vector x_global = new Vector(1, 0, 0);
            Vector y_global = new Vector(0, 1, 0);
            Vector z_global = new Vector(0, 0, 1);
            GeometricPlane geometricPlanexy = new GeometricPlane(zeroPoint, x_global, y_global);
            GeometricPlane geometricPlanezy = new GeometricPlane(zeroPoint, z_global, y_global);
            GeometricPlane geometricPlanexz = new GeometricPlane(zeroPoint, x_global, z_global);

            t3d.Point proPoint1 = Projection.PointToPlane(point1G, geometricPlanexy);
            t3d.Point proPoint2 = Projection.PointToPlane(point2G, geometricPlanexy);

            double dis1 = Distance.PointToPoint(proPoint1, point1G);
            double dis2 = Distance.PointToPoint(proPoint2, point2G);

            Vector vecProjecction = new Vector(proPoint2 - proPoint1);
            vecProjecction.Normalize();
            Vector vecY, vecZ;

            if (proPoint1 != proPoint2)
            {
                if (vecProjecction == y_global || vecProjecction == -1 * y_global)
                    vecZ = x_global;
                else
                    vecZ = y_global;
            }
            else
            {
                if (data.axis == 0)
                    vecZ = y_global;
                else
                    vecZ = x_global;
            }

            if (vecZ == x_global)
            {
                Vector vx = new Vector(point2 - point1);

                CoordinateSystem coordinate1 = new CoordinateSystem(point1, vx, new Vector(1, 0, 0).Cross(vx));

                workingPlan = new TransformationPlane(coordinate1);
                myModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(workingPlan);

            }
            else
            {

                Vector vx = new Vector(point2 - point1);
                CoordinateSystem coordinate1 = new CoordinateSystem(point1, vx, new Vector(0, 1, 0).Cross(vx));
                workingPlan = new TransformationPlane(coordinate1);
                myModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(workingPlan);

            }
            //myModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());



            //var macroBuilder = new MacroBuilder();
            //macroBuilder.ValueChange("main_frame", "select_assemblies", "1");
            //macroBuilder.Run();

           
            Vector vecX = new Vector(point2 - point1);
            vecX.Normalize();
            vecY = vecX.Cross(vecZ);
            vecX.Normalize();
            vecY.Normalize();
            vecZ.Normalize();
            TransformationPlane plane = new TransformationPlane(point1, vecX, vecY);
            //myModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(plane);

            point1 = workingPlan.TransformationMatrixToLocal.Transform(point1);
            point2 = workingPlan.TransformationMatrixToLocal.Transform(point2);

            t3d.Point purlinPoint_1 = new Point(point1.X + data.HZpffset, point1.Y + data.VLpffset, point1.Z );
            t3d.Point purlinPoint_2 = new Point(purlinPoint_1.X, purlinPoint_1.Y, purlinPoint_1.Z  + data.purlinLength);
            if(data.cb_singleDoule == 1)
            {
                purlinPoint_2 = purlinPoint_1;
                purlinPoint_1 = new Point(purlinPoint_1.X , purlinPoint_1.Y, purlinPoint_1.Z - data.purlinLength);
            }
           
            Point geometricPlaneOrgin = currentPlan.TransformationMatrixToGlobal.Transform(plane.TransformationMatrixToLocal.Transform(geometricPlanexy.Origin));
            if (proPoint1 != proPoint2)
            {
                if (dis1 > dis2)
                {
                    rotation = Position.RotationEnum.FRONT;
                    leftOrright = Position.DepthEnum.FRONT;
                    planPos = Position.PlaneEnum.LEFT;
                    if (geometricPlaneOrgin.Y < 0)
                    {
                        planPos = Position.PlaneEnum.RIGHT;
                        leftOrright = Position.DepthEnum.BEHIND;


                    }
                }
                else
                {

                    rotation = Position.RotationEnum.BACK;
                    leftOrright = Position.DepthEnum.BEHIND;
                    planPos = Position.PlaneEnum.RIGHT;
                    if (geometricPlaneOrgin.Y > 0)
                    {
                        planPos = Position.PlaneEnum.LEFT;
                        leftOrright = Position.DepthEnum.FRONT;


                    }
                }

            }
            else
            {
                if (dis1 > dis2)
                {
                    rotation = Position.RotationEnum.BACK;
                    leftOrright = Position.DepthEnum.BEHIND;
                    planPos = Position.PlaneEnum.RIGHT;
                   
                }
                else
                {

                    rotation = Position.RotationEnum.FRONT;
                    leftOrright = Position.DepthEnum.BEHIND;
                    planPos = Position.PlaneEnum.RIGHT;
                   
                }
            }
            for (int t = 0; t < 2; t++)
            {
                insert_purlin(purlinPoint_1, purlinPoint_2, data.purlin_profile, data.purlin_material, data.purlin_name, data.purlin_perfix, data.purlin_startNO, rotation, leftOrright, planPos, data.extenstion_1 * -1, data.extenstion_2);

                double dis = Distance.PointToPoint(point1, point2);
                if (data.spacings == "")
                {
                    while (dis > 2000)
                    {
                        purlinPoint_1 = new Point(purlinPoint_1.X + 2000, purlinPoint_1.Y, purlinPoint_1.Z);
                        purlinPoint_2 = new Point(purlinPoint_2.X + 2000, purlinPoint_2.Y, purlinPoint_2.Z);

                        dis -= 2000;

                        insert_purlin(purlinPoint_1, purlinPoint_2, data.purlin_profile, data.purlin_material, data.purlin_name, data.purlin_perfix, data.purlin_startNO, rotation, leftOrright, planPos, data.extenstion_1 * -1, data.extenstion_2);

                    }
                }
                else
                {
                    List<double> spacings = listspacing(data.spacings, 100);
                    for (int i = 0; i < spacings.Count; i++)
                    {
                        purlinPoint_1 = new Point(purlinPoint_1.X + spacings[i], purlinPoint_1.Y, purlinPoint_1.Z);
                        purlinPoint_2 = new Point(purlinPoint_2.X + spacings[i], purlinPoint_2.Y, purlinPoint_2.Z);
                        insert_purlin(purlinPoint_1, purlinPoint_2, data.purlin_profile, data.purlin_material, data.purlin_name, data.purlin_perfix, data.purlin_startNO, rotation, leftOrright, planPos, data.extenstion_1 * -1, data.extenstion_2);


                    }
                }
                if (data.cb_singleDoule != 2)
                {
                    break;
                }

                 purlinPoint_2 = new Point(point1.X + data.HZpffset, point1.Y + data.VLpffset, point1.Z);
                 purlinPoint_1 = new Point(purlinPoint_2.X, purlinPoint_2.Y, purlinPoint_2.Z - data.purlinLength);
                data.purlinLength = data.purlinLength2;
                data.extenstion_1 = data.extenstion_12;
                data.extenstion_2 = data.extenstion_22;
            }


            //myModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentPlan);


        }
        private List<double> listspacing(string spacings, int N)
        {
            List<double> spacing = new List<double>();
            spacings = spacings.Trim();
            //if (N != 1)
            {
                if (spacings.Contains(" ") || spacings.Contains("*"))
                {
                    string[] array = spacings.Split(' ');
                    for (int i = 0; i < array.Length; i++)
                    {
                        string text = array[i];
                        if (!text.Contains('*'))
                        {
                            spacing.Add(Convert.ToDouble(text));
                        }
                        if (text.Contains('*'))
                        {
                            string[] array2 = text.Split('*');
                            int num = int.Parse(array2[0]);
                            for (int j = 0; j < num; j++)
                            {
                                spacing.Add(Convert.ToDouble(array2[1]));
                            }
                        }
                    }
                }
                else
                {
                    //for (int i = 0; i < N - 1; i++)
                    {
                        spacing.Add(Convert.ToDouble(spacings));
                    }
                }
            }
            //else
            //{
            //    spacing.Add(Convert.ToDouble(spacings));
            //}

            return spacing;
        }

        public Beam check_with_beam(Point start_point, Point end_point)
        {
            Beam beam = new Beam();
            beam.StartPoint = start_point;
            beam.EndPoint = end_point;
            beam.Profile.ProfileString = "ROD100";
            beam.Position.Depth = Position.DepthEnum.FRONT;
            beam.Position.Plane = Position.PlaneEnum.RIGHT;
            beam.Position.Rotation = Position.RotationEnum.TOP;
            beam.Insert();
            return beam;
        }

        public Beam insert_purlin(Point start_point, Point end_point, string profile, string material, string name, string prefix, int startNO, Position.RotationEnum rotation, Position.DepthEnum leftOrRight, Position.PlaneEnum planePos,double startEx ,double endEx)
        {
            Beam beam = new Beam();
            beam.StartPoint = start_point;
            beam.EndPoint = end_point;
            beam.Profile.ProfileString = profile;
            beam.Material.MaterialString = material;
            beam.AssemblyNumber.StartNumber = startNO;
            beam.AssemblyNumber.Prefix = prefix;
            beam.Name = name;
            beam.Position.Depth = leftOrRight;
            beam.Position.Plane = planePos;
            beam.Position.Rotation = rotation;
            beam.StartPointOffset.Dx = startEx;
            beam.EndPointOffset.Dx = endEx;
            beam.Insert();
            return beam;
        }

        public class UserInterfaceDefinition
        {



            public const string Plugin3 =
                "page(\"TeklaStructures\",\"\")\n   " +
                " {\n    joint(1, Purlin_Array)\n   " +

                " {\n      tab_page(\"1\", \" Picture \", 1)\n   " +


                "     {\n      " +


                 //plate lables
                 "   attribute(\"\", \"t\", label, \"%s\", none, none, \"0\", \"0\", 140, 9)\n     " +
                "       attribute(\"\", \"b\", label, \"%s\", none, none, \"0\", \"0\", 190, 9)\n         " +
                "   attribute(\"\", \"Prefix\", label, \"%s\", none, none, \"0\", \"0\", 285, 9)\n         " +
                "   attribute(\"\", \"Start_NO\", label, \"%s\", none, none, \"0\", \"0\", 365, 9)\n       " +
                "     attribute(\"\", \"Matrial\", label, \"%s\", none, none, \"0\", \"0\", 470, 9)\n     " +
                "       attribute(\"\", \"name\", label, \"%s\", none, none, \"0\", \"0\", 610, 9)\n      " +

                "      attribute(\"\", \"Rod Profile\", label, \"%s\", none, none, \"0\", \"0\", 15, 34)\n       " +
                "      attribute(\"\", \"Plate\", label, \"%s\", none, none, \"0\", \"0\", 15, 75)\n       " +

                //rod prameterss
                "     parameter(\"\", \"rod_profile\", profile, number, 125, 34, 115)\n         " +
                "     parameter(\"\", \"rod_perfix\", string, text, 295, 34, 40)\n         " +
                "   parameter(\"\", \"rod_startNO\", integer, number, 385, 34, 70)\n   " +
                "         parameter(\"\", \"rod_material\", material, text, 490, 34, 100)\n   " +
                "         parameter(\"\", \"rod_name\", string, text, 650, 34, 100)\n  " +

                      //plate prameterss
                      "     parameter(\"\", \"plateThik\", distance, number, 125, 75, 40)\n         " +
                      "     parameter(\"\", \"plateWidth\", distance, number, 205, 75, 50)\n         " +

                      "     parameter(\"\", \"plate_perfix\", string, text, 295, 75, 40)\n         " +
                      "   parameter(\"\", \"plate_startNO\", integer, number, 385, 75, 70)\n   " +
                      "         parameter(\"\", \"plateMaterial\", material, text, 490, 75, 100)\n   " +
                      "         parameter(\"\", \"plate_name\", string, text, 650, 75, 100)\n  " +


              // pics

              "   picture(\"sagRodPlane\", 0, 0, 155, 132)\n      " +
              "   picture(\"sagrodElevasion\", 0, 0, 552, 132)\n      " +
              "   picture(\"sagRodBltEdge\", 0, 0, 372, 371)\n      " +

                  // dim

                  //"         parameter(\"\", \"HzOffset_end\", distance, number, 42, 130, 50)\n  " +
                  "         parameter(\"\", \"spacings\", string, text, 110, 320, 150)\n  " +
                "      attribute(\"\", \"Spacings\", label, \"%s\", none, none, \"0\", \"0\", 5, 320)\n       " +

                "         parameter(\"\", \"rod_extension\", distance, number, 350, 150, 50)\n  " +
                  "         parameter(\"\", \"depth\", distance, number, 485, 170, 50)\n  " +

                "         parameter(\"\", \"spacing\", distance, number, 485, 220, 50)\n  " +


                  "   attribute(\"cb_sinleordouble\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 485, 280, 100)\n  " +
                      "  {\n     " +

                      "   value(\"Single\", 1)\n      " +
                      "  value(\"Double\", 0)\n  " +
                      "  }\n         " +


                   //remove plates 


                   //"   attribute(\"removePlate2_f1\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 25, 160, 70)\n  " +
                   //     "  {\n     " +

                   //     "   value(\"Plate\", 1)\n      " +
                   //     "  value(\"None\", 0)\n  " +
                   //     "  }\n         " +


                   @"            attribute(""removePlate1_f1"", """", option, ""%s"", none, none, ""0.0"", ""0.0"", 25, 200, 100,""toggle_field:DetetedPlates_F1=1,0;"")" + "\n" +
                        "            {\n" +
                        @"                value(""Plate"", 1)" + "\n" +
                        @"                value(""None"", 0)" + "\n" +
                        @"                value(""Delete NO"", 0)" + "\n" +
                        "            }\n" +

                //"   attribute(\"removePlate1_f1\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 25, 200, 100,\"toggle_field:DetetedPlates_F1=1,0;\")\n  " +
                //      "  {\n     " +

                //      "   value(\"Plate\", 1)\n      " +
                //      "  value(\"None\", 0)\n  " +
                //      "  value(\"Delete NO\", 0)\n  " +
                //      "  }\n         " +


                "         parameter(\"\", \"DetetedPlates_F1\", string, text, 25, 240, 100)\n  " +


                    //"   attribute(\"removePlate2_f2\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 300, 118, 70)\n  " +
                    //      "  {\n     " +

                    //      "   value(\"Plate\", 1)\n      " +
                    //      "  value(\"None\", 0)\n  " +
                    //      "  }\n         " +


                    @"            attribute(""removePlate1_f2"", """", option, ""%s"", none, none, ""0.0"", ""0.0"", 340, 200, 100,""toggle_field:DetetedPlates_F2=1,0;"")" + "\n" +
                        "            {\n" +
                        @"                value(""Plate"", 1)" + "\n" +
                        @"                value(""None"", 0)" + "\n" +
                        @"                value(""Delete NO"", 0)" + "\n" +
                        "            }\n" +

                //"   attribute(\"removePlate1_f2\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 340, 200, 100\"toggle_field:DetetedPlates_F2=1,0;\")\n  " +
                //      "  {\n     " +

                //      "   value(\"Plate\", 1)\n      " +
                //      "  value(\"None\", 0)\n  " +
                //      "  value(\"Delete NO\", 0)\n  " +
                //      "  }\n         " +

                "         parameter(\"\", \"DetetedPlates_F2\", string, text, 340, 240, 100)\n  " +


                 //bolt
                 "   attribute(\"\", \"Bolt Standard\", label, \"%s\", none, none, \"0\", \"0\", 40, 380)\n     " +
                 "   attribute(\"\", \"Bolt Size\", label, \"%s\", none, none, \"0\", \"0\", 40, 410)\n     " +
                 "   attribute(\"\", \"Tolerance\", label, \"%s\", none, none, \"0\", \"0\", 40, 440)\n     " +
                 "   attribute(\"\", \"X\", label, \"%s\", none, none, \"0\", \"0\", 120, 470)\n     " +
                 "   attribute(\"\", \"Y\", label, \"%s\", none, none, \"0\", \"0\", 210, 470)\n     " +
                 "   attribute(\"\", \"Slot\", label, \"%s\", none, none, \"0\", \"0\", 40, 495)\n     " +


                "         parameter(\"\", \"bolt_sec_screwdin\", bolt_standard, text, 170, 380, 100)\n  " +
                "         parameter(\"\", \"bolt_sec_diameter\", bolt_size, number, 170,410, 100)\n  " +
                "         parameter(\"\", \"tolerance_sec\", distance, number, 170,440, 100)\n  " +
                "         parameter(\"\", \"slotX_2\", distance, number, 102,495, 50)\n  " +
                "         parameter(\"\", \"slotY_2\", distance, number, 190,495, 50)\n  " +
                "         parameter(\"\", \"edge_1\", distance, number, 310,390, 50)\n  " +



                //            // plate chanfer
                //           "   attribute(\"cb_polybeamChanfer\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 80, 210, 70)\n  " +
                //      "  {\n     " +

                //      "   value(\"arc\", 1)\n      " +
                //      "  value(\"line\", 0)\n  " +
                //      "  value(\"none\", 0)\n  " +
                //      "  }\n         " +
                //      "         parameter(\"\", \"polyBeam_chanfer_x\", distance, number, 80, 250, 70)\n  " +
                //      "         parameter(\"\", \"polyBeam_chanfer_y\", distance, number, 80, 280, 70)\n  " +

                //           // stiff chanfer
                //           "   attribute(\"cb_stiffChanfer\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 370, 155, 70)\n  " +
                //      "  {\n     " +

                //      "   value(\"arc\", 0)\n      " +
                //      "  value(\"line\", 1)\n  " +
                //      "  value(\"none\", 0)\n  " +
                //      "  }\n         " +
                //      "         parameter(\"\", \"stiff_chanfer_x\", distance, number, 370, 195, 70)\n  " +
                //      "         parameter(\"\", \"stiff_chanfer_y\", distance, number, 370, 230, 70)\n  " +
                //      //plate stiff pic
                //      "  picture(\"plate_stiff_anglePlugin_peb\", 0, 0, 185, 155)\n      " +

                //      //connection shift
                //      "  picture(\"xs_detail_64_point_def\", 0, 0, 725, 155)\n      " +
                //      "  attribute(\"\", \"Connection shift\", label, \"%s\", none, none, \"0\", \"0\", 545, 260)\n     " +
                //      "  parameter(\"\", \"Connection_shift\", distance, number, 695, 260, 70)\n  " +



                // // primary bolt

                // //labels
                // "   attribute(\"\", \"Primary Bolts\", label, \"%s\", none, none, \"0\", \"0\", 105, 350)\n     " +
                // "   attribute(\"\", \"Bolt Standard\", label, \"%s\", none, none, \"0\", \"0\", 40, 380)\n     " +
                // "   attribute(\"\", \"Bolt Size\", label, \"%s\", none, none, \"0\", \"0\", 40, 410)\n     " +
                // "   attribute(\"\", \"Tolerance\", label, \"%s\", none, none, \"0\", \"0\", 40, 440)\n     " +
                // "   attribute(\"\", \"Workshop/Site\", label, \"%s\", none, none, \"0\", \"0\", 40, 470)\n     " +
                // "   attribute(\"\", \"Washer\", label, \"%s\", none, none, \"0\", \"0\", 40, 510)\n     " +
                // "   attribute(\"\", \"Nut\", label, \"%s\", none, none, \"0\", \"0\", 200, 510)\n     " +
                // "   attribute(\"\", \"X\", label, \"%s\", none, none, \"0\", \"0\", 95, 585)\n     " +
                // "   attribute(\"\", \"Y\", label, \"%s\", none, none, \"0\", \"0\", 185, 585)\n     " +
                // "   attribute(\"\", \"Slot\", label, \"%s\", none, none, \"0\", \"0\", 15, 615)\n     " +
                // "   attribute(\"\", \"Weld Size\", label, \"%s\", none, none, \"0\", \"0\", 270, 650)\n     " +
                // "   attribute(\"\", \"Bolt Shift\", label, \"%s\", none, none, \"0\", \"0\", 175, 720)\n     " +

                //// parameters
                //"         parameter(\"\", \"bolt_main_screwdin\", bolt_standard, text, 170, 380, 100)\n  " +
                //"         parameter(\"\", \"bolt_main_diameter\", bolt_size, number, 170,410, 100)\n  " +
                //"         parameter(\"\", \"tolerance_main\", distance, number, 170,440, 100)\n  " +
                //"   attribute(\"cb_workshop_1\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 170, 470, 100)\n  " +
                //"  {\n     " +
                //"   value(\"Workshop\", 0)\n      " +
                //"  value(\"Site\", 1)\n  " +
                //"  }\n            " +
                //   "   attribute(\"cm_washerNo_1\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 30, 550, 100)\n  " +
                //"  {\n     " +
                //"   value(\"1 Washer\", 1)\n      " +
                //"  value(\"2 Washer\", 0)\n  " +
                //"  }\n            " +

                //    "   attribute(\"cm_nutNo_1\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 175, 550, 100)\n  " +
                //"  {\n     " +
                //"   value(\"1 Nut\", 1)\n      " +
                //"  value(\"2 Nut\", 0)\n  " +
                //"  }\n" +

                //"         parameter(\"\", \"slotX_1\", distance, number, 75,610, 50)\n  " +
                //"         parameter(\"\", \"slotY_1\", distance, number, 165,610, 50)\n  " +


                //    "   attribute(\"cb_sloted_1\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 250, 610, 80)\n  " +
                //"  {\n     " +
                //"   value(\"Plate\", 0)\n      " +
                //"  value(\"Beam\", 0)\n  " +
                //"   value(\"none\", 1)\n      " +

                //"  }\n" +

                //    "   attribute(\"cb_weldedBolt_1\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 20, 675, 150)\n  " +
                //"  {\n     " +
                // "   value(\"Beam_to_Beam_Clip1.xbm\", 1)\n      " +
                //"  value(\"Beam_to_Beam_Clip2.xbm\", 0)\n  " +
                //"  }\n" +

                //"         parameter(\"\", \"polybeam_weldWithMain\", distance, number, 265,675, 50)\n  " +
                //"         parameter(\"\", \"bolt_shift_1\", distance, number, 265,720, 50)\n  " +
                ////pic
                //"  picture(\"bolt_anglepeb_plugin\", 0, 0, 328, 374)\n      " +


                // // sec bolt

                // //labels
                // "   attribute(\"\", \"Secondary Bolts\", label, \"%s\", none, none, \"0\", \"0\", 670, 350)\n     " +
                // "   attribute(\"\", \"Bolt Standard\", label, \"%s\", none, none, \"0\", \"0\", 625, 380)\n     " +
                // "   attribute(\"\", \"Bolt Size\", label, \"%s\", none, none, \"0\", \"0\", 625, 410)\n     " +
                // "   attribute(\"\", \"Tolerance\", label, \"%s\", none, none, \"0\", \"0\", 625, 440)\n     " +
                // "   attribute(\"\", \"Workshop/Site\", label, \"%s\", none, none, \"0\", \"0\", 625, 470)\n     " +
                // "   attribute(\"\", \"Washer\", label, \"%s\", none, none, \"0\", \"0\", 625, 510)\n     " +
                // "   attribute(\"\", \"Nut\", label, \"%s\", none, none, \"0\", \"0\", 785, 510)\n     " +
                // "   attribute(\"\", \"X\", label, \"%s\", none, none, \"0\", \"0\", 680, 585)\n     " +
                // "   attribute(\"\", \"Y\", label, \"%s\", none, none, \"0\", \"0\", 770, 585)\n     " +
                // "   attribute(\"\", \"Slot\", label, \"%s\", none, none, \"0\", \"0\", 600, 615)\n     " +
                // "   attribute(\"\", \"Weld Size\", label, \"%s\", none, none, \"0\", \"0\", 855, 650)\n     " +
                // "   attribute(\"\", \"Bolt Shift\", label, \"%s\", none, none, \"0\", \"0\", 760, 720)\n     " +

                //// parameters
                //"         parameter(\"\", \"bolt_sec_screwdin\", bolt_standard, text, 755, 380, 100)\n  " +
                //"         parameter(\"\", \"bolt_sec_diameter\", bolt_size, number, 755,410, 100)\n  " +
                //"         parameter(\"\", \"tolerance_sec\", distance, number, 755,440, 100)\n  " +
                //"   attribute(\"cb_workshop_2\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 755, 470, 100)\n  " +
                //"  {\n     " +
                //"   value(\"Workshop\", 1)\n      " +
                //"  value(\"Site\", 0)\n  " +
                //"  }\n            " +
                //   "   attribute(\"cm_washerNo_2\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 625, 550, 100)\n  " +
                //"  {\n     " +
                //"   value(\"1 Washer\", 1)\n      " +
                //"  value(\"2 Washer\", 0)\n  " +
                //"  }\n            " +

                //    "   attribute(\"cm_nutNo_2\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 760, 550, 100)\n  " +
                //"  {\n     " +
                //"   value(\"1 Nut\", 1)\n      " +
                //"  value(\"2 Nut\", 0)\n  " +
                //"  }\n" +

                //"         parameter(\"\", \"slotX_2\", distance, number, 690,610, 50)\n  " +
                //"         parameter(\"\", \"slotY_2\", distance, number, 770,610, 50)\n  " +


                //    "   attribute(\"cb_solted_2\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 860, 610, 80)\n  " +
                //"  {\n     " +
                //"   value(\"Plate\", 0)\n      " +
                //"  value(\"Beam\", 0)\n  " +
                //"  value(\"none\", 1)\n  " +
                //"  }\n" +

                //    "   attribute(\"cb_weldedBolt_2\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 565, 675, 150)\n  " +
                //"  {\n     " +
                //"   value(\"Beam_to_Beam_Clip1.xbm\", 1)\n      " +
                //"  value(\"Beam_to_Beam_Clip3.xbm,\", 0)\n  " +
                //"  }\n" +

                //"         parameter(\"\", \"polybeam_weldWithSec\", distance, number, 850,675, 50)\n  " +
                //"         parameter(\"\", \"bolt_shift_2\", distance, number, 850,720, 50)\n  " +
                ////pic
                //"  picture(\"bolt_anglepeb_plugin\", 0, 0, 913, 374)\n      " +







                "  }\n   " +




                " }\n}\n";



        }
    }
}