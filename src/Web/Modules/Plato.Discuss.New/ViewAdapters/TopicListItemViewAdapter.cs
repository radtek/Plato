﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.ViewAdapters;
using System.Collections.Generic;
using PlatoCore.Data.Abstractions;
using Plato.Entities.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Localization;
using PlatoCore.Security.Abstractions;
using PlatoCore.Layout.Models;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Hosting.Abstractions;
using PlatoCore.Layout.Views;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using PlatoCore.Layout;
using PlatoCore.Layout.ModelBinding;

namespace Plato.Discuss.New.ViewAdapters
{

    public class TopicListItemViewAdapter : BaseAdapterProvider
    {

        private readonly IDbHelper _dbHelper;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IAuthorizationService _authorizationService;        
        private readonly IHttpContextAccessor _httpContextAccessor;    
        private readonly IEntityService<Topic> _entityService;
        private readonly IContextFacade _contextFacade;
        private readonly IViewTable _viewTableManager;
        private readonly IViewResultTable _viewResultTable;

        private readonly IUpdateModelAccessor _updateModelAccessor;

        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly ILayoutModelAccessor _layoutModelAccesssor;

        public IHtmlLocalizer T { get; }

        public TopicListItemViewAdapter(
            IHtmlLocalizer<TopicListItemViewAdapter> localizer,
            IActionContextAccessor actionContextAccessor,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IEntityService<Topic> entityService,
            IViewTable viewTableManager,
            IContextFacade contextFacade,
            IModelMetadataProvider modelMetadataProvider,
            IViewResultTable viewResultTable,
            ILayoutModelAccessor layoutModelAccesssor,
            IUpdateModelAccessor updateModelAccessor,
            IDbHelper dbHelper)
        {

            _actionContextAccessor = actionContextAccessor;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
            _viewTableManager = viewTableManager;
            _entityService = entityService;
            _contextFacade = contextFacade;
            _dbHelper = dbHelper;
            _modelMetadataProvider = modelMetadataProvider;
            _viewResultTable = viewResultTable;

            _layoutModelAccesssor = layoutModelAccesssor;
            _updateModelAccessor = updateModelAccessor;

            T = localizer;
            ViewName = "TopicListItem";

        }

        IDictionary<int, DateTimeOffset?> _lastVisits;

