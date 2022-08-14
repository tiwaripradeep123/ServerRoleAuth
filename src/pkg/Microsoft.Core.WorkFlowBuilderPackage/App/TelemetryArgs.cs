using System;

namespace CoreFeed.Core.Com.Entity.App
{
    public class TelemetryArgs
    {
        /// <summary>
        /// new TelemetryArgs() is equal to TelemetryArgs.DefaultInstance" ||
        /// new TelemetryArgs() {ThrowException = true } is equal to TelemetryArgs.DefaultInstance" ||
        /// new TelemetryArgs() {ThrowException = false } is equal to TelemetryArgs.SilentInstance" ||
        /// </summary>
        [Obsolete("Use DefaultInstance or SilentInstance e.g new TelemetryArgs() is equal to TelemetryArgs.DefaultInstance")]
        public TelemetryArgs()
        {
        }

        private TelemetryArgs(bool throwException = true, bool invokerCall = false)
        {
            this.ThrowException = throwException;
            this.InvokerCall = invokerCall;
        }

        /// <summary>
        /// Throws Exception ||
        /// new TelemetryArgs() is equal to TelemetryArgs.DefaultInstance" ||
        /// new TelemetryArgs() {ThrowException = true } is equal to TelemetryArgs.DefaultInstance" ||
        /// new TelemetryArgs() {ThrowException = false } is equal to TelemetryArgs.SilentInstance"
        /// </summary>
        public static TelemetryArgs DefaultInstance { get { return new TelemetryArgs(true, false); } }

        /// <summary>
        /// Should not be used in other than Invoker apis
        /// </summary>
        public static TelemetryArgs InvokerInstance { get { return new TelemetryArgs(true, true); } }

        /// <summary>
        /// Does not throws Exception ||
        /// new TelemetryArgs() is equal to TelemetryArgs.DefaultInstance" ||
        /// new TelemetryArgs() {ThrowException = true } is equal to TelemetryArgs.DefaultInstance" ||
        /// new TelemetryArgs() {ThrowException = false } is equal to TelemetryArgs.SilentInstance"
        /// </summary>
        public static TelemetryArgs SilentInstance { get { return new TelemetryArgs(false, false); } }

        public bool InvokerCall { get; set; } = false;
        public bool ThrowException { get; set; } = true;
    }
}