﻿namespace RazorHx.Htmx;

public static class HtmxRequestHeaderKeys
{
    /// <summary>
    ///     Indicates that the request is via an element using hx-boost.
    /// </summary>
    public const string Boosted = "HX-Boosted";

    /// <summary>
    ///     The current URL of the browser.
    /// </summary>
    public const string CurrentUrl = "HX-Current-URL";

    /// <summary>
    ///     “true” if the request is for history restoration after a miss in the local history cache.
    /// </summary>
    public const string HistoryRestoreRequest = "HX-History-Restore-Request";

    /// <summary>
    ///     The user response to an hx-prompt.
    /// </summary>
    public const string Prompt = "HX-Prompt";

    /// <summary>
    ///     Always “true”.
    /// </summary>
    public const string Request = "HX-Request";

    /// <summary>
    ///     The id of the target element if it exists.
    /// </summary>
    public const string Target = "HX-Target";

    /// <summary>
    ///     The name of the triggered element if it exists.
    /// </summary>
    public const string TriggerName = "HX-Trigger-Name";

    /// <summary>
    ///     The id of the triggered element if it exists.
    /// </summary>
    public const string Trigger = "HX-Trigger";
}