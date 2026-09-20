using Microsoft.Extensions.Logging;
#if ANDROID
using Android.Runtime;
#endif

namespace CraftingCalculator.UI.Logging;

/// <summary>
/// Installs the process-wide exception hooks, so a failure no page, boundary or catch block ever sees still
/// reaches the diagnostic log.
/// </summary>
internal static partial class GlobalExceptionHandler
{
    /// <summary>
    /// Subscribes every last-resort hook the current platform offers, using the given logger for all of them.
    /// Call once per process, as early as a logger exists.
    /// </summary>
    internal static void Install(ILogger logger)
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            // ExceptionObject is typed object because the CLR permits a non-Exception throw from other
            // languages. Only that payload's type name is logged - its ToString is arbitrary text this code
            // did not author, which L2 keeps out of the file.
            if (args.ExceptionObject is Exception exception)
            {
                LogAppDomainUnhandled(logger, exception, args.IsTerminating);
            }
            else
            {
                LogAppDomainUnhandledPayload(logger, args.ExceptionObject.GetType().Name, args.IsTerminating);
            }
        };

        // A faulted Task nobody awaited. SetObserved keeps the failure benign - an unobserved exception is
        // re-raised from the finalizer thread - while the stack trace still reaches the file.
        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            LogUnobservedTask(logger, args.Exception);
            args.SetObserved();
        };

#if ANDROID
        // Raised as a managed exception is about to cross into the Java runtime, where no managed catch can
        // reach it. It fires before the process dies, so this entry is usually the only managed stack trace
        // an Android crash leaves behind. Record only: the Java side has already unwound past the frame that
        // threw, so marking it handled would leave the app running on state the managed side abandoned.
        AndroidEnvironment.UnhandledExceptionRaiser += (_, args) =>
            LogAndroidUnhandled(logger, args.Exception, args.Handled);
#endif
#if IOS
        // The iOS twin of the Android hook: a managed exception about to be marshalled into the Objective-C
        // runtime. Record only, for the same reason.
        ObjCRuntime.Runtime.MarshalManagedException += (_, args) =>
            LogMarshalManagedException(logger, args.Exception, args.ExceptionMode);
#endif
#if WINDOWS
        // The one hook that recovers rather than records. WinUI tears the process down on an unhandled
        // exception even where the window is still usable, and the desktop head has no crash dialog of its
        // own, so the app survives with an entry instead. Bare "Application" would bind to the sibling
        // CraftingCalculator.Application namespace, hence the full name.
        Microsoft.UI.Xaml.Application.Current.UnhandledException += (_, args) =>
        {
            // args.Message is logged beside the exception because WinUI frequently hands over an Exception
            // with no StackTrace attached, leaving the message as the only description of the failure. It is
            // framework-authored text, and the redactor still collapses any path in it; what it does not
            // reach is quoted-value masking, which only runs over exception messages.
            LogWindowsUnhandled(logger, args.Exception, args.Message);
            args.Handled = true;
        };
#endif
    }

    // Source-generated, per L1, and so the bool and enum arguments below stay unboxed.
    [LoggerMessage(Level = LogLevel.Critical,
        Message = "Unhandled exception reached the AppDomain (terminating: {IsTerminating})")]
    private static partial void LogAppDomainUnhandled(ILogger logger, Exception exception, bool isTerminating);

    [LoggerMessage(Level = LogLevel.Critical,
        Message = "Unhandled non-exception payload of type {PayloadType} reached the AppDomain "
                  + "(terminating: {IsTerminating})")]
    private static partial void LogAppDomainUnhandledPayload(ILogger logger, string payloadType, bool isTerminating);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "A faulted task was collected with its exception never observed")]
    private static partial void LogUnobservedTask(ILogger logger, Exception exception);

#if ANDROID
    [LoggerMessage(Level = LogLevel.Critical,
        Message = "Unhandled exception crossing into the Android runtime (handled: {Handled})")]
    private static partial void LogAndroidUnhandled(ILogger logger, Exception exception, bool handled);
#endif
#if IOS
    [LoggerMessage(Level = LogLevel.Critical,
        Message = "Managed exception marshalled into the Objective-C runtime (mode: {Mode})")]
    private static partial void LogMarshalManagedException(
        ILogger logger, Exception exception, ObjCRuntime.MarshalManagedExceptionMode mode);
#endif
#if WINDOWS
    [LoggerMessage(Level = LogLevel.Critical,
        Message = "Unhandled exception reached WinUI and was handled; the app was left running\n{Message}")]
    private static partial void LogWindowsUnhandled(ILogger logger, Exception exception, string message);
#endif
}
