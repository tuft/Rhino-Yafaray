using RMA.Rhino;
using RMA.OpenNURBS;

namespace yrender
{
    ///<summary>
    /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
    /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
    /// A command wizard can be found in visual studio when adding a new item to the project.
    /// </summary>
    public class yrenderCommand : RMA.Rhino.MRhinoCommand
    {
        ///<summary>
        /// Rhino tracks commands by their unique ID. Every command must have a unique id.
        /// The Guid created by the project wizard is unique. You can create more Guids using
        /// the "Create Guid" tool in the Tools menu.
        ///</summary>
        ///<returns>The id for this command</returns>
        public override System.Guid CommandUUID()
        {
            return new System.Guid("{846b0392-96f6-4a45-a044-0ee5b9a4023b}");
        }

        ///<returns>The command name as it appears on the Rhino command line</returns>
        public override string EnglishCommandName()
        {
            return "yrender";
        }

        ///<summary> This gets called when when the user runs this command.</summary>
        public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
        {
            RhUtil.RhinoApp().Print(string.Format("The {0} command is under construction\n", EnglishCommandName()));
            return IRhinoCommand.result.success;
        }
    }
}

