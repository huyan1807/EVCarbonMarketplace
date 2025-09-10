using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Exceptions
{
    public class ModelValidationException : Exception
    {
        public Dictionary<string, List<string>> Errors { get; }

        public ModelValidationException(Dictionary<string, List<string>> errors)
            : base("One or more validation errors occurred.")
        {
            Errors = errors ?? new Dictionary<string, List<string>>();
        }

        public ModelValidationException(string field, string errorMessage)
            : base("One or more validation errors occurred.")
        {
            Errors = new Dictionary<string, List<string>>
            {
                { field, new List<string> { errorMessage } }
            };
        }
    }
}
