// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Commands;
using Dolittle.Commands.Coordination.Runtime;
using Dolittle.Logging;
using Microsoft.AspNetCore.Http;

namespace Dolittle.AspNetCore.Commands
{
    /// <summary>
    /// Represents an API endpoint for working with <see cref="ICommand">commands</see>.
    /// </summary>
    public class CommandCoordinator
    {
        readonly ICommandCoordinator _commandCoordinator;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandCoordinator"/> class.
        /// </summary>
        /// <param name="commandCoordinator">The underlying <see cref="ICommandCoordinator"/>.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public CommandCoordinator(
            ICommandCoordinator commandCoordinator,
            ILogger logger)
        {
            _commandCoordinator = commandCoordinator;
            _logger = logger;
        }

        /// <summary>
        /// Handles a <see cref="CommandRequest" /> from the <see cref="HttpRequest.Body" /> and writes the <see cref="CommandResult" /> to the <see cref="HttpResponse" />.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" />.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Handle(HttpContext context)
        {
            CommandRequest command = null;
            try
            {
                command = await context.RequestBodyFromJson<CommandRequest>().ConfigureAwait(false);
                var result = _commandCoordinator.Handle(command);
                var status = result switch
                    {
                        { Success: true } => StatusCodes.Status200OK,
                        { PassedSecurity: false } => StatusCodes.Status403Forbidden,
                        { Invalid: true } => StatusCodes.Status400BadRequest,
                        { HasBrokenRules: true } => StatusCodes.Status400BadRequest,
                        _ => StatusCodes.Status500InternalServerError
                    };
                await context.RespondWithStatusCodeAndResult(status, result).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not handle command request");
                await context.RespondWithStatusCodeAndResult(
                    StatusCodes.Status500InternalServerError,
                    new CommandResult
                        {
                            Command = command,
                            Exception = ex,
                            ExceptionMessage = ex.Message
                        }).ConfigureAwait(false);
            }
        }
    }
}