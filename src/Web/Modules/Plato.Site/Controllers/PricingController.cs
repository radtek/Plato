﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatoCore.Layout.ModelBinding;

namespace Plato.Site.Controllers
{
    public class PricingController : Controller, IUpdateModel
    {

        // ---------------------
        // Index
        // ---------------------

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> Index()
        {
            // Return view
            return Task.FromResult((IActionResult) View());

        }

        [HttpGet, AllowAnonymous]
        public Task<IActionResult> FullyManaged()
        {
            // Return view
            return Task.FromResult((IActionResult)View());

        }

    }

}
