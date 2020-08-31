using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinearEquationSolver.Service
{
    public interface IAPIService
    {
        string SolveEquation(string equation);
    }
}
