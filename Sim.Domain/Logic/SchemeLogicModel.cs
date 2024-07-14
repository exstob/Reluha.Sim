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
        static dynamic InputContactStates = new ExpandoObject();
        public static void AddProperty(string propertyName, object propertyValue)
        {
            var expandoDict = InputContactStates as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        public static object GetProperty(string propertyName)
        {
            var expandoDict = InputContactStates as IDictionary<string, object>;
            return expandoDict[propertyName];
        }
    }
}
