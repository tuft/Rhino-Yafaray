using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yrender
{    
    class Material
    {
        public string definition = "";

        public Material():this("")
        {
        }
        public Material(string definition) 
        {
            this.definition = definition;
        }      
       
       
        //used for saving to rhino document and also for xml export
        public string serialize() 
        {
            return this.definition;
        }
        public void setDefinition(string definition) 
        { 
            this.definition = definition;
        }

    }
}
