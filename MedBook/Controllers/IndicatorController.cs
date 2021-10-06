using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Controllers
{
    public class IndicatorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
