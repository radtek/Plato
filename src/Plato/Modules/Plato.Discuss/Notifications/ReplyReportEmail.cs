﻿using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Discuss.Models;
using Plato.Discuss.NotificationTypes;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Localization.Extensions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Notifications.Abstractions;

namespace Plato.Discuss.Notifications
{
    public class ReplyReportEmail : INotificationProvider<Reply>
    {

        private readonly IContextFacade _contextFacade;
        private readonly ILocaleStore _localeStore;
        private readonly IEmailManager _emailManager;
        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;
        private readonly IEntityStore<Topic> _topicStore;

        public ReplyReportEmail(
            IContextFacade contextFacade,
            ILocaleStore localeStore,
            IEmailManager emailManager,
            ICapturedRouterUrlHelper capturedRouterUrlHelper,
            IEntityStore<Topic> topicStore)
        {
            _contextFacade = contextFacade;
            _localeStore = localeStore;
            _emailManager = emailManager;
            _capturedRouterUrlHelper = capturedRouterUrlHelper;
            _topicStore = topicStore;
        }

        public async Task<ICommandResult<Reply>> SendAsync(INotificationContext<Reply> context)
        {

            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(EmailNotifications.ReplyReport.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new CommandResult<Reply>();

            // Get email template
            const string templateId = "NewReplyReport";
            var culture = await _contextFacade.GetCurrentCultureAsync();
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, templateId);
            if (email == null)
            {
                return result.Failed(
                    $"No email template with the Id '{templateId}' exists within the 'locales/{culture}/emails.json' file!");
            }

            // Get topic for reply
            var topic = await _topicStore.GetByIdAsync(context.Model.EntityId);

            // We need an topic for the reply
            if (topic == null)
            {
                return result.Failed(
                    $"No entity with id '{context.Model.EntityId}' exists. Failed to send reply spam email notification.");
            }

            // Build topic url
            var baseUri = await _capturedRouterUrlHelper.GetBaseUrlAsync();
            var url = _capturedRouterUrlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias,
                ["opts.replyId"] = context.Model.Id
            });

            // Build message from template
            var message = email.BuildMailMessage();
            message.Body = string.Format(
                email.Message,
                context.Notification.To.DisplayName,
                topic.Title,
                baseUri + url);
            ;
            message.IsBodyHtml = true;
            message.To.Add(new MailAddress(context.Notification.To.Email));

            // Send message
            var emailResult = await _emailManager.SaveAsync(message);
            if (emailResult.Succeeded)
            {
                return result.Success(context.Model);
            }

            return result.Failed(emailResult.Errors?.ToArray());

        }

    }
}