﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Docs.Models;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Docs.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<AdminIndex>
    {

        public override Task<IViewProviderResult> BuildIndexAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(Views(
                View<AdminIndex>("Admin.Index.Header", model => viewModel).Zone("header").Order(1),
                View<AdminIndex>("Admin.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<AdminIndex>("Admin.Index.Content", model => viewModel).Zone("content").Order(1)
            ));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
