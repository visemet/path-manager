//-----------------------------------------------------------------------
// <copyright file="Proxy.cs" company="Visemet">
//     Copyright (c) Visemet. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Visemet.Environment
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Microsoft.Win32;

    /// <summary>
    /// Proxy pattern for System.Environment.
    /// </summary>
    /// <remarks>
    /// Uses Microsoft.Win32.Registry to retrieve environment variables
    /// without expanded names, and stores environment variables with
    /// unexpanded names.
    /// </remarks>
    public static class Proxy
    {
        /// <summary>
        /// Retrieves all environment variable names and their values
        /// from the Windows operating system registry key for the
        /// current user or local machine.
        /// </summary>
        /// <param name="target">
        /// One of the <see cref="System.EnvironmentVariableTarget" /> values.
        /// </param>
        /// <returns>
        /// A dictionary that contains all environment variables
        /// names and their values from the source specified by the
        /// target parameter; otherwise, an empty dictionary if no
        /// environment variables are found.
        /// </returns>
        public static IDictionary<string, string> GetEnvironmentVariables(EnvironmentVariableTarget target)
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();

            string keyName;
            RegistryKey registryKey;

            switch (target)
            {
                case EnvironmentVariableTarget.Machine:
                    keyName = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
                    registryKey = Registry.LocalMachine.OpenSubKey(keyName);
                    break;
                case EnvironmentVariableTarget.User:
                    keyName = @"Environment";
                    registryKey = Registry.CurrentUser.OpenSubKey(keyName);
                    break;
                default:
                    throw new ArgumentException("Invalid environment variable target", "target");
            }

            foreach (string name in registryKey.GetValueNames())
            {
                variables.Add(name, (string)registryKey.GetValue(name, null, RegistryValueOptions.DoNotExpandEnvironmentNames));
            }

            return variables;
        }

        /// <summary>
        /// Creates, modifies, or deletes an environment variable stored
        /// in the Windows operating system registry key reserved for the
        /// current user or local machine.
        /// </summary>
        /// <param name="variable">
        /// The name of an environment variable.
        /// </param>
        /// <param name="value">A value to assign to variable.</param>
        /// <param name="target">
        /// One of the <see cref="System.EnvironmentVariableTarget" /> values.
        /// </param>
        public static void SetEnvironmentVariable(string variable, string value, EnvironmentVariableTarget target)
        {
            string keyName;
            RegistryKey registryKey;

            switch (target)
            {
                case EnvironmentVariableTarget.Machine:
                    keyName = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
                    registryKey = Registry.LocalMachine.OpenSubKey(keyName, true);
                    break;
                case EnvironmentVariableTarget.User:
                    keyName = @"Environment";
                    registryKey = Registry.CurrentUser.OpenSubKey(keyName, true);
                    break;
                default:
                    throw new ArgumentException("Invalid environment variable target", "target");
            }

            if (value == null)
            {
                registryKey.DeleteValue(variable, false);
            }
            else
            {
                registryKey.SetValue(variable, value, RegistryValueKind.ExpandString);
            }

            Notification.EnvironmentChanged();
        }

        /// <summary>
        /// Utility class for sending messages to one or more windows.
        /// </summary>
        private class Notification
        {
            /// <summary>
            /// Specifies the behavior of the
            /// <see cref="SendMessageTimeout(IntPtr, uint, IntPtr, string, SendMessageTimeFlags, uint, out IntPtr)"/>
            /// method.
            /// </summary>
            [Flags]
            private enum SendMessageTimeoutFlags : uint
            {
                /// <summary>
                /// The calling thread is not prevented from processing
                /// other requests while waiting for the function to
                /// return.
                /// </summary>
                SMTO_NORMAL = 0x0,

                /// <summary>
                /// Prevents the calling thread from processing any other
                /// requests until the function returns.
                /// </summary>
                SMTO_BLOCK = 0x1,

                /// <summary>
                /// The function returns without waiting for the time-out
                /// period to elapse if the receiving thread appears to
                /// not respond or "hangs."
                /// </summary>
                SMTO_ABORTIFHUNG = 0x2,

                /// <summary>
                /// The function does not enforce the time-out period as
                /// long as the receiving thread is processing messages.
                /// </summary>
                SMTO_NOTIMEOUTIFNOTHUNG = 0x8
            }

            /// <summary>
            /// Notifies all relevant windows that the environment
            /// variables have changed.
            /// </summary>
            public static void EnvironmentChanged()
            {
                IntPtr result;
                NativeMethods.SendMessageTimeout(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SETTINGCHANGE,
                    (IntPtr)0,
                    "Environment",
                    SendMessageTimeoutFlags.SMTO_ABORTIFHUNG,
                    5000,
                    out result);
            }

            /// <summary>
            /// Defines the constants necessary for sending messages.
            /// </summary>
            private class NativeMethods
            {
                /// <summary>
                /// Indicates message to send to all top-level windows in the
                /// system, including disabled or invisible unowned windows.
                /// </summary>
                public const int HWND_BROADCAST = 0xffff;

                /// <summary>
                /// Indicates changes to a system-wide setting or that policy
                /// settings have changed.
                /// </summary>
                public const int WM_SETTINGCHANGE = 0x001a;

                [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
                public static extern IntPtr SendMessageTimeout(
                    IntPtr windowHandle,
                    uint message,
                    IntPtr wordParam,
                    string longParam,
                    SendMessageTimeoutFlags flags,
                    uint timeout,
                    out IntPtr result);
            }
        }
    }
}
