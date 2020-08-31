using LinearEquationSolver.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinearEquationSolver.Service
{
    public class APIService : IAPIService
    {
        public string SolveEquation(string equation)
        {
            Sparse2DMatrix<int, int, double> aMatrix = new Sparse2DMatrix<int, int, double>();
            SparseArray<int, double> bVector = new SparseArray<int, double>();
            SparseArray<string, int> variableNameIndexMap = new SparseArray<string, int>();
            int numberOfEquations = 0;
           
            LinearEquationParser parser = new LinearEquationParser();
            LinearEquationParserStatus parserStatus = LinearEquationParserStatus.Success;
            string[] lines = equation.Split(new[] { "\r\n", "\r", "\n" , "\\n", ","}, StringSplitOptions.None);

            foreach (string inputLine in lines)
            {
                parserStatus = parser.Parse(inputLine,
                                            aMatrix,
                                            bVector,
                                            variableNameIndexMap,
                                            ref numberOfEquations);

                if (parserStatus != LinearEquationParserStatus.Success)
                {
                    break;
                }
            }

            // Assume success.
            string mainStatusBarText = "Equations solved";

            // Did an error occur?
            if (parserStatus == LinearEquationParserStatus.Success)
            {
                // Are there the same number of equations as variables?
                if (numberOfEquations == variableNameIndexMap.Count)
                {
                    // Create a solution vector.
                    SparseArray<int, double> xVector = new SparseArray<int, double>();

                    // Solve the simultaneous equations.
                    LinearEquationSolverStatus solverStatus =
                        LinearEquationSolver.Solve(numberOfEquations,
                                                   aMatrix,
                                                   bVector,
                                                   xVector);

                    if (solverStatus == LinearEquationSolverStatus.Success)
                    {
                        string solutionString = "";
                        foreach (KeyValuePair<string, int> pair in variableNameIndexMap)
                        {
                            solutionString += string.Format("{0} = {1}" + " ", pair.Key, xVector[pair.Value]);
                        }

                        return solutionString;

                    }
                    else if (solverStatus == LinearEquationSolverStatus.IllConditioned)
                    {
                        mainStatusBarText = "Error - the system of equations is ill conditioned.";
                    }
                    else if (solverStatus == LinearEquationSolverStatus.Singular)
                    {
                        mainStatusBarText = "Error - the system of equations is singular.";
                    }
                }
                else if (numberOfEquations < variableNameIndexMap.Count)
                {
                   
                    mainStatusBarText = string.Format("Only {0} equations is too few equations for {1} variables", numberOfEquations, variableNameIndexMap.Count);
                }
                else if (numberOfEquations > variableNameIndexMap.Count)
                {
                   
                    mainStatusBarText = string.Format("{0} equations is too many equations for only {1} variables", numberOfEquations, variableNameIndexMap.Count);
                }
            }
            else
            {
                // An error occurred. Report the error in the status bar.
                mainStatusBarText = LinearEquationParserStatusInterpreter.GetStatusString(parserStatus);
            }

            return mainStatusBarText;

        }
    }
}
