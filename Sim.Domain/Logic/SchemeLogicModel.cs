using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.Logic
{
    public class SchemeLogicModel
    {
        dynamic inputContactStates = new ExpandoObject();

        public SchemeLogicModel()
        {
            inputContactStates = new ExpandoObject();
        }
        public void AddProperty(string propertyName, object propertyValue)
        {
            var expandoDict = inputContactStates as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        public object GetProperty(string propertyName)
        {
            var expandoDict = inputContactStates as IDictionary<string, object>;
            return expandoDict[propertyName];
        }
    }
}
