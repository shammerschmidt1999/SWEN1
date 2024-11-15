using System;
using System.ComponentModel;
using System.Reflection;
using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Classes.HttpSvr.Handlers
{
    public abstract class Handler : IHandler
    {
        // Properties
        private static List<IHandler>? _Handlers = null;

        // Methods
        /// <summary>
        /// Retrieves a list of all non-abstract types that implement the IHandler interface.
        /// </summary>
        /// <returns>A list of instances of types that implement the IHandler interface.</returns>
        /// <remarks>
        /// This method uses reflection to find all types in the current assembly that implement the IHandler interface and are not abstract.
        /// It then creates an instance of each type and adds it to the list of handlers.
        /// </remarks>
        private static List<IHandler> _GetHandlers()
        {
            List<IHandler> rval = new();

            foreach (Type i in Assembly.GetExecutingAssembly().GetTypes()
                              .Where(m => m.IsAssignableTo(typeof(IHandler)) && !m.IsAbstract))
            {
                IHandler? h = (IHandler?)Activator.CreateInstance(i);
                if (h != null)
                {
                    rval.Add(h);
                }
            }

            return rval;
        }
        /// <summary>
        /// Handles an HTTP server event by delegating it to the appropriate handler.
        /// </summary>
        /// <param name="e">The event arguments containing the details of the HTTP request.</param>
        /// <remarks>
        /// This method initializes the list of handlers if it is null, then iterates through each handler to find one that can handle the event.
        /// If a handler successfully handles the event, the method returns immediately.
        /// If no handler can handle the event, it replies with a BAD_REQUEST status.
        /// </remarks>
        public static void HandleEvent(HttpSvrEventArgs e)
        {
            _Handlers ??= _GetHandlers();

            foreach (IHandler i in _Handlers)
            {
                if (i.Handle(e)) return;
            }
            e.Reply(HttpStatusCode.BAD_REQUEST);
        }
        public abstract bool Handle(HttpSvrEventArgs e);
    }
}
