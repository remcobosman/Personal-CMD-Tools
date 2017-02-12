/*
* Arguments class: application arguments interpreter
*
* Authors:		R. LOPES
* Contributors:	R. LOPES
* Created:		25 October 2002
* Modified:		28 October 2002
*
* Version:		1.0
*/

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Rbit.CommandLineTool.Support
{
    /// <summary>
    /// Arguments class
    /// </summary>
    public class Arguments
    {
        // Variables
        private readonly StringDictionary _parameters;

        // Constructor
        public Arguments(IEnumerable<string> arguments)
        {
            _parameters = new StringDictionary();
            var spliter = new Regex(@"^-{1,2}|^/", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            string parameter = null;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: -param1 value1 --param2 /param3:"Test-:-work" /param4=happy -param5 '--=nice=--'
            foreach (var argument in arguments)
            {
                // Look for new parameters (-,/ or --) and a possible enclosed value (=,:)
                var parts = spliter.Split(argument, 3);
                switch (parts.Length)
                {
                    // Found a value (for the last parameter found (space separator))
                    case 1:
                        if (parameter != null)
                        {
                            if (!_parameters.ContainsKey(parameter))
                            {
                                parts[0] = remover.Replace(parts[0], "$1");
                                _parameters.Add(parameter, parts[0]);
                            }
                            parameter = null;
                        }
                        // else Error: no parameter waiting for a value (skipped)
                        break;
                    // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!_parameters.ContainsKey(parameter)) _parameters.Add(parameter, "true");
                        }
                        parameter = parts[1];
                        break;
                    // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!_parameters.ContainsKey(parameter)) _parameters.Add(parameter, "true");
                        }
                        parameter = parts[1];
                        // Remove possible enclosing characters (",')
                        if (!_parameters.ContainsKey(parameter))
                        {
                            parts[2] = remover.Replace(parts[2], "$1");
                            _parameters.Add(parameter, parts[2]);
                        }
                        parameter = null;
                        break;
                }
            }
            // In case a parameter is still waiting
            if (parameter == null) return;
            if (!_parameters.ContainsKey(parameter)) _parameters.Add(parameter, "true");
        }

        // Retrieve a parameter value if it exists
        public string this[string key]
        {
            get { return (_parameters[key]); }
        }

        public bool Contains(string key
            )
        {
            return _parameters.ContainsKey(key);
        }
    }
}