        public override async Task<IViewAdapterResult> ConfigureAsync(string viewName)
        {

            // Ensure adapter is for current view
            if (!viewName.Equals(ViewName, StringComparison.OrdinalIgnoreCase))
            {
                return default(IViewAdapterResult);
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We need to be authenticated
            if (user == null)
            {
                return default(IViewAdapterResult);
            }

            // Adapt the view
            return await Adapt(ViewName, v =>
            {
                v.AdaptModel<EntityListItemViewModel<Topic>>(async model =>
                {

                    // Build last visits from metrics
                    if (_lastVisits == null)
                    {
                        // Get displayed entities
                        var entities = await GetDisplayedEntitiesAsync();
                        if (entities?.Data != null)
                        {
                            _lastVisits = await SelectLatestViewDateForEntitiesAsync(user.Id, entities.Data.Select(e => e.Id).ToArray());
                        }
                    }

                    // No metrics available to adapt the view
                    if (_lastVisits == null)
                    {
                        // Return an anonymous type, we are adapting a view component
                        return new
                        {
                            model
                        };
                    }


                    if (model.Entity == null)
                    {
                        // Return an anonymous type, we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    DateTimeOffset? lastVisit = null;
                    if (_lastVisits.ContainsKey(model.Entity.Id))
                    {
                        lastVisit = _lastVisits[model.Entity.Id];
                    }

                    // Ensure tag alterations
                    if (model.TagAlterations == null)
                    {
                        model.TagAlterations = new TagAlterations();
                    }

                    // Build tag alterations
                    var alterations = new[]
                    {
                        new TagAlteration("title", (context, output) =>
                        {
                            if (lastVisit != null)
                            {

                                var suppressAlterations = false;

                                // Last reply
                                if (model.Entity.LastReplyDate.HasValue)
                                {
                                    if (model.Entity.LastReplyDate > lastVisit)
                                    {
                                        output.PostElement.SetHtmlContent(
                                            $"<span class=\"badge badge-primary ml-2\">{T["New"].Value}</span>");
                                        suppressAlterations = true;
                                    }
                                }
                                
                                // Modified
                                if (model.Entity.ModifiedDate.HasValue && !suppressAlterations)
                                {
                                    if (model.Entity.ModifiedDate > lastVisit)
                                    {
                                        output.PostElement.SetHtmlContent(
                                            $"<span class=\"badge badge-secondary ml-2\">{T["Updated"].Value}</span>");
                                        suppressAlterations = true;
                                    }
                                }

                                // Created
                                if (model.Entity.CreatedDate.HasValue && !suppressAlterations)
                                {
                                    if (model.Entity.CreatedDate > lastVisit)
                                    {
                                        output.PostElement.SetHtmlContent(
                                            $"<span class=\"badge badge-primary ml-2\">{T["New"].Value}</span>");
                                        suppressAlterations = true;
                                    }
                                }

                            }
                            else
                            {
                                output.PostElement.SetHtmlContent(
                                    $"<span class=\"badge badge-primary ml-2\">{T["New"].Value}</span>");
                            }
                        })
                    };

                    // Apply tag alterations
                    model.TagAlterations.Add(alterations);

                    // Return an anonymous type, we are adapting a view component
                    return new
                    {
                        model
                    };

                });
            });

        }

        private async Task<IPagedResults<Topic>> GetDisplayedEntitiesAsync()
        {

 
            var viewComponentResults = _viewResultTable.FirstViewComponentWithType<EntityIndexViewModel<Topic>>();


            var layoutModel = _layoutModelAccesssor.LayoutViewModel;

            var updateModel = _updateModelAccessor.ModelUpdater;

            var metaData = _modelMetadataProvider.GetMetadataForType(typeof(EntityIndexViewModel<Topic>));
            var modelExplorer = new ModelExplorer(_modelMetadataProvider, metaData, null);

            

            var viewDataDictionary = new ViewDataDictionary<EntityIndexViewModel<Topic>>(_modelMetadataProvider, new ModelStateDictionary());
            //viewDataDictionary.Model = new FooModel();

            // Get topic index view model from context
            var viewModel2 = _viewTableManager.FirstModelOfType<EntityIndexViewModel<Topic>>();
            if (viewModel2 == null)
            {
                return null;
            }

            // Get topic index view model from context
            var viewModel = _actionContextAccessor.ActionContext.HttpContext.Items[typeof(EntityIndexViewModel<Topic>)] as EntityIndexViewModel<Topic>;
            if (viewModel == null)
            {
                return null;
            }


            // Get all entities for our current view
            return await _entityService
                .ConfigureQuery(async q =>
                {

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User,
                        Permissions.ViewPrivateTopics))
                    {
                        q.HidePrivate.True();
                    }

                    // Hide hidden?
                    if (!await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User,
                        Permissions.ViewHiddenTopics))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User,
                        Permissions.ViewSpamTopics))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User,
                        Permissions.ViewDeletedTopics))
                    {
                        q.HideDeleted.True();
                    }

                })
                .GetResultsAsync(viewModel?.Options, viewModel?.Pager);

        }

        public async Task<IDictionary<int, DateTimeOffset?>> SelectLatestViewDateForEntitiesAsync(int userId, int[] entityIds)
        {

            const string sql = @"                
                SELECT 
                    em.EntityId AS EntityId, 
                    MAX(em.CreatedDate) AS CreatedDate
                FROM 
                     {prefix}_EntityMetrics em
                WHERE
                    em.CreatedUserId = {userId} AND
                    em.EntityId IN ({entityIds})
                GROUP BY (em.EntityId)
            ";

            // Sql replacements
            var replacements = new Dictionary<string, string>()
            {
                ["{userId}"] = userId.ToString(),
                ["{entityIds}"] = entityIds.ToDelimitedString()
            };

            // Execute and return results
            return await _dbHelper.ExecuteReaderAsync(sql, replacements, async dr =>
            {
                var output = new Dictionary<int, DateTimeOffset?>();
                while (await dr.ReadAsync())
                {

                    var key = 0;
                    DateTimeOffset? value = null;

                    if (dr.ColumnIsNotNull("EntityId"))
                        key = Convert.ToInt32((dr["EntityId"]));

                    if (dr.ColumnIsNotNull("CreatedDate"))
                        value = (DateTimeOffset) (dr["CreatedDate"]);

                    if (!output.ContainsKey(key))
                    {
                        output[key] = value;
                    }

                }

                return output;
            });

        }

    }

}