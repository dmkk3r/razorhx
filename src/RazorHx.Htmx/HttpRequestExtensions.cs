﻿using Microsoft.AspNetCore.Http;

namespace RazorHx.Htmx;

public static class HttpRequestExtensions
{
    /// <summary>
    ///     Returns true if the request is an HTMX request.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static bool IsHtmxRequest(this HttpRequest request)
    {
        return new HtmxRequest(request).Request;
    }

    /// <summary>
    ///     Returns true if the request is a boosted HTMX request.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static bool IsBoostedRequest(this HttpRequest request)
    {
        return new HtmxRequest(request).Boosted;
    }
}