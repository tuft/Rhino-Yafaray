///<summary>
/// The following class is required for all Rhino.NET plug-ins.
/// These are used to display plug-in information in the plug-in manager.
/// Any string will work for these attributes, so if you don't have a fax
/// number it is OK to enter something like "none"
///</summary>
namespace yrender
{
    public class yrenderPlugInAttributes : RMA.Rhino.MRhinoPlugInAttributes
    {
        public override string Address()
        {
            return "123 Programmer Lane\nCity\nState\n12345-6789";
        }
        public override string Country()
        {
            return "USA";
        }
        public override string Email()
        {
            return "product.support@mycompany.com";
        }
        public override string Fax()
        {
            return "987.654.3210";
        }
        public override string Organization()
        {
            return "Company Name";
        }
        public override string Phone()
        {
            return "123.456.7890";
        }
        public override string UpdateURL()
        {
            return "http://updates.mycompany.com";
        }
        public override string Website()
        {
            return "http://www.mycompany.com";
        }
    }
}
