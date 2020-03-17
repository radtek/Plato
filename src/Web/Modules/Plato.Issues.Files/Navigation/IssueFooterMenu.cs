﻿using System;
using Microsoft.Extensions.Localization;
using Plato.Issues.Models;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Issues.Files.Navigation
{

    public class IssueFooterMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public IssueFooterMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "issue-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Issue)] as Issue;

            // We always need a topic
            if (entity == null)
            {
                return;
            }

            // Replies are options
            var reply = builder.ActionContext.HttpContext.Items[typeof(Comment)] as Comment;

            builder
                .Add(T["Attachments"], react => react
                    .View("EntityFiles", new
                    {
                        entity,
                        reply
                    })
                );

        }

    }

}
