// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Domain;
using Dolittle.Events;

namespace Debugging.MyFeature
{
    /// <summary>
    /// Example AggregateRoot.
    /// </summary>
    public class MyAggregateRoot : AggregateRoot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyAggregateRoot"/> class.
        /// </summary>
        /// <param name="eventSourceId">cool id.</param>
        public MyAggregateRoot(EventSourceId eventSourceId)
            : base(eventSourceId)
        {
        }

        /// <summary>
        /// Example method.
        /// </summary>
        /// <param name="myStrings">String to be applied to the event.</param>
        public void DoThing(IEnumerable<CommandString> myStrings)
        {
            foreach (var myString in myStrings)
            {
                Apply(new MyEvent(myString));
            }
        }
    }
}
