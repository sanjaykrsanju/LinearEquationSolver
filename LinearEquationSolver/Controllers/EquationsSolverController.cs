using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LinearEquationSolver.Service;
using LinearEquationSolver.Model;
using System.IO;
using System.Text;

namespace LinearEquationSolver.Controllers
{
    [Route("api/v1/EquationsSolver")]
    [ApiController]
    public class EquationsSolverController : ControllerBase
    {
        private readonly IAPIService _apiService;

        public EquationsSolverController(IAPIService ApiService)
        {
            _apiService = ApiService;
        }

        /// <summary>
        /// Provide input in plan text format with comma(,) seprated equation
        /// </summary>
        /// <param name="inputEquiation"></param>
        /// <returns></returns>
        [HttpGet (Name = "Solve")]
        public ActionResult<string> Solve(string inputEquiation)
        {
            string solutionString = _apiService.SolveEquation(inputEquiation);

            return solutionString;
        }

        /// <summary>
        /// Send apiInputDocument DocumentContent fild as Base64 converted
        /// </summary>
        /// <param name="apiInputDocument"></param>
        /// <returns></returns>
        [HttpPost (Name = "Solve")]
        [DisableRequestSizeLimit]
        public ActionResult<string> Solve([FromBody]APIInputDocument apiInputDocument)
        {
            string inputEquiation = "";
            byte[] byteArrary = Convert.FromBase64String(apiInputDocument.DocumentContent);
            using (var stream = new MemoryStream(byteArrary))
            {
                inputEquiation = Encoding.ASCII.GetString(stream.ToArray());
            }
           string solutionString = _apiService.SolveEquation(inputEquiation);

            return solutionString;
        }
    }
}