using RMA.Rhino;
using RMA.OpenNURBS;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace yrender
{   
    public class YRenderPlugIn : RMA.Rhino.MRhinoRenderPlugIn
    {
        MaterialManager materials;
        
        public override System.Guid PlugInID()
        {
            return new System.Guid("{eb30e6b4-949a-431f-b46f-b7fe4726d073}");
        }

        /// <returns>Plug-In name as displayed in the plug-in manager dialog</returns>
        public override string PlugInName()
        {
            return "YRender";
        }

        ///<returns>Version information for this plug-in</returns>
        public override string PlugInVersion()
        {
            return "1.0.0.0";
        }

        public override int OnLoadPlugIn()
        {
            this.materials = new MaterialManager();            
            return 1;
        }

        public override void OnUnloadPlugIn()
        {
            // TODO: Add plug-in cleanup code here.
        }

        //public override bool EnableAssignMaterialButton() { return true; }
        public override bool EnableEditMaterialButton(ref OnMaterial material) { return true; }
        //public override bool EnableCreateMaterialButton() { return true; }

        /*public override bool OnAssignMaterial(ref System.Windows.Forms.IWin32Window hwndParent, OnMaterial material)
        {
            return true;
        }*/
        /* public override bool OnAssignMaterial(System.Windows.Forms.IWin32Window hwndParent, ref OnMaterial material)
        {
            ymateditor mateditor = new ymateditor();
            mateditor.ShowDialog(hwndParent);
            //return IRhinoCommand.result.success;
            //material.m_plugin_id
            return true;
            //return base.OnAssignMaterial(hwndParent, ref material);
        }*/
        public override bool OnEditMaterial(System.Windows.Forms.IWin32Window hwndParent, ref OnMaterial rhinoMaterial)
        {
            ymateditor mateditor = new ymateditor(ref rhinoMaterial, ref this.materials);
            mateditor.ShowDialog(hwndParent);
            return true;
        }



        public override IRhinoCommand.result RenderWindow(RMA.Rhino.IRhinoCommandContext context, bool render_preview, RMA.Rhino.MRhinoView view, System.Drawing.Rectangle rect, bool bInWindow)
        {
            //not supported for now -> just render
            return this.Render(context, render_preview);
        }

        //writing to file
        //returns true if we have any data to wite
        /*public override bool CallWriteDocument(IRhinoFileWriteOptions options)
        {
            //only return true if you REALLY want to save something to the document
            //that is about to be written to disk
            if (options.Mode(IRhinoFileWriteOptions.ModeFlag.SelectedMode) == true) return false;
            if (options.Mode(IRhinoFileWriteOptions.ModeFlag.AsVersion2) == true) return false;
            if (options.Mode(IRhinoFileWriteOptions.ModeFlag.AsVersion3) == true) return false;
 
            //perform some other type of check to see if you need to save any data...
            //If( IHaveDataToWrite() = False ) Then Return False
 
            return false;
        }
        
        //If any ON_BinaryArchive::Write*() functions return false than you should 
        //immediately return false otherwise return true if all data was written
        //successfully.  Returning false will cause Rhino to stop writing this document.
        public override bool WriteDocument(MRhinoDoc doc, OnBinaryArchive archive, IRhinoFileWriteOptions options)
        {
            //This function is called because CallWriteDocument returned True.
            //Write your plug-in data to the document
 
            string date_string  = System.DateTime.Now.ToShortDateString();
            string time_string = System.DateTime.Now.ToShortTimeString();
 
            ///It is a good idea to always start with a version number
            //so you can modify your document read/write code in the future
            if (archive.Write3dmChunkVersion(1, 0) == false) return false;
            if (archive.WriteString(date_string) == false) return false;
            if (archive.WriteString(time_string) == false) return false;
 
            return true;
        }*/

        public override IRhinoCommand.result Render(RMA.Rhino.IRhinoCommandContext context, bool render_preview)
        {

            /*RenderSettings settings = new RenderSettings();
            if (settings.ShowDialog() == DialogResult.OK)*/

            {


               
               

                //use . instead of cz , for floats in String.Format
                System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CreateSpecificCulture("en");

                //todo: more elegant
                MRhinoObjectIterator it = new MRhinoObjectIterator(context.m_doc, IRhinoObjectIterator.object_state.normal_or_locked_objects,
                                                          IRhinoObjectIterator.object_category.active_and_reference_objects);
                it.IncludeLights(false);
                it.IncludePhantoms(false);

                //fill list with objects from iterator
                List<IRhinoObject> objs = new List<IRhinoObject>();
                foreach (MRhinoObject obj in it)
                {
                    objs.Add(obj);
                }

                //mesh selected objects from list
                IRhinoAppRenderMeshSettings rms = RhUtil.RhinoApp().AppSettings().RenderMeshSettings();
                OnMeshParameters mp = new OnMeshParameters(rms.FastMeshParameters());

                int ui_style = 1; // simple ui
                ArrayMRhinoObjectMesh meshes = new ArrayMRhinoObjectMesh();
                IRhinoCommand.result rc = RhUtil.RhinoMeshObjects(objs.ToArray(),
                                                                  ref mp,
                                                                  ref ui_style,
                                                                  ref meshes);
                //if anything was meshed:
                if (rc == IRhinoCommand.result.success)
                {

                    // open file 
                    FileStream file = new FileStream("scene.xml", FileMode.Create, FileAccess.Write);
                    TextWriter tw = new StreamWriter(file);

                    tw.WriteLine("<?xml version=\"1.0\"?>");
                    tw.WriteLine("<scene type=\"triangle\">");

                    //materials
                    tw.WriteLine(materials.serialize());

                    

                    /*if (materialDefinition.Trim() != "")
                    {
                        materialName = material.m_material_id.ToString();
                        tw.WriteLine(string.Format(ci, "<material name=\"{0}\">", materialName));
                        tw.WriteLine(materialDefinition);
                        tw.WriteLine("</material>");
                    }*/
                    /*tw.WriteLine("<material name=\"defaultMat\">");
                tw.WriteLine("	<type sval=\"shinydiffusemat\"/>");
                tw.WriteLine("</material>");*/



                    // write meshes geometry
                    // todo: normals
                    for (int i = 0; i < meshes.Count(); i++)
                    {
                        IRhinoMaterial material = meshes[i].m_parent_object.ObjectMaterial();
                        string materialName = "";                    
                        material.GetUserString("yafaray_material", ref materialName);
                        if (materialName == "") materialName = "defaultMat";
                        string meshObject = writeMeshObject(meshes[i], materialName);
                        tw.WriteLine(meshObject);
                        //TODO: get somewhere smoothing angle, or better rhino should generate normals...  
                        tw.WriteLine(string.Format(ci, "<smooth ID=\"{0}\" angle=\"30\"/>", i + 1));

                    }

                    tw.WriteLine(cameraFromActiveViewport());

                    //todo: scene setting dialog
                    tw.WriteLine("<background name=\"world_background\">");
                    tw.WriteLine("	<a_var fval=\"1\"/>");
                    tw.WriteLine("	<add_sun bval=\"true\"/>");
                    tw.WriteLine("	<b_var fval=\"1\"/>");
                    tw.WriteLine("	<background_light bval=\"true\"/>");
                    tw.WriteLine("	<c_var fval=\"1\"/>");
                    tw.WriteLine("	<d_var fval=\"1\"/>");
                    tw.WriteLine("	<e_var fval=\"1\"/>");
                    tw.WriteLine("	<from x=\"1\" y=\"1\" z=\"1\"/>");
                    tw.WriteLine("	<light_samples ival=\"8\"/>");
                    tw.WriteLine("	<power fval=\"1\"/>");
                    tw.WriteLine("	<sun_power fval=\"1\"/>");
                    tw.WriteLine("	<turbidity fval=\"3\"/>");
                    tw.WriteLine("	<type sval=\"sunsky\"/>");
                    tw.WriteLine("</background>");
                    tw.WriteLine("");
                    tw.WriteLine("<integrator name=\"default\">");
                    tw.WriteLine("	<caustics bval=\"false\"/>");
                    tw.WriteLine("	<raydepth ival=\"4\"/>");
                    tw.WriteLine("	<shadowDepth ival=\"4\"/>");
                    tw.WriteLine("	<transpShad bval=\"false\"/>");
                    tw.WriteLine("	<type sval=\"directlighting\"/>");
                    tw.WriteLine("</integrator>");
                    tw.WriteLine("");
                    tw.WriteLine("<integrator name=\"default2\">");
                    tw.WriteLine("	<caustics bval=\"false\"/>");
                    tw.WriteLine("	<raydepth ival=\"4\"/>");
                    tw.WriteLine("	<shadowDepth ival=\"4\"/>");
                    tw.WriteLine("	<transpShad bval=\"false\"/>");
                    tw.WriteLine("	<type sval=\"directlighting\"/>");
                    tw.WriteLine("</integrator>");
                    tw.WriteLine("<integrator name=\"volintegr\">");
                    tw.WriteLine("	<type sval=\"none\"/>");
                    tw.WriteLine("</integrator>");
                    tw.WriteLine("<integrator name=\"default3\">");
                    tw.WriteLine("	<caustics bval=\"false\"/>");
                    tw.WriteLine("	<raydepth ival=\"4\"/>");
                    tw.WriteLine("	<shadowDepth ival=\"4\"/>");
                    tw.WriteLine("	<transpShad bval=\"false\"/>");
                    tw.WriteLine("	<type sval=\"directlighting\"/>");
                    tw.WriteLine("</integrator>");
                    tw.WriteLine("<integrator name=\"volintegr\">");
                    tw.WriteLine("	<type sval=\"none\"/>");
                    tw.WriteLine("</integrator>");
                    tw.WriteLine("<integrator name=\"default4\">");
                    tw.WriteLine("	<caustics bval=\"false\"/>");
                    tw.WriteLine("	<raydepth ival=\"4\"/>");
                    tw.WriteLine("	<shadowDepth ival=\"4\"/>");
                    tw.WriteLine("	<transpShad bval=\"false\"/>");
                    tw.WriteLine("	<type sval=\"directlighting\"/>");
                    tw.WriteLine("</integrator>");
                    tw.WriteLine("<integrator name=\"volintegr\">");
                    tw.WriteLine("	<type sval=\"none\"/>");
                    tw.WriteLine("</integrator>");
                    tw.WriteLine("<integrator name=\"default5\">");
                    tw.WriteLine("	<caustics bval=\"false\"/>");
                    tw.WriteLine("	<raydepth ival=\"4\"/>");
                    tw.WriteLine("	<shadowDepth ival=\"4\"/>");
                    tw.WriteLine("	<transpShad bval=\"false\"/>");
                    tw.WriteLine("	<type sval=\"directlighting\"/>");
                    tw.WriteLine("</integrator>");
                    tw.WriteLine("<integrator name=\"volintegr\">");
                    tw.WriteLine("	<type sval=\"none\"/>");
                    tw.WriteLine("</integrator>");
                    tw.WriteLine("<integrator name=\"default6\">");
                    tw.WriteLine("	<caustics bval=\"false\"/>");
                    tw.WriteLine("	<raydepth ival=\"4\"/>");
                    tw.WriteLine("	<shadowDepth ival=\"4\"/>");
                    tw.WriteLine("	<transpShad bval=\"false\"/>");
                    tw.WriteLine("	<type sval=\"directlighting\"/>");
                    tw.WriteLine("</integrator>");
                    tw.WriteLine("<integrator name=\"volintegr\">");
                    tw.WriteLine("	<type sval=\"none\"/>");
                    tw.WriteLine("</integrator>");
                    tw.WriteLine("<integrator name=\"default7\">");
                    tw.WriteLine("	<caustics bval=\"false\"/>");
                    tw.WriteLine("	<raydepth ival=\"4\"/>");
                    tw.WriteLine("	<shadowDepth ival=\"4\"/>");
                    tw.WriteLine("	<transpShad bval=\"false\"/>");
                    tw.WriteLine("	<type sval=\"directlighting\"/>");
                    tw.WriteLine("</integrator>");
                    tw.WriteLine("<integrator name=\"volintegr\">");
                    tw.WriteLine("	<type sval=\"none\"/>");
                    tw.WriteLine("</integrator>");




                    tw.WriteLine("");
                    tw.WriteLine("<render>");
                    tw.WriteLine("	<AA_inc_samples ival=\"1\"/>");
                    tw.WriteLine("	<AA_minsamples ival=\"1\"/>");
                    tw.WriteLine("	<AA_passes ival=\"1\"/>");
                    tw.WriteLine("	<AA_pixelwidth fval=\"1.5\"/>");
                    tw.WriteLine("	<AA_threshold fval=\"0.05\"/>");
                    tw.WriteLine("	<background_name sval=\"world_background\"/>");
                    tw.WriteLine("	<camera_name sval=\"cam\"/>");
                    tw.WriteLine("	<clamp_rgb bval=\"false\"/>");
                    tw.WriteLine("	<filter_type sval=\"box\"/>");
                    tw.WriteLine("	<gamma fval=\"1.8\"/>");
                    tw.WriteLine("	<height ival=\"600\"/>");
                    tw.WriteLine("	<integrator_name sval=\"default\"/>");
                    tw.WriteLine("	<threads ival=\"1\"/>");
                    tw.WriteLine("	<volintegrator_name sval=\"volintegr\"/>");
                    tw.WriteLine("	<width ival=\"800\"/>");
                    tw.WriteLine("	<xstart ival=\"0\"/>");
                    tw.WriteLine("	<ystart ival=\"0\"/>");
                    tw.WriteLine("	<z_channel bval=\"true\"/>");
                    tw.WriteLine("</render>");
                    tw.WriteLine("</scene>");

                    tw.Close();
                    file.Close();

                    //run yafaray 
                    //todo: configurable path
                    System.Diagnostics.Process objProcess = new Process();
                    ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c \"c:\\Program Files\\YafaRay\\yafaray-xml.exe\" scene.xml");
                    Process.Start(psi);

                    context.m_doc.Redraw();
                }

            }
            return IRhinoCommand.result.success;

        }

        //reference film size in yafray is 1
        public string cameraFromActiveViewport()
        {
            System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CreateSpecificCulture("en");

            MRhinoViewport viewport = RhUtil.RhinoApp().ActiveView().ActiveViewport();
            IOn3dVector to = viewport.VP().CameraDirection();
            IOn3dPoint from = viewport.VP().CameraLocation();
            IOn3dVector up = viewport.VP().CameraUp();

            string camera_xml = "";

            camera_xml += string.Format(ci, "<camera name=\"cam\">");

            //todo: 
            //tw.WriteLine("<aperture fval=\"0\"/>");
            //tw.WriteLine("<bokeh_type sval=\"disk1\"/>");
            //tw.WriteLine("<dof_distance fval=\"0\"/>");

            camera_xml += string.Format(ci, "<resx ival=\"800\"/>");
            camera_xml += string.Format(ci, "<resy ival=\"600\"/>");
            camera_xml += string.Format(ci, "<type sval=\"perspective\"/>");


            /*
             todo:  
                
             if renderData.sizeX * renderData.aspectX <= renderData.sizeY * renderData.aspectY:
                f_aspect = (renderData.sizeX * renderData.aspectX) / (renderData.sizeY * renderData.aspectY)

            #print "f_aspect: ", f_aspect
            yi.paramsSetFloat("focal", camera.lens/(f_aspect*32.0))
 
             */

            //////////angle;
            double lense = 0;
            viewport.VP().GetCamera35mmLenseLength(ref lense);

            camera_xml += string.Format(ci, "<focal fval=\"{0}\"/>", lense / 35);

            camera_xml += string.Format(ci, "<from x=\"{0}\" y=\"{1}\" z=\"{2}\"/>", from.x, from.y, from.z);
            camera_xml += string.Format(ci, "<to x=\"{0}\" y=\"{1}\" z=\"{2}\"/>", to.x, to.y, to.z);
            camera_xml += string.Format(ci, "<up x=\"{0}\" y=\"{1}\" z=\"{2}\"/>", up.x, up.y, up.z);

            camera_xml += string.Format(ci, "</camera>");

            return camera_xml;
        }

        //writing mesh, material is now set to default but will be selected 
        //by layer or object, material definitions will be got from material table
        public string writeMeshObject(MRhinoObjectMesh rhinoMesh, string materialName)
        {
            OnMesh mesh = new OnMesh();
            mesh = rhinoMesh.GetMesh();
            System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CreateSpecificCulture("en");

            //use buffer for strings (much faster than string + string... )
            StringBuilder buffer = new StringBuilder(); 

            //materials
            //IRhinoMaterial material = rhinoMesh.m_parent_object.ObjectMaterial();
            //string materialName = material.m_material_id.ToString();

            

            //mesh
            int v_count = mesh.m_V.Count();
            int f_count = mesh.m_F.Count();
            
            buffer.Append(string.Format(ci, "<mesh vertices=\"{0}\" faces=\"{1}\" has_orco=\"off\" has_uv=\"false\" type=\"0\">", v_count, f_count));
            buffer.Append(string.Format(ci, "<set_material sval=\"{0}\"/>", materialName));

            //list verticles
            foreach (On3fPoint v in mesh.m_V)
            {
                buffer.Append(string.Format(ci, " <p x=\"{0}\" y=\"{1}\" z=\"{2}\"/>", v.x, v.y, v.z));
                v_count++;
            }

            //list faces
            foreach (OnMeshFace f in mesh.m_F)
            {
                buffer.Append(string.Format(ci, " <f a=\"{0}\" b=\"{1}\" c=\"{2}\"/>\n", f.get_vi(0), f.get_vi(1), f.get_vi(2)));
                //quad ?
                if (!f.IsTriangle()) buffer.Append(string.Format(ci, " <f a=\"{0}\" b=\"{1}\" c=\"{2}\"/>\n", f.get_vi(0), f.get_vi(2), f.get_vi(3)));
                f_count++;
            }

            buffer.Append(string.Format(ci, "</mesh>"));
            mesh.Destroy();
            return buffer.ToString();
        }
        
    }
}
