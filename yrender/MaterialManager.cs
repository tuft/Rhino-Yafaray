using System;
using System.Collections.Generic;
using RMA.Rhino;
using System.Text;
using System.Xml;
using System.IO;

namespace yrender
{
    class MaterialManager 
    {
        Dictionary<string, Material> m;
        
        public MaterialManager() 
        {
            m = new Dictionary<string, Material>();
            m.Add("defaultMat", new Material("<type sval=\"shinydiffusemat\"/>"));
        }

        public void create(string name)
        {
            if (!this.exists(name))
                m.Add(name, new Material());
        }

        public void create(string name, string definition)
        {
            if(!this.exists(name))
                m.Add(name, new Material(definition));
        }


        public List<string> getNames()
        {
            return new List<string>(m.Keys);
        }

        public Material get(string name)
        {
            return m[name];
        }
        public bool exists(string name) 
        {
            return m.ContainsKey(name);
        }

        //used for saving to rhino document and also for xml export
        public string serialize() 
        {
            StringBuilder buffer = new StringBuilder(); 
            foreach(KeyValuePair<string, Material> mat in m)
            {
                buffer.Append("<material name=\"" + mat.Key + "\">");
                buffer.Append(mat.Value.serialize());
                buffer.Append("</material>");
            }
            return buffer.ToString();
        }

        public void load(TextReader input)
        {
            XmlTextReader reader = new XmlTextReader(input);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "material")
                {
                    string matName = "";
                    while (reader.MoveToNextAttribute())
                    {
                        Utils.print(reader.Name);
                        if (reader.Name == "name")
                        {
                            matName = reader.Value;
                        }
                    }
                    reader.MoveToElement();
                    string matDefinitoion = reader.ReadInnerXml();
                    this.create(matName, matDefinitoion);
                }
            }
        }
        public void load(string xmlText) 
        { 
            StringReader buffer = new StringReader(xmlText);
            this.load(buffer);
        }

    }
}