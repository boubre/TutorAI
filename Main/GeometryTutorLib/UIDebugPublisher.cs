using System;

namespace GeometryTutorLib
{
    /// <summary>
    /// This class is used to asyncrhonously publish strings to the UI.
    /// The strings are displayed to the UI in the AI Debug Window, and the actual action of publishing or clearing
    /// will happen on the UI thread.
    /// </summary>
    public class UIDebugPublisher
    {
        private Action<String> publishStringAction;
        private Action clearWindowAction;

        /// <summary>
        /// Create a new UIDebugPublisher with publish and clear actions.
        /// </summary>
        /// <param name="publishStringAction">An Action that publishes the given string to the UI. This Action should invoke the actual update operation on the UI thread.</param>
        /// <param name="clearWindowAction">An Action that clears all debug strings from the UI. This Action should invoke the actual update operation on the UI thread.</param>
        public UIDebugPublisher(Action<String> publishStringAction, Action clearWindowAction) 
        {
            this.publishStringAction = publishStringAction;
            this.clearWindowAction = clearWindowAction;
        }

        /// <summary>
        /// Publish a string to the AI Debug Window.
        /// </summary>
        /// <param name="str">The string to publish</param>
        public void publishString(String str)
        {
            publishStringAction(str);
        }

        /// <summary>
        /// Clear the AI Debug Window of all strings.
        /// </summary>
        public void clearWindow()
        {
            clearWindowAction();
        }
    }
}
