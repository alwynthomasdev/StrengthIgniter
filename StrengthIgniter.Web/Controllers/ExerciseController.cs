using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StrengthIgniter.Web.Controllers
{
    public class ExerciseController : Controller
    {
        public IActionResult Index()
        {
            //TODO: load exercises
            return View();
        }
    }
}
