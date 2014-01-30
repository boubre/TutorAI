using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
using DynamicGeometry.UI;

namespace LiveGeometry
{
    /// <summary>
    /// Executes the front end parse chain in sequence and allows for blocking dialogs to be shown in between each action in the chain.
    /// </summary>
    public class ParseController
    {
        private Queue<Action> parseActions;
        private Queue<ParseDisambiguationWindow> dialogs;
        private AutoResetEvent waitHandle;

        /// <summary>
        /// Create a new ParseController.
        /// </summary>
        public ParseController()
        {
            waitHandle = new AutoResetEvent(false);
            parseActions = new Queue<Action>();
            dialogs = new Queue<ParseDisambiguationWindow>();
        }

        /// <summary>
        /// Add a parse action to the the series of events that needs to be executed during parsing.
        /// </summary>
        /// <param name="a">The action to execute after the previously added action.</param>
        public void addParseAction(Action a)
        {
            parseActions.Enqueue(a);
        }

        /// /// <summary>
        /// Add a dialog that will be displayed after the current action is complete.
        /// This dialog will block the parse thread but not the UI, and the next parse action cannot occur until
        /// after all dialogs have been resolved.
        /// </summary>
        /// <param name="message">The message of the dialog.</param>
        /// <param name="title">The title of the dialog.</param>
        /// /// <param name="parameters">Parameters to pass to the yes/no actions.</param>
        /// <param name="yesAction">The Action to execute if the Yes button is clicked.</param>
        /// <param name="noAction">The Action to execute if the No button is clicked.</param>  
        public void addDialog(string message, string title, object[] parameters, Action<object[]> yesAction, Action<object[]> noAction)
        {
            ParseDisambiguationWindow pdw = new ParseDisambiguationWindow(message, title);
            pdw.Closed += new EventHandler((object sender, EventArgs e) =>
            {
                if (pdw.DialogResult == ParseDisambiguationWindow.Result.Yes)
                {
                    yesAction(parameters);
                }
                else if (pdw.DialogResult == ParseDisambiguationWindow.Result.No)
                {
                    noAction(parameters);
                }
                waitHandle.Set();
            });
            dialogs.Enqueue(pdw);
        }

        /// <summary>
        /// Execute the front-end parse.
        /// </summary>
        public void executeParse()
        {
            //Execute each parse action in sequence on the UI thread, then wait for the action to complete.
            while (parseActions.Count > 0)
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    parseActions.Dequeue()();
                    waitHandle.Set();
                });
                waitHandle.WaitOne();
                waitHandle.Reset();

                //If the action added any dialogs, display them all now beore the next action.
                //The next action or dialog will not be activated until the current dialog is resolved.
                while (dialogs.Count > 0)
                {
                    SmartDispatcher.BeginInvoke(() =>
                    {
                        dialogs.Dequeue().Show();
                    });
                    waitHandle.WaitOne();
                    waitHandle.Reset();
                }
            }
        }
    }
}