using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
                                                                                                                                                                                                        
namespace LinearEquationSolver.Helper
{
    public static class LinearEquationParserStatusInterpreter
    {
      
        public static string GetStatusString(LinearEquationParserStatus status)
        {
            string statusString = "";

            switch (status)
            {
                case LinearEquationParserStatus.Success:
                case LinearEquationParserStatus.SuccessNoEquation:
                    statusString = "The equation was parsed successfully.";
                    break;
                case LinearEquationParserStatus.ErrorIllegalEquation:
                    statusString = "The equation syntax is illegal.";
                    break;
                case LinearEquationParserStatus.ErrorNoEqualSign:
                    statusString = "There is no equal sign in the equation.";
                    break;
                case LinearEquationParserStatus.ErrorMultipleEqualSigns:
                    statusString = "There are multiple equal signs in the equation.";
                    break;
                case LinearEquationParserStatus.ErrorNoTermBeforeEqualSign:
                    statusString = "There is no term before the equal sign in the equation.";
                    break;
                case LinearEquationParserStatus.ErrorNoTermAfterEqualSign:
                    statusString = "There is no term after the equal sign in the equation.";
                    break;
                case LinearEquationParserStatus.ErrorNoTermEncountered:
                    statusString = "A number or a variable was expected.";
                    break;
                case LinearEquationParserStatus.ErrorNoVariableInEquation:
                    statusString = "There is no variable in the equation.";
                    break;
                case LinearEquationParserStatus.ErrorMultipleDecimalPoints:
                    statusString = "A number contains more than one decimal point.";
                    break;
                case LinearEquationParserStatus.ErrorTooManyDigits:
                    statusString = "A number contains more than 15 digits.";
                    break;
                case LinearEquationParserStatus.ErrorMissingExponent:
                    statusString = "A number contains the '^' character and is missing an exponent.";
                    break;
                case LinearEquationParserStatus.ErrorIllegalExponent:
                    statusString = "A number contains an illegal exponent.";
                    break;
                default:
                    statusString = "The equation syntax is illegal.";
                    break;
            }

            return statusString;
        }
    }
}
